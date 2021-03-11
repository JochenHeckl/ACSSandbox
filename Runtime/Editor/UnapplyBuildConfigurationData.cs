using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEditor;

namespace de.JochenHeckl.Unity.ACSSandbox.Editor
{
	public class UnapplyBuildConfigurationData
	{
		public string ActiveIncludes { get; set; }
		public BuildTargetGroup TargetGroup { get; set; }
	}
}
