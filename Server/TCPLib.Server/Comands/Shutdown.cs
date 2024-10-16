using System.Threading.Tasks;

namespace TCPLib.Server.Commands
{
    internal class Shutdown : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }
        internal Server server { get; }

        public Task<bool> Execute(string[] args)
        {
            server.Stop();
            return Task.FromResult(true);
        }
        public Shutdown(Server server)
        {
            Synonyms = new [] { "shutdown", "stop" };
            Name = "shutdown";
            Description = "Shuts down the server";
            this.server = server;
        }
    }
}
