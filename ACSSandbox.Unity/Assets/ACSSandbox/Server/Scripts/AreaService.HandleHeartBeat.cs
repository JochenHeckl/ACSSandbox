using System.Collections.Generic;
using ACSSandbox.Common.Network;
using UnityEngine;

namespace ACSSandbox.Server
{
    public partial class AreaService
    {
        private readonly Dictionary<
            NetworkId,
            (float clientTimeSec, float clientTimeDeltaSec)
        > clientHeartBeats = new();

        private void HandleHeartBeat(NetworkId networkId, float clientTimeSec)
        {
            clientHeartBeats[networkId] = (clientTimeSec, Time.time - clientTimeSec);
        }
    }
}
