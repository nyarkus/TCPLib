using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPLib.Client.DPDispatcher
{
    public class DPDispatcher
    {
        private TCPLib.Client.Net.Server _client;
        private DataPackageHandlerRegistry[] _handlers;

        public bool UseDecryption;
        public bool ThrowIfNotHandled;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        public async Task Start()
        {
            while(!_cancellationTokenSource.IsCancellationRequested)
            {
                var package = await _client.ReceiveSourceAsync(UseDecryption, _cancellationTokenSource.Token);
                if (_cancellationTokenSource.IsCancellationRequested)
                    return;
                bool handled = false;
                foreach(var handler in _handlers)
                {
                    if(handler.filter.Check(package.Type))
                    {
                        await handler.Invoke(package);
                        handled = true;
                        break;
                    }
                }
                if(ThrowIfNotHandled && !handled)
                {
                    throw new InvalidOperationException("The data package was not handled by any registered handler.");
                }
            }
        }
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        internal DPDispatcher(TCPLib.Client.Net.Server client, DataPackageHandlerRegistry[] handlers, bool UseDecryption, bool ThrowIfNotHandled)
        {
            _client = client;
            _handlers = handlers;
            this.UseDecryption = UseDecryption;
            this.ThrowIfNotHandled = ThrowIfNotHandled;
        }
    }
}
