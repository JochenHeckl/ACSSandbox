using System;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.Build.Reporting;

using UnityEngine;

namespace ACSSandbox.Editor
{
	public static class BuildOperations
	{
		public static BuildReport Build( BuildConfiguration configuration )
		{
			RestoreBuildConfigurationData restoreBuildConfigurationData = null;

			try
			{
				restoreBuildConfigurationData = ApplyBuildConfiguration( configuration );

				var buildOptions = new BuildPlayerOptions()
				{
					assetBundleManifestPath = configuration.assetBundleManifestPath,
					extraScriptingDefines = configuration.extraScriptingDefines,
					locationPathName = Path.Combine(configuration.targetPath, configuration.targetName ),
					options = configuration.buildOptions,
					scenes = configuration.scenes.Select( AssetDatabase.GetAssetPath ).ToArray(),
					target = Enum.Parse<BuildTarget>( configuration.buildTarget.TargetName, true),
					targetGroup = configuration.buildTarget.ToBuildTargetGroup()
				};

				Directory.Delete( configuration.targetPath, true );

				var buildReport = BuildPipeline.BuildPlayer( buildOptions );

				Debug.Log( $"Build finished with result {buildReport.summary.result}." +
					$" {buildReport.summary.totalErrors} Errors." +
					$" {buildReport.summary.totalWarnings} Warnings." );

				return buildReport;
			}
			finally
			{
				RestoreBuildConfiguration( restoreBuildConfigurationData );
			}
		}

		private static RestoreBuildConfigurationData ApplyBuildConfiguration( BuildConfiguration configuration )
		{
			var restoreBuildConfigurationData = new RestoreBuildConfigurationData()
			{
				BuildTarget = configuration.buildTarget,
				ActiveIncludes = PlayerSettings.GetScriptingDefineSymbols( configuration.buildTarget )
			};

			PlayerSettings.productName = configuration.projectName;

			PlayerSettings.SetScriptingBackend( configuration.buildTarget, configuration.scriptingImplementation );
			PlayerSettings.SetManagedStrippingLevel( configuration.buildTarget, configuration.managedStrippingLevel );
			PlayerSettings.SetScriptingDefineSymbols( configuration.buildTarget, configuration.buildDefines );

			return restoreBuildConfigurationData;
		}

		private static void RestoreBuildConfiguration( RestoreBuildConfigurationData restoreBuildConfigurationData )
		{
			if ( restoreBuildConfigurationData == null )
			{
				return;
			}

			PlayerSettings.SetScriptingDefineSymbols(
				restoreBuildConfigurationData.BuildTarget,
				restoreBuildConfigurationData.ActiveIncludes );
		}
	}
}