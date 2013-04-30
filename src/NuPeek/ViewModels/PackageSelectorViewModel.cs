using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.DotPeek.Plugins.NuPeek.Properties;
using NuGet;

namespace JetBrains.DotPeek.Plugins.NuPeek.ViewModels
{
    public class PackageSelectorViewModel
        : INotifyPropertyChanged
    {
        public PackageSelectorViewModel()
        {
            Packages = new ObservableCollection<PackageDefinitionViewModel>();
            PackageSources = new ObservableCollection<Uri>();

            InitializePackageSources();

            PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "SearchTerm")
                    {
                        var searchTerm = SearchTerm.ToLowerInvariant();

                        if (searchTerm.Length < 3)
                        {
                            return;
                        }

                        Task.Factory.StartNew(() =>
                            {
                                var repository = new DataServicePackageRepository(PackageSource);
                                return repository.GetPackages()
                                                 .Where(p => p.Id.ToLower().StartsWith(searchTerm))
                                                 .OrderByDescending(p => p.Version)
                                                 .ToList()
                                                 .Select(p =>
                                                     {
                                                         var id = p.Id;
                                                         var version = p.Version.ToString();
                                                         return new PackageDefinitionViewModel(id, version);
                                                     })
                                                 .ToList();
                            }).ContinueWith(r =>
                                {
                                    if (!r.IsFaulted && r.IsCompleted && searchTerm == SearchTerm.ToLowerInvariant())
                                    {
                                        Packages.Clear();
                                        Packages.AddRange(r.Result);
                                    }
                                }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                };
        }

        private Uri _packageSource;
        private PackageDefinitionViewModel _package;
        private ObservableCollection<Uri> _packageSources;
        private ObservableCollection<PackageDefinitionViewModel> _packages;
        private string _searchTerm;
        private bool _loadDependencies;
        private ICommand _cancelCommand;
        private ICommand _openCommand;

        public Uri PackageSource
        {
            get { return _packageSource; }
            set
            {
                if (Equals(value, _packageSource)) return;
                _packageSource = value;
                OnPropertyChanged("PackageSource");
            }
        }

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                if (value == _searchTerm) return;
                _searchTerm = value;
                OnPropertyChanged("SearchTerm");
            }
        }

        public PackageDefinitionViewModel SelectedPackage
        {
            get { return _package; }
            set
            {
                if (Equals(value, _package)) return;
                _package = value;
                OnPropertyChanged("Package");
            }
        }

        public ObservableCollection<Uri> PackageSources
        {
            get { return _packageSources; }
            set
            {
                if (Equals(value, _packageSources)) return;
                _packageSources = value;
                OnPropertyChanged("PackageSources");
            }
        }

        public ObservableCollection<PackageDefinitionViewModel> Packages
        {
            get { return _packages; }
            set
            {
                if (Equals(value, _packages)) return;
                _packages = value;
                OnPropertyChanged("Packages");
            }
        }

        public bool LoadDependencies
        {
            get { return _loadDependencies; }
            set
            {
                if (value.Equals(_loadDependencies)) return;
                _loadDependencies = value;
                OnPropertyChanged("LoadDependencies");
            }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
            set
            {
                if (Equals(value, _cancelCommand)) return;
                _cancelCommand = value;
                OnPropertyChanged("CancelCommand");
            }
        }

        public ICommand OpenCommand
        {
            get { return _openCommand; }
            set
            {
                if (Equals(value, _openCommand)) return;
                _openCommand = value;
                OnPropertyChanged("OpenCommand");
            }
        }

        protected void InitializePackageSources()
        {
            var settings = Settings.LoadDefaultSettings(new PhysicalFileSystem("c:\\"));
            var packageSourceProvider = new PackageSourceProvider(settings);
            var packageSources = packageSourceProvider.GetEnabledPackageSources();

            foreach (var packageSource in packageSources)
            {
                PackageSources.Add(new Uri(packageSource.Source));
            }

            PackageSource = PackageSources.First();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
