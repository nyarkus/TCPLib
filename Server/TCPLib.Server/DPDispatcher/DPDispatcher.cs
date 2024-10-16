using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPLib.Server.DPDispatcher
{
    public class DPDispatcher : IDisposable
    {
        private readonly TCPLib.Server.Net.Client _client;
        private readonly DPHandlerRegistry[] _handlers;

        public bool UseDecryption { get; set; }
        public bool ThrowIfNotHandled { get; set; }

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
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

        public void Dispose()
        {
            _cancellationTokenSource.Dispose();
        }

        internal DPDispatcher(TCPLib.Server.Net.Client client, DPHandlerRegistry[] handlers, bool UseDecryption, bool ThrowIfNotHandled)
        {
            _client = client;
            _handlers = handlers;
            this.UseDecryption = UseDecryption;
            this.ThrowIfNotHandled = ThrowIfNotHandled;
        }
    }
}
