using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Protocol.ClientToServer
{
    public class SpawnRequest
    {
        public SerializableVector3 SpawnLocation { get; set; }
    }
}