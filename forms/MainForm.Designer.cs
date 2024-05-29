namespace usbip_tunnel.forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.settingButton = new System.Windows.Forms.CheckBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.connectionsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectButton = new System.Windows.Forms.CheckBox();
            this.attachButton = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.settingButton);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Controls.Add(this.attachButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(301, 42);
            this.panel1.TabIndex = 0;
            // 
            // settingButton
            // 
            this.settingButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.settingButton.Checked = true;
            this.settingButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.settingButton.FlatAppearance.BorderSize = 0;
            this.settingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.settingButton.Image = global::usbip_tunnel.Properties.Resources.icons8_setting_16;
            this.settingButton.Location = new System.Drawing.Point(41, 9);
            this.settingButton.Margin = new System.Windows.Forms.Padding(0);
            this.settingButton.Name = "settingButton";
            this.settingButton.Size = new System.Drawing.Size(25, 25);
            this.settingButton.TabIndex = 3;
            this.settingButton.UseVisualStyleBackColor = true;
            // 
            // closeButton
            // 
            this.closeButton.FlatAppearance.BorderSize = 0;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.closeButton.Image = global::usbip_tunnel.Properties.Resources.icons8_close_25;
            this.closeButton.Location = new System.Drawing.Point(267, 9);
            this.closeButton.Margin = new System.Windows.Forms.Padding(0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(20, 20);
            this.closeButton.TabIndex = 1;
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            this.closeButton.MouseEnter += new System.EventHandler(this.closeButton_MouseEnter);
            this.closeButton.MouseLeave += new System.EventHandler(this.closeButton_MouseLeave);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.connectButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 518);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(301, 46);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.connectionsListView);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 42);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 476);
            this.panel3.TabIndex = 2;
            // 
            // connectionsListView
            // 
            this.connectionsListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.connectionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.connectionsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connectionsListView.FullRowSelect = true;
            this.connectionsListView.GridLines = true;
            this.connectionsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.connectionsListView.HideSelection = false;
            this.connectionsListView.Location = new System.Drawing.Point(0, 0);
            this.connectionsListView.MultiSelect = false;
            this.connectionsListView.Name = "connectionsListView";
            this.connectionsListView.Size = new System.Drawing.Size(301, 476);
            this.connectionsListView.TabIndex = 0;
            this.connectionsListView.UseCompatibleStateImageBehavior = false;
            this.connectionsListView.View = System.Windows.Forms.View.Tile;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 150;
            // 
            // connectButton
            // 
            this.connectButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.connectButton.AutoCheck = false;
            this.connectButton.Checked = global::usbip_tunnel.Properties.Settings.Default.ServerConnected;
            this.connectButton.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::usbip_tunnel.Properties.Settings.Default, "ServerConnected", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.connectButton.FlatAppearance.BorderSize = 0;
            this.connectButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.connectButton.Image = global::usbip_tunnel.Properties.Resources.icons8_disconnect_32;
            this.connectButton.Location = new System.Drawing.Point(8, 8);
            this.connectButton.Margin = new System.Windows.Forms.Padding(0);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(45, 30);
            this.connectButton.TabIndex = 4;
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.CheckedChanged += new System.EventHandler(this.connectButton_CheckedChanged);
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // attachButton
            // 
            this.attachButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.attachButton.Checked = global::usbip_tunnel.Properties.Settings.Default.FormAttached;
            this.attachButton.DataBindings.Add(new System.Windows.Forms.Binding("Checked", global::usbip_tunnel.Properties.Settings.Default, "FormAttached", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.attachButton.FlatAppearance.BorderSize = 0;
            this.attachButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.attachButton.Image = global::usbip_tunnel.Properties.Resources.icons8_attached_16;
            this.attachButton.Location = new System.Drawing.Point(9, 9);
            this.attachButton.Margin = new System.Windows.Forms.Padding(0);
            this.attachButton.Name = "attachButton";
            this.attachButton.Size = new System.Drawing.Size(25, 25);
            this.attachButton.TabIndex = 0;
            this.attachButton.UseVisualStyleBackColor = true;
            this.attachButton.CheckedChanged += new System.EventHandler(this.attachButton_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 564);
            this.ControlBox = false;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "USBIP-TUNNEL";
            this.TopMost = true;
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView connectionsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.CheckBox attachButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.CheckBox settingButton;
        private System.Windows.Forms.CheckBox connectButton;
    }
}