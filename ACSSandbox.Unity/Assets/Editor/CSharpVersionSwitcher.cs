using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LangVersionPatcher.Editor
{
    public class LangVersionPatcher : AssetPostprocessor
    {
        private const string OldVersion = "9.0";
        private const string NewVersion = "10.0";

        [InitializeOnLoadMethod]
        public static void Setup()
        {
            IEnumerable<NamedBuildTarget> targets = typeof(NamedBuildTarget)
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(NamedBuildTarget))
                .Select(field => (NamedBuildTarget)field.GetValue(null));

            bool dirty = false;
            foreach (NamedBuildTarget target in targets)
            {
                const string CscFlag = "-langversion:";

                try
                {
                    string[] arguments = PlayerSettings.GetAdditionalCompilerArguments(target);
                    if (arguments.Any(argument => argument.StartsWith(CscFlag)))
                        continue;
                    PlayerSettings.SetAdditionalCompilerArguments(
                        target,
                        arguments.Append(CscFlag + NewVersion).ToArray()
                    );
                    dirty = true;
                }
                catch
                {
                    // ignore
                }
            }
            if (!dirty)
                return;
            string projectPath = Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
            foreach (string file in Directory.GetFiles(projectPath, "*.csproj"))
                File.Delete(file);
            foreach (string file in Directory.GetFiles(projectPath, "*.sln"))
                File.Delete(file);
            Debug.Log($"Patched C# version from {OldVersion} to {NewVersion}!");
        }

        public static string OnGeneratedCSProject(string path, string content)
        {
            const string MsBuildFlag = "LangVersion";
            return content.Replace(
                $"<{MsBuildFlag}>{OldVersion}</{MsBuildFlag}>",
                $"<{MsBuildFlag}>{NewVersion}</{MsBuildFlag}>"
            );
        }
    }
}
