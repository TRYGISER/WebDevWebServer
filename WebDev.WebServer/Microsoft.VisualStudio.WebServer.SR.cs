namespace Microsoft.VisualStudio.WebServer
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        private static Microsoft.VisualStudio.WebServer.SR loader;
        private ResourceManager resources;
        private static object s_InternalSyncObject;
        internal const string WEBDEV_DirNotExist = "WEBDEV_DirNotExist";
        internal const string WEBDEV_ErrorListeningPort = "WEBDEV_ErrorListeningPort";
        internal const string WEBDEV_InvalidPort = "WEBDEV_InvalidPort";
        internal const string WEBDEV_Name = "WEBDEV_Name";
        internal const string WEBDEV_NameWithPort = "WEBDEV_NameWithPort";
        internal const string WEBDEV_OpenInBrowser = "WEBDEV_OpenInBrowser";
        internal const string WEBDEV_RunAspNetLocally = "WEBDEV_RunAspNetLocally";
        internal const string WEBDEV_ShowDetail = "WEBDEV_ShowDetail";
        internal const string WEBDEV_Stop = "WEBDEV_Stop";
        internal const string WEBDEV_Usagestr1 = "WEBDEV_Usagestr1";
        internal const string WEBDEV_Usagestr2 = "WEBDEV_Usagestr2";
        internal const string WEBDEV_Usagestr3 = "WEBDEV_Usagestr3";
        internal const string WEBDEV_Usagestr4 = "WEBDEV_Usagestr4";
        internal const string WEBDEV_Usagestr5 = "WEBDEV_Usagestr5";
        internal const string WEBDEV_Usagestr6 = "WEBDEV_Usagestr6";
        internal const string WEBDEV_Usagestr7 = "WEBDEV_Usagestr7";

        internal SR()
        {
            this.resources = new ResourceManager("WebDevServer", base.GetType().Assembly);
        }

        private static Microsoft.VisualStudio.WebServer.SR GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new Microsoft.VisualStudio.WebServer.SR();
                    }
                }
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            Microsoft.VisualStudio.WebServer.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            Microsoft.VisualStudio.WebServer.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            Microsoft.VisualStudio.WebServer.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, Culture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static CultureInfo Culture
        {
            get
            {
                return null;
            }
        }

        private static object InternalSyncObject
        {
            get
            {
                if (s_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, obj2, null);
                }
                return s_InternalSyncObject;
            }
        }

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }
    }
}
