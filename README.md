<h1 align="center">TCPLib</h1>

<p align="center">
  <a href="https://github.com/Kacianoki/TCPLib/actions/workflows/Tests.yml">
	  <img src="https://github.com/Kacianoki/TCPLib/actions/workflows/Tests.yml/badge.svg?branch=master">
  </a>
  <img src="https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FKacianoki%2FTCPLib&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false"></img>
  <a href="https://github.com/Kacianoki/TCPLib/pulse" alt="Activity">
        <img src="https://img.shields.io/github/commit-activity/m/Kacianoki/TCPLib" />
  </a>
</p>

# What is this?!

**TCPLib** - It's a low-level library for exchanging packets with a remote computer (🤓). 
TCPLib works on the TCP protocol and supports AES and RSA encryption.
For packet exchange, TCPLib uses [Protocol Buffers](https://github.com/protocolbuffers/protobuf).

# How do I use this thing? :0

Since this is a low-level library, using it can be quite tricky : (

## Implementing the Server Side

First, you need to implement two interfaces: [TCPLib.Server.SaveFiles.IBanListSaver](https://github.com/Kacianoki/TCPLib/blob/master/Server/TCPLib.Server/SaveClasses/BanList.cs#L42) and [TCPLib.Server.SaveFiles.ISettingsSaver](https://github.com/Kacianoki/TCPLib/blob/master/Server/TCPLib.Server/SaveClasses/Settings.cs#L21).

Your implementation might look like this:

**ISettingsSaver**
```csharp
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TCPLib.Server.SaveFiles;
using System.IO;

namespace ExampleServer
{
    public class SettingsSaver : ISettingsSaver
    {
        public void Save(TCPLib.Server.SaveFiles.Settings settings)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            File.WriteAllText("Settings.yml", serializer.Serialize(settings));
        }
        public TCPLib.Server.SaveFiles.Settings Load()
        {
            if (!File.Exists("Settings.yml"))
                new TCPLib.Server.SaveFiles.Settings().Save();
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<TCPLib.Server.SaveFiles.Settings>(File.ReadAllText("Settings.yml"));
        }
    }
}
```
**IBanListSaver**
```csharp
using System;
using System.IO;
using Newtonsoft.Json;
using TCPLib.Server.SaveFiles;

namespace ExampleServer
{
    public class BanSaver : IBanListSaver
    {
        public void Save(Ban[] bans)
        {
            File.WriteAllText("banlist.json", JsonConvert.SerializeObject(bans));
        }
        public Ban[] Load()
        {
            if (!File.Exists("banlist.json")) Save(Array.Empty<Ban>());
            return JsonConvert.DeserializeObject<Ban[]>(File.ReadAllText("banlist.json"));
        }
    }
}
```
Once you've implemented these two interfaces, you can start the server! But you'll need to code a bit more : (

To create the server, you can write this:

```csharp
using System;
using System.Threading.Tasks;
using TCPLib.Server;

namespace ExampleServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPLib.Server.Server server = new Server(new BanSaver(), new SettingsSaver());

            server.Stopped += OnStopped; // If you type the standard "stop" command in the console,
            // the server won't terminate the process, so we subscribe to this event.

            server.Start();
            server.ConsoleRead(); // We want to send commands to the server :0
        }

        static Task OnStopped()
        {
            Environment.Exit(0);

            return Task.CompletedTask;
        }
    }
}
```
Wow! We managed to start the **SERVER** 🎉

But it doesn't work with the client yet, so let's fix that!

```csharp
using System;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace ExampleServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TCPLib.Server.Server server = new Server(new BanSaver(), new SettingsSaver());

            server.Stopped += OnStopped;

            TCPLib.Server.Net.Client.SuccessfulConnection += OnConnected;

            server.Start();
            server.ConsoleRead();
        }

        private static async Task OnConnected(TCPLib.Classes.ResponseCode code, Client client)
        {
            while (true)
            {
                var message = await client.ReceiveSourceAsync(); // Here we get the raw byte array of the packet (this is basically a hack)
                TCPLib.Server.Console.Info(UTF8Encoding.UTF8.GetString(message.Data));
            }
        }

        static Task OnStopped()
        {
            Environment.Exit(0);

            return Task.CompletedTask;
        }
    }
}
```
It would be more appropriate to use `ReceiveAsync()` here, but for that, you would need to write a Protobuf schema, compile it, and implement the *TCPLib.Net.IProtobufSerializable* interface, which we don't need right now, so we made a hack instead 😎.

## Implementing the Client Side

Now we need to write a client for the server! Let's start with the simplest part:

```csharp
using TCPLib.Client;
using System.Net;
using System.Threading.Tasks;

namespace ExampleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Client client = new Client();

            var server = await client.Connect(IPAddress.Parse("127.0.0.1"), 2024); // 127.0.0.1 - local IP
        }
    }
}
```

To start sending messages to the server, we need to create a message class:

```csharp
using System.Text;
using TCPLib.Net;

namespace ExampleClient 
{
    internal class Message : IProtobufSerializable<Message>
    {
        public string Data;

        public Message FromBytes(byte[] bytes)
        {
            return new Message() { Data = Encoding.UTF8.GetString(bytes) };
        }

        public byte[] ToByteArray()
        {
            return Encoding.UTF8.GetBytes(Data);
        }

        public Message() { } // A parameterless constructor is required
    }
}
```

Once we have the message class, we can send objects of this class to the server:

```csharp
using TCPLib.Client;
using System.Net;
using System.Threading.Tasks;
using System;

namespace ExampleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Client client = new Client();

            var server = await client.Connect(IPAddress.Parse("127.0.0.1"), 2024);

            while (true)
            {
                string input = Console.ReadLine();

                await server.SendAsync(new Message() { Data = input });
            }
        }
    }
}
```
We wrote the server and the **CLIENT**! Hooray! 🥳
You can find code examples [right here](https://github.com/Kacianoki/TCPLib/tree/master/Examples).