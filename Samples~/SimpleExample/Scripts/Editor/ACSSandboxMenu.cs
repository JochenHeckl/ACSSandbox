using System.Diagnostics;
using System.IO;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Editor
{
	public static class ACSSandboxMenu
	{
		[MenuItem( "ACS Sandbox/Kill ACSSandbox.Server" )]
		public static void KillWindowsServers()
		{
			string serverExecutableName = "ACSSandbox.Server";

			foreach ( var process in System.Diagnostics.Process.GetProcesses() )
			{
				if ( process.ProcessName.Contains( serverExecutableName ) )
				{
					process.Kill();
				}
			}
		}

		[MenuItem( "ACS Sandbox/Show TimeSamples" )]
		public static void ShowTimeSamples()
		{
			var allSamples = Directory.EnumerateFiles( Directory.GetCurrentDirectory(), "*.TimeSamples.md" ).ToArray();

			File.WriteAllLines(
				"TimeSamples.md",
				allSamples
					.SelectMany( x => File.ReadAllLines( x ).Concat( Enumerable.Repeat( "&nbsp;", 1 ) ) )
					.ToArray() );

			Process.Start( "TimeSamples.md" );
		}

		[MenuItem( "ACS Sandbox/Build/Build Windows Server" )]
		public static void BuildWindowsServer()
		{
			KillWindowsServers();

			var buildConfigurationGuid = AssetDatabase.FindAssets( "BuildConfiguration.Windows.Server" ).FirstOrDefault();

			if ( !string.IsNullOrEmpty( buildConfigurationGuid ) )
			{
				var assetPath = AssetDatabase.GUIDToAssetPath( buildConfigurationGuid );
				var buildConfiguration = AssetDatabase.LoadAssetAtPath<BuildConfiguration>( assetPath );

				BuildOperations.Build( buildConfiguration );
			}
		}
	}
}