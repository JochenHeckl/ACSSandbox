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
			UnapplyBuildConfigurationData unapplyBuildConfigurationData = null;

			try
			{
				unapplyBuildConfigurationData = ApplyBuildConfiguration( configuration );

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

				Directory.Delete( Path.GetDirectoryName( configuration.targetPath ), true );

				var buildReport = BuildPipeline.BuildPlayer( buildOptions );

				Debug.Log( $"Build finished with result {buildReport.summary.result}." +
					$" {buildReport.summary.totalErrors} Errors." +
					$" {buildReport.summary.totalWarnings} Warnings." );

				return buildReport;
			}
			finally
			{
				UnapplyBuildConfiguration( unapplyBuildConfigurationData );
			}
		}

		private static UnapplyBuildConfigurationData ApplyBuildConfiguration( BuildConfiguration configuration )
		{
			var unapplyBuildConfigurationData = new UnapplyBuildConfigurationData()
			{
				TargetGroup = configuration.targetGroup,
				ActiveIncludes = PlayerSettings.GetScriptingDefineSymbolsForGroup( configuration.targetGroup )
			};

			PlayerSettings.productName = configuration.projectName;

			PlayerSettings.SetScriptingBackend( configuration.targetGroup, configuration.scriptingImplementation );
			PlayerSettings.SetManagedStrippingLevel( configuration.targetGroup, configuration.managedStrippingLevel );

			var defines = string.Join( ";", configuration.buildDefines.Distinct().ToArray() );
			PlayerSettings.SetScriptingDefineSymbolsForGroup( configuration.targetGroup, defines );

			return unapplyBuildConfigurationData;
		}

		private static void UnapplyBuildConfiguration( UnapplyBuildConfigurationData unapplyBuildConfigurationData )
		{
			if ( unapplyBuildConfigurationData == null )
			{
				return;
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(
				unapplyBuildConfigurationData.TargetGroup,
				unapplyBuildConfigurationData.ActiveIncludes );
		}
	}
}