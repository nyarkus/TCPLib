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
    public class DPDispatcherBuilder
    {
        private readonly TCPLib.Client.Net.Server _client;
        private List<DPHandler> _handlers = new List<DPHandler>();
        public bool UseDecryption { get; set; } = true;
        public bool ThrowIfNotHandled { get; set; } = true;
        /// <param name="client">The client must be connected to the server</param>
        public DPDispatcherBuilder(TCPLib.Client.Net.Server client, params DPHandler[] handlers)
        {
            _client = client;
            _handlers = new List<DPHandler>(handlers);
        }
        public DPDispatcherBuilder AddDataPackageHandlerRegistry(DPHandler handler)
        {
            var handlers = new List<DPHandler>(_handlers)
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

        private static void CheckDuplications(DPHandler[] handlers)
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
