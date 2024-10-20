# Что такое DPDispatcher??!?!

## 👆🤓
**DPDispatcher** is a class introduced in version **3.0.0** that is designed for receiving and __processing packets__. Unlike traditional methods of packet reception, **DPDispatcher** uses a filtering mechanism, allowing for more flexible management of incoming data processing.

Upon receiving a packet, **DPDispatcher** calls the corresponding methods registered in the handlers, making it easy to extend the functionality of data processing.

## 🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️ ERM WHAT THE SIGMAA!?!?!?!? 🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️
**DPDispatcher** is a convenient tool for receiving packets in some cases.

Let me show you how to write using this tool with an example.

### Server Side
First, let's create data savers:
```csharp
using System;
using System.IO;
using Newtonsoft.Json;
using TCPLib.Server.SaveFiles;

namespace ExampleServer
{
    public struct SettingsSaver : ISettingsSaver
    {
        public Settings? settings;

        public SettingsSaver() { }

        public Settings Load()
        {
            if(settings == null)
                settings = new Settings();

            return settings;
        }

        public void Save(Settings settings)
            => this.settings = settings;
    }
    public struct BanSaver : IBanListSaver
    {
        Ban[] bans = [];

        public BanSaver() { }

        public Ban[] Load()
            => bans;

        public void Save(Ban[] bans)
            => this.bans = bans;
    }
}
```
I didn't put too much effort into them since this is just an example.

Now let's write the logic to start the server:
```csharp
using ExampleServer;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace DPDispatcherServer
{
    internal static class Program
    {

        private static async Task Main(string[] args)
        {
            var server = new Server(new ServerConfiguration(new BanSaver(), new SettingsSaver(), ServerComponents.BaseCommands) { DefaultPort = 2025 });

            await server.Start();
        }
    }
}
```
`BanSaver` and `SettingsSaver` are classes for saving various server data :000

Now let's create our own structure for the state:
```csharp
using System.Text;
using TCPLib.Net;

namespace DPDispatcherServer
{
    public struct State : IDataSerializable<State>
    {
        public string Content { get; set; }
        public State FromBytes(byte[] bytes)
        {
            return new State() { Content = Encoding.ASCII.GetString(bytes) };
        }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(Content);
        }
    }
}
```
In our case, we could do without it, but in the future, I want to show a convenient feature when working with `DPDispatcher`.

To work with `DPDispatcher`, we need to create our own class for the client:
```csharp
using System.Text;
using TCPLib.Classes;
using TCPLib.Net.DPDispatcher;
using TCPLib.Server.DPDispatcher;
using TCPLib.Server.Net;

namespace DPDispatcherServer
{
    public class CustomClient
    {
        public Client client;
        public DPDispatcher dispatcher;

        public CustomClient(Client client)
        {
            this.client = client;

            // Creating package handlers
            var messageHandler = DPHandler.Create(DPFilter.Equals("msg"), new DataPackageReceive(OnMessage));
            var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));

            // Create a dispatcher
            dispatcher = new DPDispatcherBuilder(client, messageHandler, stateHandler).Build();
        }

        private Task OnState(DataPackageSource package)
        {
            var state = package.As<State>();

            // Output to the console what the client sent us.
            TCPLib.Server.Console.Info(state.Value.Content);

            // We stop the dispatcher so that we don't create an endless loop.
            dispatcher.Stop();
            
            return Task.CompletedTask;
        }

        private async Task OnMessage(DataPackageSource package)
        {
            var content = Encoding.UTF8.GetString(package.Data);

            await client.SendAsync(new State() { Content = "I got your message!" });
        }
    }
}
```
I don't know how to implement a proper working solution with `DPDispatcher` in any other way.

We have a packet dispatcher:
```csharp
public DPDispatcher dispatcher;
```
It will receive and filter packets.

Next, we create methods that will handle the received packets:
```csharp
private Task OnState(DataPackageSource package)
{
    var state = package.As<State>();

    // Output to the console what the client sent us.
    TCPLib.Server.Console.Info(state.Value.Content);

    // We stop the dispatcher so that we don't create an endless loop.
    dispatcher.Stop();
            
    return Task.CompletedTask;
}

private async Task OnMessage(DataPackageSource package)
{
    var content = Encoding.UTF8.GetString(package.Data);

    await client.SendAsync(new State() { Content = "I got your message!" });
}
```
Here I use that very feature **(ВСТАВИТЬ URL СЮДА ПОТОМ)**:
```csharp
var state = package.As<State>();
```
`DataPackageSource` can be explicitly converted into packets with known data types.

After all this chaos, we create handlers and specify filters:
```csharp
var messageHandler = DPHandler.Create(DPFilter.Equals("msg"), new DataPackageReceive(OnMessage));
var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));
```
- `DPFilter` - a filter applied to the type of the received packet.
- `DataPackageReceive` - a delegate that defines the structure of the methods that will be called if the packet passes through the filter (`DPFilter`).

On this line:
```csharp
dispatcher = new DPDispatcherBuilder(client, messageHandler, stateHandler).Build();
```
We create an instance of the `DPDispatcherBuilder` class and pass it the already connected client. 
`DPDispatcherBuilder` contains some methods for working with it, but we don't need them, and we immediately build the `DPDispatcher` using the `Build()` method.

