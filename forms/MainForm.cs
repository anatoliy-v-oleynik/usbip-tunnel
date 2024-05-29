using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using usbip_tunnel.net;

namespace usbip_tunnel.forms
{
    public partial class MainForm : Form
    {
        public ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }

             
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Log.Debug("MainForm_Shown");

            this.Left = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            this.Top = Screen.PrimaryScreen.WorkingArea.Top;
            this.Height = Screen.PrimaryScreen.WorkingArea.Height;

            if (!this.attachButton.Checked) Hide();

            if (this.connectButton.Checked) Program.Context.Server.Start(null);
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (!this.attachButton.Checked) Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Log.Debug("MainForm_Load");

            this.connectionsListView.Groups.Add("undefined", "Ожидающие клиенты");

            Program.Context.Server.Started += Server_Started;
            Program.Context.Server.Stopped += Server_Stopped;

            Program.Context.Server.ClientConnected += Server_ClientConnected;
            Program.Context.Server.ClientDisconnected += Server_ClientDisconnected;
            Program.Context.Server.ClientJoined += Server_ClientJoined;

            this.attachButton.Image = this.attachButton.Checked ? Properties.Resources.icons8_attached_16 : Properties.Resources.icons8_attached_16_1;
            this.connectButton.Image = this.connectButton.Checked ? Properties.Resources.icons8_connect_32 : Properties.Resources.icons8_disconnect_32;


        }

        private void Server_Stopped(object sender, EventArgs e)
        {
            this.connectButton.Checked = false;
        }

        private void Server_Started(object sender, EventArgs e)
        {
            this.connectButton.Checked = true;
        }

        private void Server_ClientJoined(object sender, ClientEventArgs e)
        {
            Log.DebugFormat("Server_ClientJoined - {0} - {1}", e.Client.ToString(), e.Client.Tunnel.ToString());

            this.Invoke(new MethodInvoker(() =>
            {
                ListViewGroup group = this.connectionsListView.Groups[e.Client.Tunnel.BusNum];
                if (group == null) group = this.connectionsListView.Groups.Add(e.Client.Tunnel.BusNum, e.Client.Tunnel.BusNum);

                ListViewItem item = this.connectionsListView.Items[e.Client.Socket.RemoteEndPoint.ToString()];
                item.Group = group;
            }));

        }

        private void Server_ClientDisconnected(object sender, ClientEventArgs e)
        {
            Log.DebugFormat("Server_ClientDisconnected - {0}", e.Client.ToString());


            this.Invoke(new MethodInvoker(() =>
            {
                this.connectionsListView.Items.RemoveByKey(e.Client.Socket.RemoteEndPoint.ToString());
            }));
        }

        private void Server_ClientConnected(object sender, ClientEventArgs e)
        {
            Log.DebugFormat("Server_ClientConnected - {0}", e.Client.ToString());

            this.Invoke(new MethodInvoker(() =>
            {
                ListViewItem item = this.connectionsListView.Items.Add(e.Client.Socket.RemoteEndPoint.ToString(), e.Client.Socket.RemoteEndPoint.ToString(), 0);
                item.Group = this.connectionsListView.Groups["undefined"];
            }));

            Log.DebugFormat("End Server_ClientConnected - {0}", e.Client.ToString());
        }

        private void closeButton_MouseEnter(object sender, EventArgs e)
        {
            this.closeButton.Image = Properties.Resources.icons8_close_over_25;
        }

        private void closeButton_MouseLeave(object sender, EventArgs e)
        {
            this.closeButton.Image = Properties.Resources.icons8_close_25;
        }

        private void attachButton_CheckedChanged(object sender, EventArgs e)
        {
            this.attachButton.Image = this.attachButton.Checked ? Properties.Resources.icons8_attached_16 : Properties.Resources.icons8_attached_16_1;
        }

        private void connectButton_CheckedChanged(object sender, EventArgs e)
        {
            this.connectButton.Image = this.connectButton.Checked ? Properties.Resources.icons8_connect_32 : Properties.Resources.icons8_disconnect_32;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (this.connectButton.Checked)
            {
                Program.Context.Server.Stop();
            }
            else
            {
                Program.Context.Server.Start(null);
            }

            Log.DebugFormat(" connectButton_Click - ChackState:{0}", this.connectButton.Checked);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Log.DebugFormat("MainForm_FormClosed");

            Properties.Settings.Default.Save();
        }
    }
}
