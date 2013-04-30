using System.ComponentModel;
using JetBrains.DotPeek.Plugins.NuPeek.Properties;

namespace JetBrains.DotPeek.Plugins.NuPeek.ViewModels
{
    public class PackageDefinitionViewModel
        : INotifyPropertyChanged
    {
        private string _id;
        private string _version;

        public PackageDefinitionViewModel()
        {
        }

        public PackageDefinitionViewModel(string id, string version)
        {
            _id = id;
            _version = version;
        }

        public string Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                if (value == _version) return;
                _version = value;
                OnPropertyChanged("Version");
            }
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