Now let's add to the main server code:
```csharp
using ExampleServer;
using TCPLib.Server;
using TCPLib.Server.Net;

namespace DPDispatcherServer
{
    internal static class Program
    {

        private static async Task Main(string[] args)
        {
            var server = new Server(new ServerConfiguration(new BanSaver(), new SettingsSaver(), ServerComponents.BaseCommands) { DefaultPort = 2025 });

            // Sign up for the event
            Client.SuccessfulConnection += OnConnection;
            await server.Start();
        }

        private static async Task OnConnection(Client client)
        {
            // Creating custom client class
            var cc = new CustomClient(client);

            // Start the dispatcher. While the dispatcher is running, the thread will be blocked.
            await cc.dispatcher.Start();
        }
    }
}
```
We subscribe to the `SuccessfulConnection` event to receive new clients upon new connections. When a new connection occurs, we create a new object of the `CustomClient` class and start the dispatcher.

### Client Side
Since we created a `State` structure for the server, we won't leave the client out either, or it might get upset:
```csharp
using System.Text;
using TCPLib.Net;

namespace DPDispatcherClient
{
    public struct State : IDataSerializable<State>
    {
        public string Content { get; set; }
        public State FromBytes(byte[] bytes)
        {
            return new State() { Content = Encoding.ASCII.GetString(bytes) };
        }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(Content);
        }
    }
}
```
The dispatcher for the client works exactly the same way, so I'll show the entire source code right away and explain it:

```csharp
using System.Text;
using TCPLib.Classes;
using TCPLib.Client;
using TCPLib.Client.DPDispatcher;
using TCPLib.Net.DPDispatcher;

namespace DPDispatcherClient
{
    internal static class Program
    {
        private static Client client = new Client();
        private static DateTimeOffset SendTime;
        private static bool Running = true;
        private static async Task Main(string[] args)
        {
            // Creating package handler
            var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));

            // Connecting to the server
            var server = await client.Connect("127.0.0.1:2025");

            // Create a DPDispatcher builder and immediately build it in DPDispatcher
            var dispatcher = new DPDispatcherBuilder(server, stateHandler).Build();

            // Start the dispatcher in a new thread.
            _ = Task.Run(dispatcher.Start);


            Console.WriteLine("Write something");
            var input = Console.ReadLine();

            await server.SendAsync(new DataPackageSource("msg", Encoding.UTF8.GetBytes(input)));

            // Use of TimeProvider is optional and is synonymous with DateTimeOffster.UtcNow;
            SendTime = TCPLib.Time.TimeProvider.Now;
            while (Running)
            {
                // That's not the right way to do it 🤓
                await Task.Delay(100);
            }
            dispatcher.Stop();
            await client.ConnectedServer.Disconnect();

            return;
            
        }

        private static async Task OnState(DataPackageSource package)
        {
            await client.ConnectedServer.SendAsync(new State() { Content = $"Ping = {(TCPLib.Time.TimeProvider.Now - SendTime).TotalMilliseconds:F1} ms" });

            Running = false;
        }
    }
}
```

On these lines:
```csharp
// Creating package handler
var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));

// Connecting to the server
var server = await client.Connect("127.0.0.1:2025");

// Create a DPDispatcher builder and immediately build it in DPDispatcher
var dispatcher = new DPDispatcherBuilder(server, stateHandler).Build();

// Start the dispatcher in a new thread.
_ = Task.Run(dispatcher.Start);
```
We create the dispatcher, connect to the server, and run the dispatcher in a new thread so that it doesn't block the main one.

Then we get input from the console, send a packet of type `msg` to the server, and record the time of sending:
```csharp
Console.WriteLine("Write something");
var input = Console.ReadLine();

await server.SendAsync(new DataPackageSource("msg", Encoding.UTF8.GetBytes(input)));

// Use of TimeProvider is optional and is synonymous with DateTimeOffset.UtcNow;
SendTime = TCPLib.Time.TimeProvider.Now;
```
After that, we start blocking the main thread until the server responds, and after its response, we disconnect:
```csharp
while (Running)
{
    // That's not the right way to do it 🤓
    await Task.Delay(100);
}
dispatcher.Stop();
await client.ConnectedServer.Disconnect();

return;
```

When we receive a response from the server, before the program ends, we send it a message:
```csharp
private static async Task OnState(DataPackageSource package)
{
    await client.ConnectedServer.SendAsync(new State() { Content = $"Ping = {(TCPLib.Time.TimeProvider.Now - SendTime).TotalMilliseconds:F1} ms" });

    Running = false;
}
```
In this case, we calculate and send it the delay between sending and receiving the response.

## Note
The client/server passed to `DPDispatcher` must be connected before starting the dispatcher.

### Source Code

- The client can be found [here](https://github.com/nyarkus/TCPLib/tree/master/Examples/DPDispatcherClient)
- The server can be found [here](https://github.com/nyarkus/TCPLib/tree/master/Examples/DPDispatcherServer)
