// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

using TCPLib.Server.Net;

namespace TCPLib.Server.Commands;

internal class Ban : ICommand
{
    public string[] Synonyms { get; }

    public string Name { get; }

    public string Description { get; }

    public Task<bool> Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error("Not enough arguments to know more write: ? ban");
            return Task.FromResult(false);
        }
        string ip = "";
        NetClient client = null;
        if (args[0].IndexOf('.') == -1)
        {
            var clientlist = Client.clients.Where(x => x.id.ToString() == args[0]);
            if (clientlist.Count() == 0)
            {
                Console.Error($"The user with id {args[0]} was not found.");
                return Task.FromResult(false);
            }
            ip = clientlist.First().client.Client.RemoteEndPoint.ToString().Split(':')[0];
            client = clientlist.First();
        }
        else
        {
            if (args[0].IndexOf(':') == -1)
                ip = args[0];
            else
                ip = args[0].Split(':')[0];
            foreach (var s in Client.clients)
                if (s.client.Client.RemoteEndPoint.ToString().Split(':')[0] == ip)
                    client = s;
        }
        var list = SaveFiles.Ban.Load().ToList();
        foreach (var s in list)
        {
            if (s.IP == ip)
            {
                Console.Error($"The user with ip {ip} is already blocked.");
            }
        }
        if (args.Length > 1)
        {
            string reason = "";
            for (int i = 1; i < args.Length; i++)
            {
                reason += args[i] + " ";
            }
            reason = reason.TrimEnd(' ');
            reason = reason.TrimStart(' ');
            var ban = SaveFiles.Ban.CreateBan(ip, reason);
            list.Add(ban);
            SaveFiles.Ban.Save(list.ToArray());
            if (client is not null)
                client.Kick(new KickMessage(ResponseCode.Blocked, reason));
            Console.Info($"{ip} has been blocked with reason: {reason}");
        }
        else
        {
            var ban = SaveFiles.Ban.CreateBan(ip);
            list.Add(ban);
            SaveFiles.Ban.Save(list.ToArray());
            if (client is not null)
                client.Kick(new KickMessage(ResponseCode.Blocked));
            Console.Info($"{ip} has been blocked");
        }
        return Task.FromResult(true);
    }
    public Ban()
    {
        Synonyms = ["block", "ban"];
        Name = "ban";
        Description = "Blocks a user by ip or id. Usage: ban {ip/id} {reason}";
    }
}