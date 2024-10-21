using GlobExpressions;
using Nuke.Common;
using Nuke.Common.IO;
using Serilog;

partial class Build : NukeBuild
{
    private string[] UnityProjects = { "ACSSandbox.Unity" };

    private string[] UnityProjectCleanDirectories =
    {
        "Library",
        "Logs",
        "Temp",
        "obj",
        "*.csproj",
    };

    Target CleanUnityProjects =>
        _ =>
            _.Executes(() =>
            {
                foreach (var project in UnityProjects)
                {
                    Log.Information("Cleaning up {Project}", project);

                    foreach (var globCleanTarget in UnityProjectCleanDirectories)
                    {
                        (RootDirectory / project)
                            .GlobDirectories(UnityProjectCleanDirectories)
                            .DeleteDirectories();

                        (RootDirectory / project)
                            .GlobFiles(UnityProjectCleanDirectories)
                            .DeleteFiles();
                    }
                }
            });
}
