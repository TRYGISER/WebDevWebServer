namespace Microsoft.VisualStudio.WebServer.UIComponents
{
    using System;

    public interface ICommandHandler
    {
        bool HandleCommand(Command command);
        bool UpdateCommand(Command command);
    }
}
