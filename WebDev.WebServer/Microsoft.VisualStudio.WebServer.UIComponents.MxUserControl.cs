namespace Microsoft.VisualStudio.WebServer.UIComponents
{
    using System;
    using System.Windows.Forms;

    public class MxUserControl : UserControl
    {
        private IServiceProvider _serviceProvider;

        public MxUserControl()
        {
        }

        public MxUserControl(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }
            this._serviceProvider = serviceProvider;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._serviceProvider = null;
            }
            base.Dispose(disposing);
        }

        protected override object GetService(System.Type serviceType)
        {
            if (this._serviceProvider != null)
            {
                return this._serviceProvider.GetService(serviceType);
            }
            return null;
        }

        protected IServiceProvider ServiceProvider
        {
            get
            {
                return this._serviceProvider;
            }
        }
    }
}
