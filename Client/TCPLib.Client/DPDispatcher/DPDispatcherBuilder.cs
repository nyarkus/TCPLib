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
        private List<DPHandlerRegistry> _handlers = new List<DPHandlerRegistry>();
        public bool UseDecryption = true;
        public bool ThrowIfNotHandled = true;

        public DPDispatcherBuilder(TCPLib.Client.Net.Server client, params DPHandlerRegistry[] handlers)
        {
            _client = client;
            _handlers = new List<DPHandlerRegistry>(handlers);
        }
        public DPDispatcherBuilder AddDataPackageHandlerRegistry(DPHandlerRegistry handler)
        {
            var handlers = new List<DPHandlerRegistry>(_handlers)
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

        private static void CheckDuplications(DPHandlerRegistry[] handlers)
        {
            var uniqueHandlers = new HashSet<DPFilter>();

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
