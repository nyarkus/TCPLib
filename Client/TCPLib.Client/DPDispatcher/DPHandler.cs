using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;
using TCPLib.Net.DPDispatcher;

namespace TCPLib.Client.DPDispatcher
{
    /// <summary>
    /// Data Package Dispatcher
    /// This class is designed to listen for packets and may be more convenient in some cases.
    /// For more information, visit the <a href="https://github.com/nyarkus/TCPLib/blob/master/documentation/DataPackageDispatcher.md">Documentation</a>.
    /// </summary>
    public class DPHandler
    {
        public event DataPackageReceive OnReceived;

        public DPFilter filter { get; set; }
        /// <summary>
        /// Creates a new package handler
        /// </summary>
        /// <param name="filter">Condition for package type</param>
        /// <param name="methods">Methods that will be called if the condition is triggered</param>
        /// <returns>Package handler needed for <see cref="TCPLib.Client.DPDispatcher.DPDispatcherBuilder"/></returns>
        /// <exception cref="ArgumentException">If no method is passed, this exception will be thrown</exception>
        public static DPHandler Create(DPFilter filter, params DataPackageReceive[] methods)
        {
            if (methods.Length == 0)
                throw new ArgumentException("There is a lack of methods", nameof(methods));

            var dpd = new DPHandler();
            foreach (var method in methods)
                dpd.OnReceived += method;

            
            dpd.filter = filter;
            return dpd;
        }
        internal async Task Invoke(DataPackageSource package)
            => await OnReceived.Invoke(package);

        private DPHandler() { }
    }
}
