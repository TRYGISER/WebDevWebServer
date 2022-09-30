namespace Microsoft.VisualStudio.WebServer.UIComponents
{
    using System;

    public interface IStatusBar
    {
        void SetProgress(int percentComplete);
        void SetText(string text);
    }
}
