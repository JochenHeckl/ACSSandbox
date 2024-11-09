using JH.DataBinding;
using UnityEngine;

namespace ACSSandbox.Client
{
    [CreateAssetMenu(fileName = "ClientResources", menuName = "ACS Sandbox/ClientResources")]
    public class ClientResources : ScriptableObject
    {
        [field: SerializeField]
        public ClientArea[] Areas { get; set; }

        [field: SerializeField]
        public ViewAssetReference LoginPanel { get; set; }
    }
}
