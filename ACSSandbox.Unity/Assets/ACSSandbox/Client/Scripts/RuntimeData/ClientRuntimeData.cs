using UnityEngine;

namespace ACSSandbox.Client
{
    public class ClientRuntimeData
    {
        public ClientConfiguration Configuration { get; set; } = new();
        public ClientResources ClientResources { get; set; }

        public bool ConnectedToCoordinator { get; set; }
        public bool ConnectedToAreaServer { get; set; }
        public Transform WorldRootTransform { get; set; }
        public float ServerTimeSec { get; set; } = 0f;
        public string ActiveAreaId { get; set; }
        public ClientArea ActiveArea { get; set; }

        public DataSources DataSources { get; set; }
        public Canvas MainCanvas { get; set; }
        public ClientViews Views { get; set; } = new();
    }
}
