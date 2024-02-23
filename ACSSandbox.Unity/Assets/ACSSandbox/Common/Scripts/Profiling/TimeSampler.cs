using System;
using System.Diagnostics;

namespace ACSSandbox.Common.Profiling
{
    using SampleKeyType = Byte;
    public class TimeSampler : IDisposable
    {
        public static SampleData[] Samples { get; } = new SampleData[SampleKeyType.MaxValue];
        public record SampleData
        {
            public int numSamples;
            public float totalSampleTimeMilliSec;
            public float maxSampleTimeMilliSec;
        }
        
        private readonly SampleKeyType sampleKey;
        private readonly Stopwatch stopwatch = new();

        public TimeSampler( SampleKeyType sampleKey )
        {
            this.sampleKey = sampleKey;
            stopwatch.Restart();
        }
        
        public void Dispose()
        {
            var elapsedTimeMilliSec = stopwatch.ElapsedMilliseconds;

            var currentSample = Samples[sampleKey];

            Samples[sampleKey] = new SampleData
            {
                numSamples = currentSample.numSamples + 1,
                maxSampleTimeMilliSec = currentSample.maxSampleTimeMilliSec > elapsedTimeMilliSec
                    ? currentSample.maxSampleTimeMilliSec
                    : elapsedTimeMilliSec,
                totalSampleTimeMilliSec = currentSample.totalSampleTimeMilliSec + elapsedTimeMilliSec
            };
        }
    }
}