using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net.DPDispatcher;

namespace TCPLib.Server.DPDispatcher
{
    public delegate Task DataPackageReceive(DataPackageSource package);
    public class DPHandlerRegistry
    {
        public event DataPackageReceive OnReceived;

        public DPFilter filter;

        public static DPHandlerRegistry Create(DPFilter filter, params DataPackageReceive[] methods)
        {
            if (methods.Length == 0)
                throw new ArgumentException("There is a lack of methods", nameof(methods));

            var dpd = new DPHandlerRegistry();
            foreach (var method in methods)
                dpd.OnReceived += method;

            
            dpd.filter = filter;
            return dpd;
        }
        internal async Task Invoke(DataPackageSource package)
            => await OnReceived.Invoke(package);

        private DPHandlerRegistry() { }
    }
}
