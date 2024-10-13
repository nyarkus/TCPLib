using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Net;

namespace TCPLib.Client.DPDispatcher
{
    public class DataPackageHandlerRegistry<T> where T : IDataSerializable<T>, new()
    {
        public delegate Task DataPackageReceive(DataPackage<T> package);
        public event DataPackageReceive OnReceived;

        public DataPackageFilter filter;

        public static DataPackageHandlerRegistry<T> Create(DataPackageFilter filter, params DataPackageReceive[] methods)
        {
            var dpd = new DataPackageHandlerRegistry<T>();
            foreach (var method in methods)
                dpd.OnReceived += method;

            dpd.filter = filter;
            return dpd;
        }

        private DataPackageHandlerRegistry() { }
    }
}
