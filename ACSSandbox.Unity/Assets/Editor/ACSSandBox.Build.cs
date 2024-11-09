using System.Diagnostics;
using System.IO;
using System.Linq;
using Unity.Logging;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Timeline.Actions;
using UnityEngine;

namespace ACSSandbox.Build
{
    public static class ACSSandboxBuild
    {
        public static string GetProjectPath()
        {
            return Directory.GetParent(Application.dataPath).FullName;
        }

        private static readonly string windowsClientBuildExecutablePath =
            $"{GetProjectPath()}/Build/Windows/Client/ACSSandbox.Client.exe";

        private static readonly string windowsServerBuildExecutablePath =
            $"{GetProjectPath()}/Build/Windows/Server/ACSSandbox.Server.exe";

        private static readonly string linuxServerBuildExecutablePath =
            $"{GetProjectPath()}/Build/Linux/Server/ACSSandbox.Server.exe";
        private static Process serverProcess;

        [MenuItem("Build/Build Client Windows", priority = 1)]
        public static void BuildClientWindows()
        {
            var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(
                "Assets/Settings/Build Profiles/Standalone Client Windows.asset"
            );

            Log.Info("Building client using profile {ProfileName}", profile.name);

            BuildProfile.SetActiveBuildProfile(profile);

            var buildPlayerOptions = new BuildPlayerOptions()
            {
                locationPathName = windowsClientBuildExecutablePath,
                scenes = profile.scenes.Select(x => x.path).ToArray(),
                extraScriptingDefines = profile.scriptingDefines,
                target = BuildTarget.StandaloneWindows64,
                subtarget = (int)StandaloneBuildSubtarget.Player,
                options = BuildOptions.DetailedBuildReport,
                targetGroup = BuildTargetGroup.Standalone,
            };

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Build Server Windows", priority = 1)]
        public static void BuildServerWindows()
        {
            var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(
                "Assets/Settings/Build Profiles/Standalone Server Windows.asset"
            );

            Log.Info("Building server using profile {ProfileName}", profile.name);

            BuildProfile.SetActiveBuildProfile(profile);

            var buildPlayerOptions = new BuildPlayerOptions()
            {
                locationPathName = windowsServerBuildExecutablePath,
                scenes = profile.scenes.Select(x => x.path).ToArray(),
                extraScriptingDefines = profile.scriptingDefines,
                target = BuildTarget.StandaloneWindows64,
                subtarget = (int)StandaloneBuildSubtarget.Server,
                options = BuildOptions.DetailedBuildReport,
                targetGroup = BuildTargetGroup.Standalone,
            };

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Build Server Linux", priority = 1)]
        public static void BuildServerLinux()
        {
            var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(
                "Assets/Settings/Build Profiles/Standalone Server Windows.asset"
            );

            Log.Info("Building server using profile {ProfileName}", profile.name);

            BuildProfile.SetActiveBuildProfile(profile);

            var buildPlayerOptions = new BuildPlayerOptions()
            {
                locationPathName = linuxServerBuildExecutablePath,
                scenes = profile.scenes.Select(x => x.path).ToArray(),
                extraScriptingDefines = profile.scriptingDefines,
                target = BuildTarget.StandaloneLinux64,
                subtarget = (int)StandaloneBuildSubtarget.Server,
                options = BuildOptions.DetailedBuildReport,
                targetGroup = BuildTargetGroup.Standalone,
            };

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);
        }

        [MenuItem("Build/Run Server Windows", priority = 100)]
        public static void RunServerWindows()
        {
            var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(
                "Assets/Settings/Build Profiles/Standalone Server Windows.asset"
            );

            Log.Info("Running server {Executable}", windowsServerBuildExecutablePath);

            // if (serverProcess != null)
            // {
            //     serverProcess.Kill();
            //     serverProcess = null;
            // }

            serverProcess = new Process();

            serverProcess.StartInfo.FileName = windowsServerBuildExecutablePath;

            // serverProcess.StartInfo.CreateNoWindow = true;
            serverProcess.StartInfo.UseShellExecute = true;

            try
            {
                // Start the process
                serverProcess.Start();
                serverProcess = null;
            }
            catch (System.Exception exception)
            {
                Log.Error(
                    exception,
                    "Failed to launch server: {Executable}",
                    windowsServerBuildExecutablePath
                );
            }
        }
    }
}
