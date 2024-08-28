// Copyright (C) Kacianoki - All Rights Reserved 
//  
//  This source code is protected under international copyright law.  All rights 
//  reserved and protected by the copyright holders. 
//  This file is confidential and only available to authorized individuals with the 
//  permission of the copyright holders.  If you encounter this file and do not have 
//  permission, please contact the copyright holders and delete this file.

namespace TCPLib.Server.Net;
public class BanMessage
{
    public TimeSpan? period;
    public string? reason;
    public BanMessage(TimeSpan? period = null, string? reason = null)
    {
        this.period = period;
        this.reason = reason;
    }
    public BanMessage(DateTime? period = null, string? reason = null)
    {
        this.period = period - DateTime.UtcNow;
        this.reason = reason;
    }
}