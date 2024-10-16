using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net.DPDispatcher;

namespace TCPLib.Client.DPDispatcher
{
    public class DPHandlerRegistry
    {
        public event DataPackageReceive OnReceived;

        public DPFilter filter { get; set; }

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
