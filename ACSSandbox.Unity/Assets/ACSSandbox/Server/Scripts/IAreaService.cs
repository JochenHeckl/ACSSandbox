using System.Threading.Tasks;
using ACSSandbox.AreaServiceProtocol;

namespace ACSSandbox.Server
{
    internal interface IAreaService
    {
        ServerSend Send { get; }
        ServerReceive Receive { get; }

        void StartService(int areaServicePort);
        Task StopService();
    }
}
