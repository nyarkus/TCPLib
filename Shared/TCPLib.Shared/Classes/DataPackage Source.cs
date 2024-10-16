namespace TCPLib.Classes
{
    public struct DataPackageSource
    {
        public string Type { get; set; }
        public byte[] Data { get; set; }

        public DataPackageSource(string type, byte[] data)
        {
            Type = type;
            Data = data;
        }

    }
}
