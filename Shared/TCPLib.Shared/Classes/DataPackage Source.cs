namespace TCPLib.Classes
{
    public struct DataPackageSource
    {
        public string Type;
        public byte[] Data;

        public DataPackageSource(string type, byte[] data)
        {
            Type = type;
            Data = data;
        }

    }
}
