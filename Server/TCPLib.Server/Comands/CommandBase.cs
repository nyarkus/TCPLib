using System.Threading.Tasks;

namespace TCPLib.Server.Commands
{
    public interface ICommand
    {
        Task<bool> Execute(string[] args);
        string[] Synonyms { get; }
        string Name { get; }
        string Description { get; }
    }
}