namespace Microsoft.VisualStudio.WebServer
{
    using Microsoft.VisualStudio.WebHost;
    using Microsoft.VisualStudio.WebServer.UIComponents;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;

    public sealed class WebServerApp
    {
        [STAThread]
        public static int Main(string[] args)
        {
            bool flag2;
            CommandLine line = new CommandLine(args);
            bool flag = line.Options["silent"] != null;
            if (!flag && line.ShowHelp)
            {
                ShowUsage();
                return 0;
            }
            string virtualPath = (string) line.Options["vpath"];
            if (virtualPath != null)
            {
                virtualPath = virtualPath.Trim();
            }
            if ((virtualPath == null) || (virtualPath.Length == 0))
            {
                virtualPath = "/";
            }
            else if (!virtualPath.StartsWith("/", StringComparison.Ordinal))
            {
                if (!flag)
                {
                    ShowUsage();
                }
                return -1;
            }
            string path = (string) line.Options["path"];
            if (path != null)
            {
                path = path.Trim();
            }
            if ((path == null) || (path.Length == 0))
            {
                if (!flag)
                {
                    ShowUsage();
                }
                return -1;
            }
            if (!Directory.Exists(path))
            {
                if (!flag)
                {
                    ShowMessage(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_DirNotExist", new object[] { path }));
                }
                return -2;
            }
            int port = 0;
            string s = (string) line.Options["port"];
            if (s != null)
            {
                s = s.Trim();
            }
            if ((s != null) && (s.Length != 0))
            {
                try
                {
                    port = int.Parse(s, CultureInfo.InvariantCulture);
                    if ((port >= 1) && (port <= 0xffff))
                    {
                        goto Label_016E;
                    }
                    if (!flag)
                    {
                        ShowUsage();
                    }
                    return -1;
                }
                catch
                {
                    if (!flag)
                    {
                        ShowMessage(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_InvalidPort", new object[] { s }));
                    }
                    return -3;
                }
            }
            port = 80;
        Label_016E:
            flag2 = line.Options["ntlm"] != null;
            try
            {
                Server server = new Server(port, virtualPath, path, flag2);
                server.Start();
                Application.Run(new WebServerForm(server));
            }
            catch (Exception exception)
            {
                if (!flag)
                {
                    ShowMessage(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_ErrorListeningPort", new object[] { port, exception.Message }));
                }
                return -4;
            }
            catch
            {
                return -4;
            }
            return 0;
        }

        private static void ShowMessage(string msg)
        {
            if (Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                MessageBox.Show(msg, Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), MessageBoxButtons.OK, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
            else
            {
                MessageBox.Show(msg, Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private static void ShowMessage(string msg, MessageBoxIcon icon)
        {
            if (Thread.CurrentThread.CurrentUICulture.TextInfo.IsRightToLeft)
            {
                MessageBox.Show(msg, Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), MessageBoxButtons.OK, icon, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
            else
            {
                MessageBox.Show(msg, Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Name"), MessageBoxButtons.OK, icon);
            }
        }

        private static void ShowUsage()
        {
            ShowMessage(Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr1") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr2") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr3") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr4") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr5") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr6") + Microsoft.VisualStudio.WebServer.SR.GetString("WEBDEV_Usagestr7"), MessageBoxIcon.Asterisk);
        }
    }
}
