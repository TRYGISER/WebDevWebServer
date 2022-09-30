namespace Microsoft.VisualStudio.WebHost
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;
    using System.Web.Hosting;
    using System.IO;
    using System.Windows.Forms;

    [PermissionSet(SecurityAction.LinkDemand, Name="Everything"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class Server : MarshalByRefObject
    {
        private ApplicationManager _appManager;
        private Host _host;
        private object _lockObject;
        private WaitCallback _onSocketAccept;
        private WaitCallback _onStart;
        private string _physicalPath;
        private int _port;
        private IntPtr _processToken;
        private string _processUser;
        private bool _requireAuthentication;
        private bool _shutdownInProgress;
        private Socket _socket;
        private string _virtualPath;
        private const int SecurityImpersonation = 2;
        private const int TOKEN_ALL_ACCESS = 0xf01ff;
        private const int TOKEN_EXECUTE = 0x20000;
        private const int TOKEN_IMPERSONATE = 4;
        private const int TOKEN_READ = 0x20008;

        public Server(int port, string virtualPath, string physicalPath) : this(port, virtualPath, physicalPath, false)
        {
        }

        public Server(int port, string virtualPath, string physicalPath, bool requireAuthentication)
        {
            this._lockObject = new object();
            this._port = port;
            this._virtualPath = virtualPath;
            this._physicalPath = physicalPath.EndsWith(@"\", StringComparison.Ordinal) ? physicalPath : (physicalPath + @"\");
            this._requireAuthentication = requireAuthentication;
            this._onSocketAccept = new WaitCallback(this.OnSocketAccept);
            this._onStart = new WaitCallback(this.OnStart);
            this._appManager = ApplicationManager.GetApplicationManager();
            this.ObtainProcessToken();
        }

        [DllImport("KERNEL32.DLL", SetLastError=true)]
        private static extern IntPtr GetCurrentThread();
        private Host GetHost()
        {
            if (this._shutdownInProgress)
            {
                return null;
            }
            Host host = this._host;
            if (host == null)
            {
                lock (this._lockObject)
                {
                    host = this._host;
                    if (host == null)
                    {
                        //初始化目标路径中的WebDev.WebHost
                        InitHost();
                        string appId = (this._virtualPath + this._physicalPath).ToLowerInvariant().GetHashCode().ToString("x", CultureInfo.InvariantCulture);
                        this._host = (Host) this._appManager.CreateObject(appId, typeof(Host), this._virtualPath, this._physicalPath, false);
                        this._host.Configure(this, this._port, this._virtualPath, this._physicalPath, this._requireAuthentication);
                        host = this._host;
                    }
                }
            }
            return host;
        }
        void InitHost()
        {            
            string path = System.IO.Path.Combine(_physicalPath, "bin");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string srcfileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string destfileName = System.IO.Path.Combine(path, Path.GetFileName(srcfileName));
            if (!File.Exists(destfileName))
            {                
                File.Copy(srcfileName, destfileName);
            }
        }
        public IntPtr GetProcessToken()
        {
            return this._processToken;
        }

        public string GetProcessUser()
        {
            return this._processUser;
        }

        internal void HostStopped()
        {
            this._host = null;
        }

        [DllImport("ADVAPI32.DLL", SetLastError=true)]
        private static extern bool ImpersonateSelf(int level);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private void ObtainProcessToken()
        {
            if (ImpersonateSelf(2))
            {
                OpenThreadToken(GetCurrentThread(), 0xf01ff, true, ref this._processToken);
                RevertToSelf();
                this._processUser = WindowsIdentity.GetCurrent().Name;
            }
        }

        private void OnSocketAccept(object acceptedSocket)
        {
            if (!this._shutdownInProgress)
            {   
                Microsoft.VisualStudio.WebHost.Connection conn = new Microsoft.VisualStudio.WebHost.Connection(this, (Socket) acceptedSocket);
                if (conn.WaitForRequestBytes() == 0)
                {
                    conn.WriteErrorAndClose(400);
                }
                else
                {
                    Host host = this.GetHost();
                    if (host == null)
                    {
                        conn.WriteErrorAndClose(500);
                    }
                    else
                    {
                        host.ProcessRequest(conn);
                    }
                }
            }
        }

        private void OnStart(object unused)
        {
            while (!this._shutdownInProgress)
            {
                try
                {   
                    Socket state = this._socket.Accept();
                    //OnSocketAccept(state);
                    ThreadPool.QueueUserWorkItem(this._onSocketAccept, state);
                    continue;
                }
                catch
                {
                    Thread.Sleep(100);
                    continue;
                }
            }
        }

        [DllImport("ADVAPI32.DLL", SetLastError=true)]
        private static extern int OpenThreadToken(IntPtr thread, int access, bool openAsSelf, ref IntPtr hToken);
        [DllImport("ADVAPI32.DLL", SetLastError=true)]
        private static extern int RevertToSelf();
        public void Start()
        {
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.ExclusiveAddressUse = true;
            try
            {
                this._socket.Bind(new IPEndPoint(IPAddress.Any, this._port));
            }
            catch
            {
                this._socket.ExclusiveAddressUse = false;
                this._socket.Bind(new IPEndPoint(IPAddress.Any, this._port));
            }
            this._socket.Listen(0x7ffff);
            ThreadPool.QueueUserWorkItem(this._onStart);
        }

        public void Stop()
        {
            this._shutdownInProgress = true;
            try
            {
                if (this._socket != null)
                {
                    this._socket.Close();
                }
            }
            catch
            {
            }
            finally
            {
                this._socket = null;
            }
            try
            {
                if (this._host != null)
                {
                    this._host.Shutdown();
                }
                while (this._host != null)
                {
                    Thread.Sleep(100);
                }
            }
            catch
            {
            }
            finally
            {
                this._host = null;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return this._physicalPath;
            }
        }

        public int Port
        {
            get
            {
                return this._port;
            }
        }

        public string RootUrl
        {
            get
            {
                if (this._port != 80)
                {
                    return ("http://localhost:" + this._port + this._virtualPath);
                }
                return ("http://localhost" + this._virtualPath);
            }
        }

        public string VirtualPath
        {
            get
            {
                return this._virtualPath;
            }
        }
    }
}
