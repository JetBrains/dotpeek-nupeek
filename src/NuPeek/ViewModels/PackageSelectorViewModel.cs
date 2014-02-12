using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using JetBrains.DotPeek.Plugins.NuPeek.Infrastructure;
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

                        SearchesInProgress++;

                        Task.Factory.StartNew(() =>
                            {
                                var repository = PackageRepositoryFactory.Default.CreateRepository(PackageSource.ToString());
                                return repository.Search(searchTerm, true)
                                                 .Where(p => p.Id.ToLower().StartsWith(searchTerm))
                                                 .OrderBy(p => p.Id)
                                                 .ThenByDescending(p => p.Version)
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
                                    if (r.IsFaulted)
                                    {
                                        var exception = r.Exception.InnerExceptions.FirstOrDefault();
                                        if (exception != null)
                                        {
                                            Status = exception.Message;
                                        }
                                        else
                                        {
                                            Status = "An unknown error occured accessing the Package Source.";
                                        }
                                    }
                                    if (!r.IsFaulted && r.IsCompleted && searchTerm == SearchTerm.ToLowerInvariant())
                                    {
                                        Status = "";
                                        Packages.Clear();
                                        Packages.AddRange(r.Result);
                                    }
                                    SearchesInProgress--;
                                }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                };
        }

        private Uri _packageSource;
        private PackageDefinitionViewModel _package;
        private ObservableCollection<Uri> _packageSources;
        private ObservableCollection<PackageDefinitionViewModel> _packages;
        private string _searchTerm;
        private string _status;
        private bool _loadDependencies;
        private ICommand _cancelCommand;
        private ICommand _openCommand;
        private int _searchesInProgress;

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

        public string Status
        {
            get { return _status; }
            set
            {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        public int SearchesInProgress
        {
            get { return _searchesInProgress; }
            set
            {
                if (value == _searchesInProgress) return;
                _searchesInProgress = value;
                OnPropertyChanged("SearchesInProgress");
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
            var settings = Settings.LoadDefaultSettings(new PhysicalFileSystem("C:\\"), null, null);
            var packageSourceProvider = new PackageSourceProvider(settings);
            var packageSources = packageSourceProvider.GetEnabledPackageSources().ToList();

            HttpClient.DefaultCredentialProvider = new SettingsCredentialProvider(new DotPeekCredentialProvider(), packageSourceProvider);

            if (!packageSources.Any())
            {
                packageSources.Add(PluginConstants.NuGetPackageSource);
            }

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
