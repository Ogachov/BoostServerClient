using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class LogClient
{
    private LogStateObject state;
    private ManualResetEvent done = new ManualResetEvent(false);
    private SynchronizationContext context;

    public event EventHandler<LogClientEventArgs> OnReceive;
    public void Connect(int port)
    {
        context = SynchronizationContext.Current;
        
        var addr = IPAddress.Parse("127.0.0.1");
        var endPoint = new IPEndPoint(addr, 24680);

        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(endPoint);

        state = new LogStateObject(socket) {IsHeader = true, rest = 4};

//        Send("Message from LogClient");

        socket.BeginReceive(state.buffer, 0, state.rest, 0, ReceiveCallback, state);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        var s = (LogStateObject) ar.AsyncState;
        var socket = s.socket;
        var length = socket.EndReceive(ar);

        if (length == 0)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            return;
        }

        s.rest -= length;
        if (s.IsHeader)
        {
            if (s.rest > 0)
            {
                socket.BeginReceive(s.buffer, 4 - s.rest, s.rest, 0, ReceiveCallback, s);
            }
            else
            {
                s.bodyLength = BitConverter.ToInt32(s.buffer, 0);
                if (s.bodyLength == 0)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                    return;
                }

                s.rest = s.bodyLength;
                s.IsHeader = false;
                s.recvStream = new MemoryStream();
                var size = s.rest > s.buffer.Length ? s.buffer.Length : s.rest;
                socket.BeginReceive(s.buffer, 0, size, 0, ReceiveCallback, s);
            }
        }
        else
        {
            s.recvStream.Write(s.buffer, 0, length);
            if (s.rest > 0)
            {
                var size = s.rest > s.buffer.Length ? s.buffer.Length : s.rest;
                socket.BeginReceive(s.buffer, 0, size, 0, ReceiveCallback, s);
            }
            else
            {
                // 受信が終わったデータをどうにかする
                if (OnReceive != null)
                {
                    context.Post(_ =>
                    {
                        OnReceive(this, new LogClientEventArgs(s.recvStream));
                    },null);
                }

                s.IsHeader = true;
                s.rest = 4;
                socket.BeginReceive(s.buffer, 0, s.rest, 0, ReceiveCallback, s);
            }
        }
    }

    public void Send(string text)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        var len = bytes.Length;
        using (var ms = new MemoryStream())
        {
            var bw = new BinaryWriter(ms);

            var bodyLength = len + 4;
            bw.Write(bodyLength);
            bw.Write((int) LogCommand.Command.Text);
            bw.Write(bytes);

            state.socket.Send(ms.ToArray());
        }
    }

    public void SendClientId(int id)
    {
        using (var ms = new MemoryStream())
        {
            var bw = new BinaryWriter(ms);
            bw.Write((int)0);
            bw.Write((int)LogCommand.Command.ClientId);
            bw.Write((int)id);

            var pos = bw.BaseStream.Position;
            bw.BaseStream.Seek(0, SeekOrigin.Begin);
            var len = (int) pos - 4;
            bw.Write(len);
            bw.BaseStream.Seek(pos, SeekOrigin.Begin);

            state.socket.Send(ms.ToArray());
        }
    }
}

