# WebDevWebServer

自从Vs2005起，Vs开发环境便自带了WebDev.WebServer，就是这个图标

它实际上是一个小型的Web服务器，专用于.net平台。大家经常调试程序它还是相当的方便，经过小小的配置就可以指向某个文件夹，使该文件夹成为网站目录已供访问。
WebDev.WebServer确实很轻便，同时它本身是.net程序，才2个文件，作为测试和演示环境是非常的好用，在xp也能很好的运行，Xp的IIS5实在是不方便用。
但是自带的WebDev.WebServer只能用于本机，那是因为MS对其进行了限制，因为作出它的目的，本身是为了Vs的开发更方便而已，但是很多测试也是基于网络的，至少你不希望别人测试一些网站，总得来用你的电脑吧。
WebDev.WebServer一共是两个文件，一个是WebDev.WebServer.exe，另一个是WebDev.WebHost.dll，另外说一下WebDev.WebServer是安装开发环境才有的，不是安装.net framework里面的东东，所以你提取这两个文件都需要已经安装有开发环境下来进行。
WebDev.WebServer.exe在C:\WINDOWS\Microsoft.NET\Framework\v2.0.50727目录下，如果安装Vs2008可以在C:\Program Files\Common Files\Microsoft Shared\DevServer\9.0目录下找到，而WebDev.WebHost.dll则不在那里，WebDev.WebHost.dll在安装时候作为全局程序集已经被编译到C:\windows\Assembly\XXXX....下，根据你安装的vs2005还是vs2008相关，一共有两个，一个是vs2005的，一个是vs2008的，它们与WebDev.WebServer.exe互相对应，获得方法是来到命令行窗口，使用原始的命令copy 目标文件 c:\得到，这个DLL也是纯.net的。
获得这两个文件然后用reflector来反编译下，获得其源码，呵呵很方便吧，本身的源代码都不长。
然后经过简单加工下使其能够编译运行，又远程访问试下，不行，提示什么？提示拒绝访问，但是说明连接上了，跟踪下代码来到以下设置：
```
private bool TryParseRequest()
        {
            this.Reset();
            this.ReadAllHeaders();
            if (!this._connection.IsLocal)
            {
                this._connection.WriteErrorAndClose(0x193);
                return false;
            }
            if (((this._headerBytes == null) || (this._endHeadersOffset < 0)) || ((this._headerByteStrings == null) || (this._headerByteStrings.Count == 0)))
            {
                this._connection.WriteErrorAndClose(400);
                return false;
            }
            。。。。。。
}
```
看到this._connection.IsLocal这个判断，摆明是故意的...，注释掉重编译，运行，再访问，OK。
程序还有一个细节是WebDev.WebServer对执行站点目录采用Remoting远程代理方式来载入主机的。
```
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
```
这就需要在站点目录的bin目录下面放入WebDev.WebHost.dll，（因为开发环境下已经配置到了全局程序集所以不需要），InitHost()这个方法是我自己加的，用于自动copy WebDev.WebHost.dll到站点目录下。
项目源代码我已经发布了，用vs2008项目来编译的，可以在我的空间找到http://ocean.ys168.com找到，另外在使用的时候需要对WebDev.WebServer项目进行一下自己的配置。

命令行参数里面请指向你自己要测试的站点目录。
