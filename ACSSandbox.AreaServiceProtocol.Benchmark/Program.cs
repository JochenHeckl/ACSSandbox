using System;
using ACSSandbox.AreaServiceProtocol;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using ClientToServer = ACSSandbox.AreaServiceProtocol.ClientToServer;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class ProtocolSerializerMemoryPackBenchmark
    {
        readonly ProtocolSerializerMemoryPack serializer = new();
        readonly ClientToServer.ClientHeartBeat message = new ClientToServer.ClientHeartBeat() { clientTimeSec = 1f };
        private byte[] messageBytes;

        public ProtocolSerializerMemoryPackBenchmark()
        {
            messageBytes = serializer.Serialize(message).ToArray();
        }


        [Benchmark]
        public void ClientHeartBeatRoundtrip()
        {
            var byteStream = serializer.Serialize(message);
            var _ = serializer.Deserialize(byteStream);
        }

        [Benchmark]
        public void ClientHeartBeatDeserialze()
        {
            var _ = serializer.Deserialize(messageBytes);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var config = DefaultConfig.Instance.AddJob(
                Job.MediumRun.WithLaunchCount(1).WithToolchain(InProcessEmitToolchain.Instance)
            );

            var summary = BenchmarkRunner.Run<ProtocolSerializerMemoryPackBenchmark>(config);

            Console.WriteLine(summary.ToString());
        }
    }
}
