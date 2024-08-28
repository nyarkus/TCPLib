using System;
using System.Collections.Generic;
using System.Linq;
namespace TCPLib.Server.Commands
{
    public class CommandManager
    {
        public static List<ICommand> commands { get; private set; } = new List<ICommand>();
        public static void RegCommand(ICommand command)
        {
            try
            {
                foreach (var cmd in commands)
                {
                    foreach (var syn in cmd.Synonyms)
                        foreach (var s in command.Synonyms)
                            if (syn == s) throw new CommandAlreadyExists(s);
                    if (cmd.Name == command.Name)
                        throw new CommandAlreadyExists(command.Name);
                }
                commands.Add(command);
                Console.Debug($"The new \"{command.Name}\" command has been registered");
            }
            catch (Exception ex)
            {
                Console.Error($"Failed to register the {command.Name}\" command due to an unhandled exception");
                Console.Debug(ex);
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
                        for (int i = 1; i < splited.Length; i++) arg.Add(splited[i]);
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
