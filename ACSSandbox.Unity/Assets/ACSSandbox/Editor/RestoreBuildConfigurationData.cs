using UnityEditor.Build;

namespace ACSSandbox.Editor
{
	public class RestoreBuildConfigurationData
	{
		public string ActiveIncludes { get; set; }
		public NamedBuildTarget BuildTarget { get; set; }
	}
}
