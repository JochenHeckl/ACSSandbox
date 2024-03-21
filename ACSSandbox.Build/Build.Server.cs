using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Utilities.Collections;
using YamlDotNet.Core.Tokens;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.Docker.DockerTasks;

partial class Build : NukeBuild
{
    private const string LinuxServerTag = "65395cb44c3b4d5494d2ca1684361f/acs_sandbox.server.linux";

    Target BuildServerContainer =>
        _ =>
            _.Executes(() =>
            {
                DockerBuild(x =>
                    x.SetPath(RootDirectory)
                        .SetFile(
                            RootDirectory
                                / "ACSSandbox.Build"
                                / "Data"
                                / "Linux.Server"
                                / "Dockerfile"
                        )
                        .SetTag(LinuxServerTag)
                );

                DockerImagePrune();
            });

    Target RunServerContainer =>
        _ =>
            _.Executes(() =>
            {
                DockerRun(x => x.SetImage(LinuxServerTag).AddPublish(("1337:1337/udp")));
            });
}
