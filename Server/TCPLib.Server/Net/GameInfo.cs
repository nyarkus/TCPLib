// This file uses Protocol Buffers from Google, which is licensed under BSD-3-Clause.

using Google.Protobuf;

namespace TCPLib.Server.Net
{
    public class GameInfo : IProtobufSerializable<GameInfo>
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public GameInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }
        public GameInfo() { }

        public byte[] ToByteArray()
        => new TCPLib.Protobuf.GameInfo() { Name = Name, Version = Version }.ToByteArray();

        public GameInfo FromBytes(byte[] bytes)
        {
            var gi = TCPLib.Protobuf.GameInfo.Parser.ParseFrom(bytes);

            return new GameInfo(gi.Name, gi.Version);
        }
    }
}