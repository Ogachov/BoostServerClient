using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Host
{
    public partial class ViewerForm : Form
    {
        private LogClient client;

        public ViewerForm()
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
            textBoxLog.AppendText($"Receive {e.ReceivedStream.Length}bytes");
        }
    }
}
