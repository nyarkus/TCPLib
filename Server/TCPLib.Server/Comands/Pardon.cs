namespace TCPLib.Server.Commands
{
    internal class Pardon : ICommand
    {
        public string[] Synonyms { get; }

        public string Name { get; }

        public string Description { get; }

        public Task<bool> Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.Error("Not enough arguments to know more write: ? pardon");
                return Task.FromResult(false);
            }
            string ip = "";
            if (args[0].IndexOf(':') == -1)
                ip = args[0];
            else
                ip = args[0].Split(':')[0];

            var list = SaveFiles.Ban.Load().ToList();

            var newlist = new List<SaveFiles.Ban>();

            bool unbanned = false;
            foreach (var s in list)
            {
                if (s.IP == ip)
                    unbanned = true;
                else
                    newlist.Add(s);
            }
            SaveFiles.Ban.Save(newlist.ToArray());
            if (unbanned)
            {
                Console.Info($"{ip} has been unblocked");
                return Task.FromResult(true);
            }
            else
            {
                Console.Error($"failed to unblock {ip}");
                return Task.FromResult(false);
            }

        }
        public Pardon()
        {
            Synonyms = new string[] { "pardon", "unban" };
            Name = "pardon";
            Description = "Unblocks the user using his ip. Usage: pardon {ip}";
        }
    }
}