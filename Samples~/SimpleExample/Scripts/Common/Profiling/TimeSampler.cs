using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using UnityEngine;

public class TimeSampler<SampleKeyType>
{
    private struct SampleData
	{
		public int numSamples;
		public float maxSampleTimeMilliSec;
		public float totalSampleTimeMilliSec;
	}

	private Dictionary<SampleKeyType, SampleData> sampleData = new Dictionary<SampleKeyType, SampleData>();
	private Stopwatch stopwatch = new Stopwatch();

	public void InitSample( SampleKeyType sample )
	{
		sampleData[sample] = default;
	}

	public void StartSample()
	{
		stopwatch.Restart();
	}

	public void StopSample( SampleKeyType sample )
	{
		var elapsedTimeMillliSec = stopwatch.ElapsedMilliseconds;
		var curSample = sampleData[sample];

		sampleData[sample] = new SampleData()
		{
			numSamples = curSample.numSamples + 1,
			maxSampleTimeMilliSec = Mathf.Max( curSample.maxSampleTimeMilliSec, elapsedTimeMillliSec ),
			totalSampleTimeMilliSec = curSample.totalSampleTimeMilliSec + elapsedTimeMillliSec
		};
	}

	public IEnumerable< string > MarkDownSamples( string title, Func<SampleKeyType, string> keyToString )
	{
		yield return $"## {title}";
		yield return "| Sample | sample count | avg [ms] | max [ms] |";
		yield return "| -| -| -| -|";
		
		foreach ( var sample in sampleData.OrderByDescending( x=>x.Value.maxSampleTimeMilliSec ) )
		{
			var averageSampleTimeMS = sample.Value.totalSampleTimeMilliSec / sample.Value.numSamples;

			yield return
				$"| {keyToString( sample.Key )} " +
				$"| {sample.Value.numSamples} " +
				$"| {averageSampleTimeMS:n2} " +
				$"| {sample.Value.maxSampleTimeMilliSec:n2} " +
				$"|";
		}
	}
}
