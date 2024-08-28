namespace TCPLib.Server.Commands;
public interface ICommand
{
    public Task<bool> Execute(string[] args);
    public string[] Synonyms { get; }
    public string Name { get; }
    public string Description { get; }
}