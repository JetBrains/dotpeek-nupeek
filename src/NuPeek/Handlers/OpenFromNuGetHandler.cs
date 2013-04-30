using System.Windows;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.DotPeek.Plugins.NuPeek.Controls;
using JetBrains.DotPeek.Plugins.NuPeek.ViewModels;
using JetBrains.ProjectModel;
using JetBrains.UI.Extensions.Commands;
using NuGet;

namespace JetBrains.DotPeek.Plugins.NuPeek.Handlers
{
    [ActionHandler("NuPeek.OpenFromNuget")]
    public class OpenFromNuGetHandler 
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

            var model = new PackageSelectorViewModel();
            var window = new Window
            {
                Title = "Select Package",
                Content = new PackageSelector(model),
                Width = 520,
                Height = 520,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize
            };
            model.CancelCommand = new SimpleCommand(window.Close);
            model.OpenCommand = new SimpleCommand(() =>
            {
                window.Close();

                if (model.SelectedPackage != null)
                {
                    var packagesToOpen = GetPackagesToOpen(PackageRepositoryFactory.Default.CreateRepository(model.PackageSource.ToString()), model.SelectedPackage.Id, model.SelectedPackage.Version, model.LoadDependencies);
                    OpenPackageFilesInCurrentSolution(context, packagesToOpen);
                }
                
            });
            window.ShowDialog();
        }
    }
}