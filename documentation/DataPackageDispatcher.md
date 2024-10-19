# Что такое DPDispatcher??!?!

## 👆🤓
**DPDispatcher** - это класс :0, введённый в версии **3.0.0**, который предназначен для приёма и __обработки пакетов__. В отличие от традиционных методов приёма пакетов, **DPDispatcher** использует механизм фильтрации, позволяя более гибко управлять обработкой входящих данных.

При получении пакета, **DPDispatcher** вызывает соответствующие методы, зарегистрированные в обработчиках, что позволяет легко расширять функциональность обработки данных.

## 💪😎
**DPDispatcher** удобная, в некоторых случаев, штука для приёма пакетов.

Покажу на примере как писать с помощью этой штуки

### Серверная часть
Для начала создадим сервер:
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
`BanSaver` и `SettingsSaver` - классы сохранения всяких данных сервера :0

Для удобной работы с этой штуковиной, нам нужно создать свой класс для клиента:
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
Как ты мог заметить, тут уже есть логика и её объяснения написаны в коментариях, однако, давай я объясню чуть-чуть подробнее.

Этими строками мы создаём обработчики пакетов:
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

Здесь мы делаем всё по анологии с сервером:
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
            if(input == null)
            {
                dispatcher.Stop();
                await server.Disconnect();
                return;
            }
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