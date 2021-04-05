using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;

public class LogStateObject
{
    public System.Net.Sockets.Socket socket;
    public byte[] buffer;
    public bool IsHeader;
    public int rest;
    public int bodyLength;
    public MemoryStream recvStream;

    public LogStateObject(Socket socket)
    {
        this.socket = socket;
        buffer = new byte[1024];
    }
}
