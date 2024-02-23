using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Common
{
    public partial class NetworkSimulator : MonoBehaviour, INetworkClient, INetworkServer
    {
        public async Task RunClientAsync(
            string host,
            int servicePort,
            INetworkClientEventProcessor eventProcessor,
            CancellationToken cancellationToken
        )
        {
            Connections = new[] { NetworkId.Create() };

            eventProcessor.HandleConnect();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Yield();

                while (serverMessages?.TryDequeue(out var message) ?? false)
                {
                    eventProcessor.HandleInboundData(message);
                }
            }
        }

        public void Send(ReadOnlySpan<byte> message, TransportChannel channel)
        {
            clientMessages.Enqueue(message.ToArray());
        }
    }
}
