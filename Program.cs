using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using usbip_tunnel.forms;
using usbip_tunnel.net;

namespace usbip_tunnel
{
    public class Program
    {
        public static AppContext Context { get; private set; }

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Context = new AppContext(args));
        }
    }
}
