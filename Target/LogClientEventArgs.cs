using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LogClientEventArgs : EventArgs
{
    public LogClientEventArgs(MemoryStream ms)
    {
        ReceivedStream = ms;
    }

    public MemoryStream ReceivedStream { get; set; }
}
