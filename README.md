<h1 align="center">TCPLib</h2>

<p align="center">
  <a href="https://github.com/Kacianoki/TCPLib/actions/workflows/Tests.yml">
	  <img src="https://github.com/Kacianoki/TCPLib/actions/workflows/Tests.yml/badge.svg?branch=master">
  </a>
  <img src="https://hits.seeyoufarm.com/api/count/incr/badge.svg?url=https%3A%2F%2Fgithub.com%2FKacianoki%2FTCPLib&count_bg=%2379C83D&title_bg=%23555555&icon=&icon_color=%23E7E7E7&title=hits&edge_flat=false"></img>
  <a href="https://github.com/Kacianoki/TCPLib/pulse" alt="Activity">
        <img src="https://img.shields.io/github/commit-activity/m/Kacianoki/TCPLib" />
  </a>
</p>

# Что это такое?!

**TCPLib** - Это низкоуровневая библиотека для обменна пакетами с удалённым компьютером(🤓). 
TCPLib работает на TCP протоколе и поддерживает шифрование AES и RSA.
Для обмена пакетами TCPLib использует [Protocol Buffers](https://github.com/protocolbuffers/protobuf).

# А как этой штукой пользоваться? :0

Поскольку это низкоуровневая библиотека, пользоваться ей довольно не просто : (

## Реализация серверной части

Для начала вам нужно реализовать 2 интерфейса: [TCPLib.Server.SaveFiles.IBanListSaver](https://github.com/Kacianoki/TCPLib/blob/master/Server/TCPLib.Server/SaveClasses/BanList.cs#L42) и [TCPLib.Server.SaveFiles.ISettingsSaver](https://github.com/Kacianoki/TCPLib/blob/master/Server/TCPLib.Server/SaveClasses/Settings.cs#L21)

Их реализация может выглядеть вот так:

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

После того, как вы реализовали эти два интерфейса, вы уже можете запустить сервер! Но для этого нужно будет ещё покодить : (

Для создания сервера вы можете написать вооот это:

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

            server.Stopped += OnStopped; // Если написать в консоль стандартную
            // команду "stop", то сервер не завершит процесс, поэтому мы 
            // подписываемся на это событие.

            server.Start();
            server.ConsoleRead(); // Мы же хотим вводить команды серверу :0
        }

        static Task OnStopped()
        {
            Environment.Exit(0);

            return Task.CompletedTask;
        }
    }
}

```

Вот это да! Мы смогли запустить **СЕРВЕР**🎉

Но он никак не работает с клиентом, давай исправим!

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
                var message = await client.ReceiveSourceAsync(); // Здесь мы получаем исходный массив байтов пакета (фактически это костыль)
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
Правильнее было бы использовать здесь обычный `ReceiveAsync()`, но для него нужно будет писать
Protobuf схему, компилировать её, реализовывать интерфейс TCPLib.Net.IProtobufSerializable а оно
нам не нужно поэтому сделали костылём 😎.

## Реализация клиентской части

Теперь нам нужно написать клиента для сервера! Начнём с самого простого:
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

            var server = await client.Connect(IPAddress.Parse("127.0.0.1"), 2024); // 127.0.0.1 - локальный IP
        }
    }
}
```

Чтобы начать отправлять какие-то сообщения серверу, нам нужно создать класс сообщения:

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

        public Message() { } // Обязательно нужен конструктор не принимающий параметры
    }
}
```
Когда мы сделали класс сообщения, мы можен отправлять объекты этого класса серверу:

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

Мы написали сервер и клиент! Ура! 🥳
