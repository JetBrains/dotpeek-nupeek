using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.Util;
using NuGet;
#if DP10
using JetBrains.DotPeek.AssemblyExplorer;
#else
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer;
#endif

namespace JetBrains.DotPeek.Plugins.NuPeek.Handlers
{
    public class OpenFromNuGetHandlerBase
    {
        protected void OpenPackageFilesInCurrentSolution(IDataContext context, IEnumerable<FileSystemPath> packageFiles)
        {
            ISolution solution = context.GetData<ISolution>(JetBrains.ProjectModel.DataContext.DataConstants.SOLUTION);
            if (solution == null)
            {
                return;
            }

            var assemblyExplorerManager = SolutionEx.GetComponent<IAssemblyExplorerManager>(solution);

            var assemblyExplorer = assemblyExplorerManager.Opened;
            if (assemblyExplorer == null)
            {
                return;
            }

            var explorerManager = SolutionEx.GetComponent<IAssemblyExplorerManager>(solution);
            explorerManager.AddItemsByPath(packageFiles.ToArray());
        }

        protected string RetrieveTemporaryPackageFile(IPackage package)
        {
            var temporaryPath = Path.Combine(Path.GetTempPath(), string.Format("{0} {1}.nupkg", package.Id, package.Version));
            using (FileStream stream = new FileStream(temporaryPath, FileMode.OpenOrCreate))
            {
                package.GetStream().CopyTo(stream);
            }
            return temporaryPath;
        }

        protected IEnumerable<FileSystemPath> GetPackagesToOpen(IPackageRepository repository, string id, string version, bool recurse)
        {
            List<FileSystemPath> returnValue = new List<FileSystemPath>();

            var package = repository.FindPackage(id, new SemanticVersion(version));
            if (package != null)
            {
                returnValue.Add(new FileSystemPath(RetrieveTemporaryPackageFile(package)));

                if (recurse)
                {
                    foreach (var dependency in Enumerable.SelectMany(package.DependencySets, d => d.Dependencies))
                    {
                        var childPackages = repository.FindPackagesById(dependency.Id);
                        var childPackage = childPackages.FindByVersion(dependency.VersionSpec).FirstOrDefault();
                        if (childPackage != null)
                        {
                            returnValue.AddRange(GetPackagesToOpen(repository, childPackage.Id, childPackage.Version.ToString(), recurse));
                        }
                    }
                }
            }

            return returnValue;
        }
    }
}