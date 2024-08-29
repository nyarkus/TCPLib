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
