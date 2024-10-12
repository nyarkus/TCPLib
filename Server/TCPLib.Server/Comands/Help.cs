using System.Text;
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
            StringBuilder sb = new StringBuilder();
            if (args.Length == 1)
            {
                foreach (var s in CommandManager.commands)
                {
                    bool x = false;
                    foreach (var syn in s.Synonyms)
                    {
                        if (syn == args[0]) x = true;
                        {
                            sb.Append($"{syn}, ");
                        }
                    }
                    sb.Remove(sb.Length - 2, 2);
                    if (s.Name == args[0]) x = true;
                    {
                        sb.AppendLine($" - {s.Description}");
                    }
                    if (x)
                    {
                        Console.Info(sb.ToString());
                        return Task.FromResult(true);
                    }
                    sb.Clear();
                }
            }
            foreach (var s in CommandManager.commands)
            {
                foreach (var syn in s.Synonyms)
                    sb.Append($"{syn}, ");
                sb.Remove(sb.Length - 2, 2);
                sb.AppendLine($" - {s.Description}");
            }
            Console.Info(sb.ToString());
            return Task.FromResult(true);
        }
        public Help()
        {
            Synonyms = new string[] { "help", "?" };
            Name = "help";
            Description = "Displays a list of commands and their descriptions. Usage: help {commandName} ";
        }
    }
}
