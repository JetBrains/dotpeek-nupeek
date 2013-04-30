using System.Windows.Controls;
using JetBrains.DotPeek.Plugins.NuPeek.ViewModels;

namespace JetBrains.DotPeek.Plugins.NuPeek.Controls
{
    /// <summary>
    /// Interaction logic for PackageSelector.xaml
    /// </summary>
    public partial class PackageSelector 
        : UserControl
    {
        public PackageSelector(PackageSelectorViewModel model)
        {
            InitializeComponent();

            DataContext = model;
        }
    }
}
