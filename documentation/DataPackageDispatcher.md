# Что такое DPDispatcher??!?!

## 👆🤓
**DPDispatcher** - это класс, введённый в версии **3.0.0**, который предназначен для приёма и __обработки пакетов__. В отличие от традиционных методов приёма пакетов, **DPDispatcher** использует механизм фильтрации, позволяя более гибко управлять обработкой входящих данных.

При получении пакета, **DPDispatcher** вызывает соответствующие методы, зарегистрированные в обработчиках, что позволяет легко расширять функциональность обработки данных.

## 🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️ ERM WHAT THE SIGMAA!?!?!?!? 🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️🗿🤫🧏‍♀️
**DPDispatcher** удобная, в некоторых случаев, штука для приёма пакетов.

Покажу на примере как писать с помощью этой штуки

### Серверная часть
Для начала создадим сейверы данных:
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
Сильно заморачиваться надними я не стал, т.к. это всего-лишь пример.

Теперь напишем логику для запуска сервера:
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
`BanSaver` и `SettingsSaver` - классы сохранения всяких данных сервера :000

Теперь сделаем свою структуру для состояния:
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
В нашем случае можно было бы обойтись без неё, однако в будущем я хочу показать одну удобную фичу
при работае с DPDispatcher.

Чтобы работать с DPDispatcher, нам нужно сделать свой класс для клиента:
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
Я не знаю как по-другому сделать адекватную реализацию работы с DPDispatcher.

У нас есть диспетчер пакетов:
```csharp
public DPDispatcher dispatcher;
```
Он и будет принимать и фильровать пакеты.

Далее мы создаём методы, которые будут обрабатывать полученые пакеты:
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
Вот здесь я использую ту самую фичу **(ВСТАВИТЬ URL СЮДА ПОТОМ)**:
```csharp
var state = package.As<State>();
```
**DataPackageSource** умеет явно преобразовываться в пакеты с известными типами данных.

После всего этого ужаса, мы создаём обработчики и указываем фильтры:
```csharp
var messageHandler = DPHandler.Create(DPFilter.Equals("msg"), new DataPackageReceive(OnMessage));
var stateHandler = DPHandler.Create(DPFilter.Equals("State"), new DataPackageReceive(OnState));
```
- `DPFilter` - фильтр применяющийся на тип полученого пакета.
- `DataPackageReceive` - Делегат определяющий структуру методов, которые будут вызываться в случаее прохождения пакета через фильт(`DPFilter`)

На этой строчке:
```csharp
dispatcher = new DPDispatcherBuilder(client, messageHandler, stateHandler).Build();
```
Мы создаём экземпляр класса `DPDispatcherBuilder` и передаём ему уже подключенного клиента.
`DPDispatcherBuilder` содержит некоторые методы для работы с ним, однако они нам не нужны и мы сразу собираем `DPDispatcher` с помощью метода `Build()`.

Теперь дополним основной код сервера:
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
Мы подписываемся на ивент `SuccessfulConnection` чтобы получать новых клиентов при новых подключениях.
При новом подключении мы создаём новый объект класса `CustomClient` и запускаем диспетчер.

### Клиентская часть
Поскольку серверу мы сделали структуру `State`, то и клиента не будем обделять, а то обидеться:
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
Диспетчер для клиента работает точно так же, поэтому я сразу покажу весь исходный код и объясню его:

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

На этих строках:
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
Мы создаём диспетчер, подключаемся к серверу и запускаем диспетчер в новом потоке, чтобы он не блокировал основной.

Потом мы получаем ввод с консоли, отправляем серверу пакет с типом `msg` и записываем время отправки:
```csharp
Console.WriteLine("Write something");
var input = Console.ReadLine();

await server.SendAsync(new DataPackageSource("msg", Encoding.UTF8.GetBytes(input)));

// Use of TimeProvider is optional and is synonymous with DateTimeOffset.UtcNow;
SendTime = TCPLib.Time.TimeProvider.Now;
```
После этого мы начинаем блокировать основной поток, пока сервер не ответит и после его ответа отключаемся:
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

Когда мы получаем ответ от сервера, перед завершением программы, мы отправляем ему сообщение:
```csharp
private static async Task OnState(DataPackageSource package)
{
    await client.ConnectedServer.SendAsync(new State() { Content = $"Ping = {(TCPLib.Time.TimeProvider.Now - SendTime).TotalMilliseconds:F1} ms" });

    Running = false;
}
```
В этом случае, мы вычисляем и отправляем ему задержку между отправкой и ответом.