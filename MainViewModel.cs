using SecureAppLocker.Services;
using SecureAppLocker.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace SecureAppLocker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly ProcessService _processService;
        private readonly LockService _lockService;
        private readonly MonitorService _monitorService;
        private readonly AuthService _authService;

        public ObservableCollection<string> RunningApps { get; set; }
        public ObservableCollection<string> LockedApps { get; set; }

        private string? _selectedApp;
        public string? SelectedApp
        {
            get => _selectedApp;
            set
            {
                _selectedApp = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedApp)));
            }
        }

        public ICommand LockCommand { get; }
        public ICommand UnlockCommand { get; }

        public MainViewModel()
        {
            _processService = new ProcessService();
            _lockService = new LockService();
            _authService = new AuthService();

            _monitorService = new MonitorService(_processService, _lockService);
            _monitorService.Start();

            RunningApps = new ObservableCollection<string>();
            LockedApps = new ObservableCollection<string>();

            LockCommand = new RelayCommand(LockApp);
            UnlockCommand = new RelayCommand(UnlockApp);

            LoadProcesses();
        }

        private void LoadProcesses()
        {
            RunningApps.Clear();

            foreach (var app in _processService.GetRunningProcesses())
            {
                RunningApps.Add(app);
            }
        }

        private void LockApp()
        {
            if (SelectedApp != null)
            {
                _lockService.AddLock(SelectedApp);

                if (!LockedApps.Contains(SelectedApp))
                    LockedApps.Add(SelectedApp);
            }
        }

        private void UnlockApp()
        {
            if (SelectedApp == null) return;

            // TEMP: simple input (replace later with UI popup)
            string input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter password:", "Unlock App", "");

            if (_authService.Validate(input))
            {
                _lockService.RemoveLock(SelectedApp);
                LockedApps.Remove(SelectedApp);
            }
        }
    }
}