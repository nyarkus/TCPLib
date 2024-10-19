using Google.Protobuf;
using System;
using TCPLib.Net;

namespace TCPLib.Classes
{
    public struct DataPackageSource : IEquatable<DataPackageSource>
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }

        public DataPackageSource(string type, byte[] data)
        {
            Type = type;
            Data = data;
        }

        public bool Equals(DataPackageSource other)
            => Type == other.Type && Data.Equals(other.Data);

        public byte[] Pack()
        => new TCPLib.Protobuf.DataPackage { Data = ByteString.CopyFrom(Data), Type = Type }.ToByteArray();

        public DataPackage<T> As<T>() where T : IDataSerializable<T>, new()
        {
            return new DataPackage<T>(Type, Data);
        }
    }
}
