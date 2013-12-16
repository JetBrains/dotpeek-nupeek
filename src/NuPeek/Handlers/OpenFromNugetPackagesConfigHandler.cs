using System.Collections.Generic;
using System.Linq;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.Util;
using Microsoft.Win32;
using NuGet;

namespace JetBrains.DotPeek.Plugins.NuPeek.Handlers
{
    [ActionHandler("NuPeek.OpenFromNugetPackagesConfig")]
    public class OpenFromNugetPackagesConfigHandler
        : OpenFromNuGetHandlerBase, IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return context.GetData<ISolution>(JetBrains.ProjectModel.DataContext.DataConstants.SOLUTION) != null;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            ISolution solution = context.GetData<ISolution>(JetBrains.ProjectModel.DataContext.DataConstants.SOLUTION);
            if (solution == null)
            {
                return;
            }

            string extensionFilters = "NuGet packages.config|packages.config|All files (*.*)|*.*";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.AddExtension = false;
            openFileDialog.ValidateNames = true;
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = extensionFilters;

            bool? openFileDialogResult = openFileDialog.ShowDialog();
            if ((!openFileDialogResult.GetValueOrDefault() ? 1 : (!openFileDialogResult.HasValue ? 1 : 0)) != 0)
            {
                return;
            }

            foreach (var packagesConfigFile in openFileDialog.FileNames)
            {
                var packagesConfig = new PackageReferenceFile(packagesConfigFile);
                var packagesToOpen = GetPackagesToOpen(packagesConfig);
                OpenPackageFilesInCurrentSolution(context, packagesToOpen);
            }
        }

        protected IEnumerable<FileSystemPath> GetPackagesToOpen(PackageReferenceFile packagesConfig)
        {
            var settings = Settings.LoadDefaultSettings(new PhysicalFileSystem("C:\\"), null, null);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var packageSources = packageSourceProvider.GetEnabledPackageSources().ToList();

            if (!packageSources.Any())
            {
                packageSources.Add(PluginConstants.NuGetPackageSource);
            }

            var repository = new AggregateRepository(packageSources
                .Select(s => PackageRepositoryFactory.Default.CreateRepository(s.Source)));

            List<FileSystemPath> returnValue = new List<FileSystemPath>();

            foreach (var package in packagesConfig.GetPackageReferences())
            {
                returnValue.AddRange(GetPackagesToOpen(repository, package.Id, package.Version.ToString(), false));
            }

            return returnValue;
        }
    }
}