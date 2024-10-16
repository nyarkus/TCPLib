using System;
using System.Collections.Generic;
using System.Linq;
namespace TCPLib.Server.Commands
{
    public static class CommandManager
    {
        public static List<ICommand> commands { get; private set; } = new List<ICommand>();
        public static void RegCommand(ICommand command)
        {
            try
            {
                foreach (var cmd in commands)
                {
                    if (cmd == null)
                        continue;

                    checkForDuplicateSynonyms(cmd, command);
                    checkForDuplicateName(cmd, command);
                }

                commands.Add(command);
                Console.Debug($"The new \"{command.Name}\" command has been registered");
            }
            catch (Exception ex)
            {
                Console.Error($"Failed to register the \"{command.Name}\" command due to an unhandled exception");
                Console.Debug(ex);
            }
        }

        private static void checkForDuplicateSynonyms(ICommand cmd, ICommand command)
        {
            foreach (var syn in cmd.Synonyms)
            {
                if (command.Synonyms.Contains(syn))
                {
                    throw new CommandAlreadyExists(syn);
                }
            }
        }

        private static void checkForDuplicateName(ICommand cmd, ICommand command)
        {
            if (cmd.Name == command.Name)
            {
                throw new CommandAlreadyExists(command.Name);
            }
        }

        public static void HandleLine(string line)
        {
            if (line == null)
                return;

            while (line.StartsWith(" ")) line = line.TrimStart();
                if (line is null || line.Length == 0)
                {
                    Console.Error($"Command \"{line}\" was not found.");
                    return;
                }

            var splited = line.Split(' ');
            foreach (var cmd in commands)
            {
                if (cmd.Synonyms.Contains(splited[0]))
                {
                    string[] args = { };
                    if (splited.Length > 1)
                    {
                        var arg = new List<string>();
                        for (int i = 1; i < splited.Length; i++) 
                            arg.Add(splited[i]);

                        args = arg.ToArray();
                    }
                    cmd.Execute(args);
                    return;
                }
            }
            Console.Error($"Command \"{splited[0]}\" was not found.");
        }
        internal static void RegAllCommands(Server server)
        {
            RegCommand(new Help());
            RegCommand(new Shutdown(server));
            RegCommand(new Ban());
            RegCommand(new Pardon());
            RegCommand(new Kick());
        }
    }

}
