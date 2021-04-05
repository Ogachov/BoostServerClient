using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;

namespace Target
{
    public partial class TargetForm : Form
    {
        private LogClient client;

        public TargetForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client = new LogClient();
            client.OnReceive += OnReceive;
            client.Connect(24680);
        }

        private void OnReceive(object sender, LogClientEventArgs e)
        {
            textBox1.AppendText($"Receive {e.ReceivedStream.Length}bytes");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client.Send(textBoxMessage.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.SendClientId(12345);
        }
    }
}
