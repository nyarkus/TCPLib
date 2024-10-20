﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TCPLib.Server.DPDispatcher
{
    /// <summary>
    /// Data Package Dispatcher
    /// This class is designed to listen for packets and may be more convenient in some cases.
    /// For more information, visit the <a href="https://github.com/nyarkus/TCPLib/blob/master/documentation/DataPackageDispatcher.md">Documentation</a>.
    /// </summary>
    public class DPDispatcher : IDisposable
    {
        private readonly TCPLib.Server.Net.Client _client;
        private readonly DPHandler[] _handlers;

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

        private bool disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }

            disposed = true;
        }
        ~DPDispatcher()
        {
            Dispose(false);
        }

        internal DPDispatcher(TCPLib.Server.Net.Client client, DPHandler[] handlers, bool UseDecryption, bool ThrowIfNotHandled)
        {
            _client = client;
            _handlers = handlers;
            this.UseDecryption = UseDecryption;
            this.ThrowIfNotHandled = ThrowIfNotHandled;
        }
    }
}
