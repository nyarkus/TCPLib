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
            if (args.Length == 1)
            {
                return ExecuteWithSingleArgument(args[0]);
            }

            ExecuteWithoutArguments();
            return Task.FromResult(true);
        }

        private Task<bool> ExecuteWithSingleArgument(string arg)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var command in CommandManager.commands)
            {
                bool found = CheckCommandSynonyms(command, arg, sb);
                if (found || command.Name == arg)
                {
                    sb.AppendLine($" - {command.Description}");
                    Console.Info(sb.ToString());
                    return Task.FromResult(true);
                }
                sb.Clear();
            }

            return Task.FromResult(false);
        }

        private bool CheckCommandSynonyms(ICommand command, string arg, StringBuilder sb)
        {
            bool found = false;

            foreach (var syn in command.Synonyms)
            {
                if (syn == arg)
                {
                    found = true;
                }
                sb.Append($"{syn}, ");
            }

            if (sb.Length > 2)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return found;
        }

        private void ExecuteWithoutArguments()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var command in CommandManager.commands)
            {
                foreach (var syn in command.Synonyms)
                {
                    sb.Append($"{syn}, ");
                }

                if (sb.Length > 2)
                {
                    sb.Remove(sb.Length - 2, 2);
                }

                sb.AppendLine($" - {command.Description}");
            }

            Console.Info(sb.ToString());
        }

        public Help()
        {
            Synonyms = new [] { "help", "?" };
            Name = "help";
            Description = "Displays a list of commands and their descriptions. Usage: help {commandName} ";
        }
    }
}
