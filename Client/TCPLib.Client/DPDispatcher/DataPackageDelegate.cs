using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Net;

namespace TCPLib.Client.DPDispatcher
{
    public class DataPackageDelegate<T> where T : IDataSerializable<T>, new()
    {
        public delegate Task DataPackageReceive(DataPackage<T> package);
        public event DataPackageReceive OnReceived;
        public DataPackageFilter filter;
        public static DataPackageDelegate<T> Create(DataPackageFilter filter, params DataPackageReceive[] methods)
        {
            var dpd = new DataPackageDelegate<T>();
            foreach (var method in methods)
                dpd.OnReceived += method;

            dpd.filter = filter;
            return dpd;
        }

        private DataPackageDelegate() { }
    }
}
