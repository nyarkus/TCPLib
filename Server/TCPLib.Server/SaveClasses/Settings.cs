// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Server.SaveFiles;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class Settings
{
    public string title = "Untitled";
    public string description = "";
    public int deleteLogsAfterDays = -1;
    public bool saveLogs = true;
    public int maxPlayers = 16;
    public ushort port = 2024;
    public void Save()
    {
        var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        File.WriteAllText("Settings.yml", serializer.Serialize(this));
    }
    public static Settings Load()
    {
        if (!File.Exists("Settings.yml"))
            new Settings().Save();
        var deserializer = new DeserializerBuilder().Build();
        GC.Collect();
        return deserializer.Deserialize<Settings>(File.ReadAllText("Settings.yml"));
    }
}