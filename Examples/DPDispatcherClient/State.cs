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