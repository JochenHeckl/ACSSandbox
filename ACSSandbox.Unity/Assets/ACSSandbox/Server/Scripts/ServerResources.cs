using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ACSSandbox.Server
{
    [CreateAssetMenu(fileName = "ServerResources", menuName = "ACS Sandbox/ServerResources")]
    public class ServerResources : ScriptableObject
    {
        public ServerArea[] Areas;
    }
}
