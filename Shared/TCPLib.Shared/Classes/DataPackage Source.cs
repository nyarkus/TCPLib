using System;

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
    }
}
