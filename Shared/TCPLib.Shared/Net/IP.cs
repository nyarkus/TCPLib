using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPLib.Net
{
    /// <summary>
    /// Structure storing IP address and port
    /// </summary>
    public struct IP : IEquatable<IP>
    {
        public byte First { get; set; }
        public byte Second { get; set; }    
        public byte Third { get; set; }
        public byte Fourth { get; set; }

        public int? Port { 
            get
            {
                if (_portSeted)
                    return _port;
                else
                    return null;
            }
            set
            {
                if(!value.HasValue)
                {
                    _port = 0;
                    _portSeted = false;
                }
                else
                {
                    _port = value.Value;
                    _portSeted = true;
                }
            }
        }
        private int _port;
        private bool _portSeted;

        public override string ToString()
            => $"{First}.{Second}.{Third}.{Fourth}" + (_portSeted ? $":{_port}" : "");

        public bool Equals(IP other)
        {
            if (_portSeted)
                return _port == other._port && other._portSeted && EqualsIP(other);
            else
                return EqualsIP(other);
        }
        private bool EqualsIP(IP other)
        {
            return First == other.First && Second == other.Second && Third == other.Third && Fourth == other.Fourth;
        }
        public IP RemovePort()
        {
            var ip = new IP
            {
                First = First,
                Second = Second,
                Third = Third,
                Fourth = Fourth,
            };

            return ip;
        }

        public override int GetHashCode()
        {
            int hashCode = -1447650190;
            hashCode = hashCode * -1521134295 + First.GetHashCode();
            hashCode = hashCode * -1521134295 + Second.GetHashCode();
            hashCode = hashCode * -1521134295 + Third.GetHashCode();
            hashCode = hashCode * -1521134295 + Fourth.GetHashCode();
            hashCode = hashCode * -1521134295 + Port.GetHashCode();
            if(_portSeted)
                hashCode = hashCode * -1521134295 + _port.GetHashCode();
            return hashCode;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public static bool operator ==(IP first, IP second)
            => first.Equals(second);
        public static bool operator !=(IP first, IP second)
            => !first.Equals(second);
        public static implicit operator string(IP first)
        {
            return first.ToString();
        }
        /// <summary>
        /// You may not use this method, because the string can be implicitly converted to this structure
        /// </summary>
        /// <param name="s">A string of the form: ‘127.0.0.1:2025’</param>
        public static IP Parse(string s)
        {
            var bytes = s.Split('.');
            var ip = new IP();
            ip.First = byte.Parse(bytes[0]);
            ip.Second = byte.Parse(bytes[1]);
            ip.Third = byte.Parse(bytes[2]);
            
            bytes = bytes[3].Split(':');

            ip.Fourth = byte.Parse(bytes[0]);
            if (bytes.Length > 1 && !string.IsNullOrWhiteSpace(bytes[1]))
            {
                ip.Port = int.Parse(bytes[1]);
            }

            return ip;
        }
        public static implicit operator IP(string val)
            => Parse(val);
    }
}
