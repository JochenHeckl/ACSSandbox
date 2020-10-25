using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.Build.Reporting;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Editor
{
	public static class BuildOperations
	{
		public static BuildReport Build( BuildConfiguration configuration )
		{
			ApplyBuildConfiguration( configuration );

			var buildOptions = new BuildPlayerOptions()
			{
				assetBundleManifestPath = configuration.assetBundleManifestPath,
				extraScriptingDefines = configuration.extraScriptingDefines,
				locationPathName = configuration.targetPath,
				options = configuration.buildOptions,
				scenes = configuration.scenes.Select( x => AssetDatabase.GetAssetPath( x ) ).ToArray(),
				target = configuration.target,
				targetGroup = configuration.targetGroup
			};

			var buildReport = BuildPipeline.BuildPlayer( buildOptions );

			Debug.Log( $"Build finished with result {buildReport.summary.result}." +
				$" {buildReport.summary.totalErrors} Errors." +
				$" {buildReport.summary.totalWarnings} Warnings." );

			return buildReport;
		}

		private static void ApplyBuildConfiguration( BuildConfiguration configuration )
		{
			PlayerSettings.productName = configuration.projectName;

			PlayerSettings.SetScriptingBackend( configuration.targetGroup, configuration.scriptingImplementation );
			PlayerSettings.SetManagedStrippingLevel( configuration.targetGroup, configuration.managedStrippingLevel );

			var defines = string.Join( ";", configuration.buildDefines.Distinct().ToArray() );
			PlayerSettings.SetScriptingDefineSymbolsForGroup( configuration.targetGroup, defines );
		}
	}
}