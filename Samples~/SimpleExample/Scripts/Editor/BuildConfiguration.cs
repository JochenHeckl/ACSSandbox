using UnityEditor;
using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Editor
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
		public BuildOptions buildOptions;
		public SceneAsset[] scenes;
		public BuildTarget target;
		public BuildTargetGroup targetGroup;
		public string assetBundleManifestPath;
	}
}