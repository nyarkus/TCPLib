// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

using TCPLib.Server.Net;

namespace TCPLib.Server.Commands;

internal class Kick : ICommand
{
    public string[] Synonyms { get; }

    public string Name { get; }

    public string Description { get; }

    public Task<bool> Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.Error("Not enough arguments to know more write: ? kick");
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
            var clientlist = Client.clients.Where(x => x.client.Client.RemoteEndPoint.ToString().Split(':')[0] == args[0]);
            if (clientlist.Count() == 0)
            {
                Console.Error($"The user with ip {ip} was not found.");
                return Task.FromResult(false);
            }
            client = clientlist.First();
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
            client.Kick(new KickMessage(ResponseCode.Kicked, reason));
            Console.Info($"{ip} has been kicked with reason: {reason}");
        }
        else
        {
            client.Kick(new KickMessage(ResponseCode.Kicked));
            Console.Info($"{ip} has been kicked");
        }
        return Task.FromResult(true);
    }
    public Kick()
    {
        Synonyms = ["kick"];
        Name = "kick";
        Description = "Excludes a user by ip or id. Usage: kick {ip/id} {reason}";
    }
}