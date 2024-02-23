using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace ACSSandbox.Editor
{
	[CreateAssetMenu( menuName = "ACS Sandbox/BuildConfiguration" )]
	public class BuildConfiguration : ScriptableObject
	{
		public string projectName;
		
		public ScriptingImplementation scriptingImplementation;
		public ManagedStrippingLevel managedStrippingLevel;

		public string[] buildDefines;
		public string[] extraScriptingDefines;
		public string targetPath;
		public string targetName;
		public BuildOptions buildOptions;
		public SceneAsset[] scenes;
		public NamedBuildTarget buildTarget;
		public string assetBundleManifestPath;
	}
}