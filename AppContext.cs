using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using usbip_tunnel.forms;

namespace usbip_tunnel
{
    public class AppContext : ApplicationContext
    {
        private static AppContext s_Instance;

        public static AppContext GetInstance(string[] args)
        {
            if (s_Instance == null) s_Instance = new AppContext(args);
            return s_Instance;
        }

        public ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public TunnelServer Server { get; private set; }
        public NotifyIcon TrayIcon { get; private set; }

        private LogForm m_LogForm = new LogForm();


        public AppContext(string[] args) : base()
        {
            Application.ApplicationExit += new EventHandler(this.OnApplicationExit);

            this.Server = TunnelServer.Instance;

            this.TrayIcon = new NotifyIcon();
            this.TrayIcon.Text = "usbip-tunnel";
            this.TrayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseUp += TrayIcon_MouseUp;
            this.TrayIcon.ContextMenuStrip = new ContextMenuStrip();
            this.TrayIcon.ContextMenuStrip.Items.Add("Показать лог", null, (object sender, EventArgs e) =>
            {
                m_LogForm.ShowDialog();
            });

            ToolStripItem startToolStripItem = this.TrayIcon.ContextMenuStrip.Items.Add("Запустить сервер", null, (object sender, EventArgs e) =>
            {
                this.Server.Start(args);

                (sender as ToolStripItem).Owner.Items["Stop"].Enabled = true;
                (sender as ToolStripItem).Enabled = false;
            });
            startToolStripItem.Name = "Start";
            startToolStripItem.Enabled = true;

            ToolStripItem stopToolStripItem = this.TrayIcon.ContextMenuStrip.Items.Add("Остановить сервер", null, (object sender, EventArgs e) =>
            {
                this.Server.Stop();

                (sender as ToolStripItem).Owner.Items["Start"].Enabled = true;
                (sender as ToolStripItem).Enabled = false;
            });
            stopToolStripItem.Name = "Stop";
            stopToolStripItem.Enabled = false;

            ToolStripItem exitToolStripItem = this.TrayIcon.ContextMenuStrip.Items.Add("Выйти", null, (object sender, EventArgs e) =>
            {
                Application.Exit();
            });
            exitToolStripItem.Name = "Exit";
            exitToolStripItem.Enabled = true;

            this.MainForm = new MainForm();
            this.MainForm.Visible = false;

            //this.server.Start(args);
        }

        private void OnApplicationExit(object sender, EventArgs e)
        {
            this.TrayIcon.Visible = false;
            this.TrayIcon.Dispose();

            this.Server.Stop();

            Log.Info("Выход из приложения");
        }

        private void TrayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:

                    this.MainForm.Visible = !this.MainForm.Visible;
                    if (this.MainForm.Visible) this.MainForm.Activate();

                    //Console.WriteLine(this.MainForm.Visible);
                    //this.MainForm.WindowState = this.MainForm.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : FormWindowState.Minimized;
                    //NativeMethods.AnimateWindow(this.MainForm.Handle, 100, this.MainForm.WindowState == FormWindowState.Minimized ? NativeMethods.AnimateWindowFlags.AW_HOR_POSITIVE | NativeMethods.AnimateWindowFlags.AW_HIDE : NativeMethods.AnimateWindowFlags.AW_HOR_NEGATIVE | NativeMethods.AnimateWindowFlags.AW_ACTIVATE);
                    //this.MainForm.Visible = this.MainForm.WindowState == FormWindowState.Minimized ? false : true;
                    break;
                case MouseButtons.Right:
                    break;
            }
        }
    }
}
