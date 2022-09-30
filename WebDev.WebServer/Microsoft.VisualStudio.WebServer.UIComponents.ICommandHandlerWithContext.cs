namespace Microsoft.VisualStudio.WebServer.UIComponents
{
    using System;

    public interface ICommandHandlerWithContext
    {
        bool HandleCommand(Command command, object context);
        bool UpdateCommand(Command command, object context);
    }
}
