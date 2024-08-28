// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

using System.Net.Sockets;

namespace TCPLib.Server.Net;
public abstract partial class NetClient
{
    public EncryptType EncryptType { get; set; } = EncryptType.RSA;

    public TcpClient client;
    public NetworkStream stream;
    public uint id { get; private set; }
    public static List<NetClient> clients = new();
    public Encryptor Encryptor;
    public NetClient(TcpClient client, NetworkStream stream)
    {
        this.client = client;
        if (clients.Count == 0) id = 0;
        else id = clients.Last().id + 1;
        this.stream = stream;
    }
    public async Task Kick(KickMessage message)
    {
        try
        {
            await SendAsync(message);
        }
        catch { }
        client.Close();
        clients.Remove(this);
        GC.Collect();
    }
}
public enum EncryptType
{
    AES,
    RSA
}