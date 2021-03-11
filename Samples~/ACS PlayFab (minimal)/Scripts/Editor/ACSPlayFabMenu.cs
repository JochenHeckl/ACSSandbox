using System.IO;
using System.IO.Compression;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Editor
{
	public static class ACSPlayFabMenu
	{
		[MenuItem( "ACS Sandbox/ACS PlayFab/Build PlayFab Server" )]
		public static void BuildWindowsServer()
		{
			var buildConfigName = "BuildConfiguration.ACSPlayFab.Windows.Server";

			var buildConfigurationGuid = AssetDatabase.FindAssets( buildConfigName ).FirstOrDefault();

			if ( string.IsNullOrEmpty( buildConfigurationGuid ) )
			{
				throw new InvalidDataException( $"{buildConfigName} could not be found." );
			}

			var assetPath = AssetDatabase.GUIDToAssetPath( buildConfigurationGuid );
			var buildConfiguration = AssetDatabase.LoadAssetAtPath<BuildConfiguration>( assetPath );

			var buildReport = BuildOperations.Build( buildConfiguration );

			Debug.Log( buildReport.summary.ToString() );
		}
    }
}