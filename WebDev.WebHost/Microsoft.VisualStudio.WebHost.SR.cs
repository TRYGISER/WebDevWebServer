namespace Microsoft.VisualStudio.WebHost
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    internal sealed class SR
    {
        private static Microsoft.VisualStudio.WebHost.SR loader;
        private ResourceManager resources;
        internal const string WEBDEV_DirListing = "WEBDEV_DirListing";
        internal const string WEBDEV_HTTPError = "WEBDEV_HTTPError";
        internal const string WEBDEV_ServerError = "WEBDEV_ServerError";
        internal const string WEBDEV_VersionInfo = "WEBDEV_VersionInfo";
        internal const string WEBDEV_VWDName = "WEBDEV_VWDName";

        internal SR()
        {
            this.resources = new ResourceManager("WebHost", base.GetType().Assembly);
        }

        private static Microsoft.VisualStudio.WebHost.SR GetLoader()
        {
            if (loader == null)
            {
                Microsoft.VisualStudio.WebHost.SR sr = new Microsoft.VisualStudio.WebHost.SR();
                Interlocked.CompareExchange<Microsoft.VisualStudio.WebHost.SR>(ref loader, sr, null);
            }
            return loader;
        }

        public static object GetObject(string name)
        {
            Microsoft.VisualStudio.WebHost.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetObject(name, Culture);
        }

        public static string GetString(string name)
        {
            Microsoft.VisualStudio.WebHost.SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, Culture);
        }

        public static string GetString(string name, params object[] args)
        {
            Microsoft.VisualStudio.WebHost.SR loader = GetLoader();
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

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }
    }
}
