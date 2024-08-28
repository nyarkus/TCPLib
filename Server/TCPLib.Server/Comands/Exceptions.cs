namespace TCPLib.Server.Commands
{
    public class CommandAlreadyExists : Exception
    {
        public CommandAlreadyExists(string name) : base($"A command with the name {name} already exists!") { }
    }

}
