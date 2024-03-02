using System;
using ACSSandbox.AreaServiceProtocol;
using ACSSandbox.AreaServiceProtocol.ServerToClient;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using ServerToClient = ACSSandbox.AreaServiceProtocol.ServerToClient;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class ProtocolSerializerMemoryPackBenchmark
    {
        readonly ProtocolSerializerMemoryPack<byte> serializer = new();
        readonly ServerToClient.ServerHeartBeat message = new () { serverTimeSec = 1f };
        private byte[] messageBytes;

        public ProtocolSerializerMemoryPackBenchmark()
        {
            serializer.RegisterServerMessageDispatch<ServerHeartBeat>( MessageTypeId.ServerHeartBeat, (x) => {});
            messageBytes = serializer.Serialize(message).ToArray();
        }


        [Benchmark]
        public void ClientHeartBeatRoundtrip()
        {
            var byteStream = serializer.Serialize(message);
            serializer.DeserializedDispatch(byteStream);
        }

        [Benchmark]
        public void ClientHeartBeatDeserialze()
        {
            serializer.DeserializedDispatch(messageBytes);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = DefaultConfig.Instance.AddJob(
                Job.MediumRun.WithLaunchCount(1).WithToolchain(InProcessEmitToolchain.Instance)
            );

#if DEBUG
            config.WithOptions( ConfigOptions.DisableOptimizationsValidator );
#endif

            var summary = BenchmarkRunner.Run<ProtocolSerializerMemoryPackBenchmark>(config);

            Console.WriteLine(summary.ToString());
        }
    }
}
