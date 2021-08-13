using System;
using System.Collections.Generic;
using System.Linq;

using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.CopyPlugins);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target CopyPlugins =>
        _ => _
            .DependsOn(Compile)
            .Executes(() =>
            {
                AbsolutePath pluginsDir = RootDirectory / "plugins";
                AbsolutePath pluginsSourceDir = RootDirectory / "src" / "plugins";

                IEnumerable<Project> pluginProjects =
                    Solution.AllProjects
                        .Where(x => pluginsSourceDir.Contains(x.Path) &&
                                    !x.Name.EndsWith(".contract", StringComparison.InvariantCultureIgnoreCase) &&
                                    !x.Name.EndsWith(".shared", StringComparison.InvariantCultureIgnoreCase));

                foreach (Project pluginProject in pluginProjects)
                {
                    DotNetLogger(OutputType.Std, $"Building plugin {pluginProject.Name}...");

                    DotNetBuild(_ => _
                        .SetProjectFile(pluginProject)
                        .SetConfiguration(Configuration)
                    );

                    DotNetLogger(OutputType.Std, $"Copying plugin {pluginProject.Name} to {pluginsDir}...");
                    string targetFramework = pluginProject.GetTargetFrameworks()!.First();

                    AbsolutePath pluginBuildDir = pluginProject.Directory / "bin" / Configuration / targetFramework;
                    DeleteDirectory(pluginsDir / pluginProject.Name);

                    CopyDirectoryRecursively(
                        pluginBuildDir,
                        pluginsDir / pluginProject.Name,
                        excludeDirectory: info => info.Name == "ref",
                        excludeFile: file => file.Name.StartsWith("GarryDB.", StringComparison.InvariantCultureIgnoreCase)
                    );
                }
            });
}
