using ACSSandbox.Common.Network;
using JH.DataBinding;

namespace ACSSandbox.Client
{
    public class NetworkConnectionStatus : DataSourceBase<NetworkConnectionStatus>
    {
        // public AreaServiceConnectionStatus AreaServiceConnectionStatus{
        //     get;
        //     set;
        // }

        public float RoundtripTimeSec { get; set; }
    }
}
