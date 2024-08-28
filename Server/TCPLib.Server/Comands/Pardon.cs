// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Server.Commands;

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
        List<SaveFiles.Ban> newlist = new();
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
        Synonyms = ["pardon", "unban"];
        Name = "pardon";
        Description = "Unblocks the user using his ip. Usage: pardon {ip}";
    }
}