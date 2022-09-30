namespace Microsoft.VisualStudio.WebServer
{
    using Microsoft.VisualStudio.WebHost;
    using Microsoft.VisualStudio.WebServer.UIComponents;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;

    internal class WebServerForm : TaskForm
    {
        private MxLabel _appRootLabel;
        private MxTextBox _appRootTextBox;
        private bool _exitMode;
        private MxLabel _hyperlinkLabel;
        private LinkLabel _hyperlinkLinkLabel;
        private MxLabel _physicalPathLabel;
        private MxTextBox _physicalPathTextBox;
        private MxLabel _portLabel;
        private MxTextBox _portTextBox;
        private Server _server;
        private MxButton _stopButton;
        private TrayIcon _trayIcon;
        private MxContextMenu _trayMenu;
        private const int WM_QUERYENDSESSION = 0x11;

        public WebServerForm()
        {
            this.InitializeComponent();
        }

        public WebServerForm(Server server) : base(new ServiceContainer())
        {
            this.InitializeComponent();
            base.TaskCaption = Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name");
            base.TaskDescription = Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_RunAspNetLocally");
            this._hyperlinkLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(this.OnLinkClickedHyperlinkLinkLabel);
            this._stopButton.Click += new EventHandler(this.OnClickStopButton);
            this._trayIcon = new TrayIcon();
            this._trayIcon.Owner = this;
            this._trayIcon.DoubleClick += new EventHandler(this.OnDoubleClickTrayIcon);
            this._trayIcon.ShowContextMenu += new ShowContextMenuEventHandler(this.OnTrayIconShowContextMenu);
            base.Icon = new Icon(base.GetType(), "WebServerForm.ico");
            base.TaskGlyph = new Bitmap(typeof(WebServerForm), "WebServerForm.bmp");
            this._server = server;
            this._physicalPathTextBox.Text = this._server.PhysicalPath;
            this._appRootTextBox.Text = this._server.VirtualPath;
            this._portTextBox.Text = this._server.Port.ToString(CultureInfo.InvariantCulture);
            this._hyperlinkLinkLabel.Text = this._server.RootUrl;
            this.Text = Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_NameWithPort", new object[] { this._server.Port });
            this._trayIcon.Icon = new Icon(typeof(WebServerForm), "WebServerForm.ico");
            this._trayIcon.Text = this.Text;
            this._trayIcon.Visible = true;
            this._trayIcon.ShowBalloon(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), this._server.RootUrl, 15);
        }

        private void DoLaunch()
        {
            string rootUrl = this._server.RootUrl;
            if (!rootUrl.EndsWith("/", StringComparison.Ordinal))
            {
                rootUrl = rootUrl + "/";
            }
            Process.Start(rootUrl);
        }

        private void DoShow()
        {
            base.Show();
            base.Focus();
            base.WindowState = FormWindowState.Normal;
        }

        private void DoStop()
        {
            this._exitMode = true;
            base.Close();
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(WebServerForm));
            this._appRootTextBox = new MxTextBox();
            this._portTextBox = new MxTextBox();
            this._portLabel = new MxLabel();
            this._hyperlinkLabel = new MxLabel();
            this._hyperlinkLinkLabel = new LinkLabel();
            this._physicalPathLabel = new MxLabel();
            this._appRootLabel = new MxLabel();
            this._stopButton = new MxButton();
            this._physicalPathTextBox = new MxTextBox();
            base.SuspendLayout();
            manager.ApplyResources(this._appRootTextBox, "_appRootTextBox");
            this._appRootTextBox.AlwaysShowFocusCues = true;
            this._appRootTextBox.FlatAppearance = true;
            this._appRootTextBox.Name = "_appRootTextBox";
            this._appRootTextBox.ReadOnly = true;
            this._appRootTextBox.TabStop = false;
            manager.ApplyResources(this._portTextBox, "_portTextBox");
            this._portTextBox.AlwaysShowFocusCues = true;
            this._portTextBox.FlatAppearance = true;
            this._portTextBox.Name = "_portTextBox";
            this._portTextBox.ReadOnly = true;
            this._portTextBox.TabStop = false;
            manager.ApplyResources(this._portLabel, "_portLabel");
            this._portLabel.FlatStyle = FlatStyle.System;
            this._portLabel.Name = "_portLabel";
            manager.ApplyResources(this._hyperlinkLabel, "_hyperlinkLabel");
            this._hyperlinkLabel.FlatStyle = FlatStyle.System;
            this._hyperlinkLabel.Name = "_hyperlinkLabel";
            manager.ApplyResources(this._hyperlinkLinkLabel, "_hyperlinkLinkLabel");
            this._hyperlinkLinkLabel.Name = "_hyperlinkLinkLabel";
            manager.ApplyResources(this._physicalPathLabel, "_physicalPathLabel");
            this._physicalPathLabel.FlatStyle = FlatStyle.System;
            this._physicalPathLabel.Name = "_physicalPathLabel";
            manager.ApplyResources(this._appRootLabel, "_appRootLabel");
            this._appRootLabel.FlatStyle = FlatStyle.System;
            this._appRootLabel.Name = "_appRootLabel";
            manager.ApplyResources(this._stopButton, "_stopButton");
            this._stopButton.Name = "_stopButton";
            manager.ApplyResources(this._physicalPathTextBox, "_physicalPathTextBox");
            this._physicalPathTextBox.AlwaysShowFocusCues = true;
            this._physicalPathTextBox.FlatAppearance = true;
            this._physicalPathTextBox.Name = "_physicalPathTextBox";
            this._physicalPathTextBox.ReadOnly = true;
            this._physicalPathTextBox.TabStop = false;
            manager.ApplyResources(this, "$this");
            base.Controls.Add(this._stopButton);
            base.Controls.Add(this._hyperlinkLinkLabel);
            base.Controls.Add(this._portTextBox);
            base.Controls.Add(this._appRootTextBox);
            base.Controls.Add(this._physicalPathTextBox);
            base.Controls.Add(this._hyperlinkLabel);
            base.Controls.Add(this._portLabel);
            base.Controls.Add(this._appRootLabel);
            base.Controls.Add(this._physicalPathLabel);
            base.FormBorderStyle = FormBorderStyle.FixedDialog;
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "WebServerForm";
            base.ShowInTaskbar = false;
            base.TaskBorderStyle = BorderStyle.FixedSingle;
            base.TaskCaption = "ASP.NET Development Server";
            base.TaskDescription = "Run ASP.NET applications locally.";
            base.WindowState = FormWindowState.Minimized;
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        private void OnClickStopButton(object sender, EventArgs e)
        {
            this.DoStop();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!this._exitMode)
            {
                e.Cancel = true;
                base.WindowState = FormWindowState.Minimized;
                base.Hide();
                this._trayIcon.ShowBalloon(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), this._server.RootUrl, 10);
            }
            else
            {
                base.OnClosing(e);
                if (!e.Cancel)
                {
                    this._trayIcon.Dispose();
                    this._server.Stop();
                }
            }
        }

        private void OnCommandLaunch(object sender, EventArgs e)
        {
            this.DoLaunch();
        }

        private void OnCommandShow(object sender, EventArgs e)
        {
            this.DoShow();
        }

        private void OnCommandStop(object sender, EventArgs e)
        {
            this.DoStop();
        }

        private void OnDoubleClickTrayIcon(object sender, EventArgs e)
        {
            this.DoShow();
        }

        private void OnLinkClickedHyperlinkLinkLabel(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.DoLaunch();
        }

        private void OnTrayIconShowContextMenu(object sender, ShowContextMenuEventArgs e)
        {
            if (this._trayMenu == null)
            {
                //MxMenuItem item = new MxMenuItem(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_ShowDetail"), string.Empty, new EventHandler(this.OnCommandShow));
                //MxMenuItem item2 = new MxMenuItem(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Stop"), string.Empty, new EventHandler(this.OnCommandStop));
                //MxMenuItem item3 = new MxMenuItem(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_OpenInBrowser"), string.Empty, new EventHandler(this.OnCommandLaunch));


                MxMenuItem item = new MxMenuItem("œÍœ∏", string.Empty, new EventHandler(this.OnCommandShow));
                MxMenuItem item2 = new MxMenuItem("Õ£÷π", string.Empty, new EventHandler(this.OnCommandStop));
                MxMenuItem item3 = new MxMenuItem("‰Ø¿¿", string.Empty, new EventHandler(this.OnCommandLaunch));


                this._trayMenu = new MxContextMenu(new MenuItem[] { item3, new MxMenuItem("-"), item2, new MxMenuItem("-"), item });
                if (this.RightToLeftLayout)
                {
                    this._trayMenu.RightToLeft = RightToLeft.Yes;
                }
            }
            this._trayMenu.Show(this, e.Location.X, e.Location.Y);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x11)
            {
                this._exitMode = true;
            }
            base.WndProc(ref m);
        }
    }
}
