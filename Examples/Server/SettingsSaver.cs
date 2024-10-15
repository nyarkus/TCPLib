using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using TCPLib.Server.SaveFiles;
using System.IO;

namespace ExampleServer
{
    public class SettingsSaver : ISettingsSaver
    {
        const string path = "Settings.yml";
        public void Save(TCPLib.Server.SaveFiles.Settings settings)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            File.WriteAllText(path, serializer.Serialize(settings));
        }
        public TCPLib.Server.SaveFiles.Settings Load()
        {
            if (!File.Exists(path))
                new TCPLib.Server.SaveFiles.Settings().Save();

            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<TCPLib.Server.SaveFiles.Settings>(File.ReadAllText(path));
        }
    }
}
