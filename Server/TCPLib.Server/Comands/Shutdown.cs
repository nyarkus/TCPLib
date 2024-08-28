// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Server.Commands;

internal class Shutdown : ICommand
{
    public string[] Synonyms { get; }

    public string Name { get; }

    public string Description { get; }
    internal Server server { get; }

    public Task<bool> Execute(string[] args)
    {
        server.Stop();
        return Task.FromResult(true);
    }
    public Shutdown(Server server)
    {
        Synonyms = ["shutdown", "stop"];
        Name = "shutdown";
        Description = "Shuts down the server";
        this.server = server;
    }
}
