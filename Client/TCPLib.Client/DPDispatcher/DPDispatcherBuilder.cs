using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCPLib.Classes;

namespace TCPLib.Client.DPDispatcher
{
    
    public class DPDispatcherBuilder
    {
        private TCPLib.Client.Net.Server _client;
        private List<DataPackageHandlerRegistry> _handlers = new List<DataPackageHandlerRegistry>();
        public bool UseDecryption = true;
        public bool ThrowIfNotHandled = true;

        public DPDispatcherBuilder(TCPLib.Client.Net.Server client, params DataPackageHandlerRegistry[] handlers)
        {
            _client = client;
            _handlers = new List<DataPackageHandlerRegistry>(handlers);
        }
        public DPDispatcherBuilder AddDataPackageHandlerRegistry(DataPackageHandlerRegistry handler)
        {
            var handlers = new List<DataPackageHandlerRegistry>(_handlers)
            {
                handler
            };
            _handlers = handlers;
            return this;
        }

        public DPDispatcher Build()
        {
            var handlers = _handlers.ToArray();
            CheckDuplications(handlers);
            return new DPDispatcher(_client, handlers, UseDecryption, ThrowIfNotHandled);
        }

        private static void CheckDuplications(DataPackageHandlerRegistry[] handlers)
        {
            var uniqueHandlers = new HashSet<DataPackageFilter>();

            foreach (var handler in handlers)
            {
                if (!uniqueHandlers.Add(handler.filter))
                {
                    throw new ArgumentException($"Duplicate handler filter found: \"{handler.filter.ToString()}\"");
                }
            }
        }

    }

}
