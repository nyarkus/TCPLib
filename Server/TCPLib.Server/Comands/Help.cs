using System.Threading.Tasks;

namespace TCPLib.Server.Commands
{
    internal class Help : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }

        public Task<bool> Execute(string[] args)
        {
            string output = "";
            if (args.Length == 1)
            {
                foreach (var s in CommandManager.commands)
                {
                    bool x = false;
                    foreach (var syn in s.Synonyms)
                    {
                        if (syn == args[0]) x = true;
                        output += $"{syn}, ";
                    }
                    output = output.TrimEnd(' ');
                    output = output.TrimEnd(',');
                    output += " ";
                    if (s.Name == args[0]) x = true;
                    output += $"- {s.Description}\n";
                    if (x)
                    {
                        Console.Info(output);
                        return Task.FromResult(true);
                    }
                    output = "";
                }
            }
            foreach (var s in CommandManager.commands)
            {
                foreach (var syn in s.Synonyms)
                    output += $"{syn}, ";
                output = output.TrimEnd(' ');
                output = output.TrimEnd(',');
                output += " ";
                output += $"- {s.Description}\n";
            }
            Console.Info(output);
            return Task.FromResult(true);
        }
        public Help()
        {
            Synonyms = new string[] { "help", "?" };
            Name = "help";
            Description = "Displays a list of commands and their descriptions";
        }
    }
}
