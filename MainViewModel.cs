using System.ComponentModel;
using System.Collections.ObjectModel;
using SecureAppLocker.Services;
using System.Windows.Input;
using SecureAppLocker.Helpers;

namespace SecureAppLocker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public readonly ProcessService _processService;
        public ObservableCollection<string> RunningApps { get; set; }

        public string? _selectedApp;
        public string? SelectedApp
        {
            get => _selectedApp;
            set
            {
                _selectedApp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedApp)));
            }
        }

        public ICommand KillAppCommand { get; private set; } = null!;
        public MainViewModel()
        {
            _processService = new ProcessService();
            RunningApps = new ObservableCollection<string>();
            KillAppCommand = new RelayCommand(KillSelectedApp);
            LoadProcesses();
        }

        public void LoadProcesses()
        {
            var processes = _processService.GetRunningProcesses();
            RunningApps.Clear();
            foreach(var process in processes)
            {
                RunningApps.Add(process);   
            }
        }

        private void KillSelectedApp()
        {
            if (!string.IsNullOrEmpty(SelectedApp))
            {
                _processService.KillProcess(SelectedApp);
                LoadProcesses();
            }
        }
        //private string _status = "App Ready";
        //public string Status
        //{
        //    get => _status;
        //    set
        //    {
        //        _status = value;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        //    }
        //}
    }
}