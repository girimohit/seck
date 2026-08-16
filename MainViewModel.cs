using SecureAppLocker.Services;
using SecureAppLocker.Helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        private readonly System.Windows.Threading.DispatcherTimer _refreshTimer;

        public ObservableCollection<string> RunningApps { get; set; }
        public ObservableCollection<string> LockedApps { get; set; }
        public ObservableCollection<string> AlwaysLockedApps { get; set; }

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
        public ICommand ChangePasswordCommand { get; }
        public ICommand AlwaysLockCommand { get; }
        public ICommand SessionUnlockCommand { get; }
        public ICommand RemoveAlwaysLockCommand { get; }

        public MainViewModel()
        {
            StartupHelper.EnableAutoStart();

            _processService = new ProcessService();
            _lockService = new LockService();
            _authService = new AuthService();

            _monitorService = new MonitorService(_processService, _lockService);
            _monitorService.Start();

            // to refresh process
            _refreshTimer = new System.Windows.Threading.DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(2);
            _refreshTimer.Tick += (s, e) => LoadProcesses();
            _refreshTimer.Start();

            RunningApps = new ObservableCollection<string>();
            LockedApps = new ObservableCollection<string>();
            AlwaysLockedApps = new ObservableCollection<string>();

            LockCommand = new RelayCommand(LockApp);
            UnlockCommand = new RelayCommand(UnlockApp);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
            AlwaysLockCommand = new RelayCommand(AlwaysLockApp);
            SessionUnlockCommand = new RelayCommand(SessionUnlockApp);
            RemoveAlwaysLockCommand = new RelayCommand(RemoveAlwaysLockApp);

            LoadProcesses();
            LoadAlwaysLockedApps();
        }

        private void LoadProcesses()
        {
            RunningApps.Clear();

            foreach (var app in _processService.GetRunningProcesses())
            {
                RunningApps.Add(app);
            }
        }

        private void LoadAlwaysLockedApps()
        {
            AlwaysLockedApps.Clear();
            foreach (var app in _lockService.GetAlwaysLockedApps())
            {
                AlwaysLockedApps.Add(app);
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
            Logger.Log($"Locked app: {SelectedApp}");
        }

        private void UnlockApp()
        {
            string? appToUnlock = SelectedApp;
            if (string.IsNullOrEmpty(appToUnlock) || !LockedApps.Contains(appToUnlock))
            {
                appToUnlock = LockedApps.FirstOrDefault();
            }

            if (appToUnlock == null) return;

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter password to unlock '{appToUnlock}':", "Unlock App", "");

            if (string.IsNullOrEmpty(input)) return;

            if (_authService.Validate(input))
            {
                _lockService.RemoveLock(appToUnlock);
                LockedApps.Remove(appToUnlock);
                Logger.Log($"Unlocked app: {appToUnlock}");
            }
            else
            {
                System.Windows.MessageBox.Show("Incorrect password!", "Authentication Failed",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // ── Always Lock: persists across restarts ──

        private void AlwaysLockApp()
        {
            if (SelectedApp == null) return;

            var lowerName = SelectedApp.ToLower();
            _lockService.AddAlwaysLock(SelectedApp);

            if (!AlwaysLockedApps.Contains(lowerName))
                AlwaysLockedApps.Add(lowerName);

            // Remove from session locks if present (now permanently locked)
            LockedApps.Remove(SelectedApp);

            Logger.Log($"Always locked app: {SelectedApp}");
        }

        private void SessionUnlockApp()
        {
            string? appToUnlock = SelectedApp;
            if (string.IsNullOrEmpty(appToUnlock) || !AlwaysLockedApps.Contains(appToUnlock))
            {
                appToUnlock = AlwaysLockedApps.FirstOrDefault();
            }

            if (appToUnlock == null) return;

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter password to temporarily unlock '{appToUnlock}':\n(Will re-lock on next restart)",
                "Session Unlock", "");

            if (string.IsNullOrEmpty(input)) return;

            if (_authService.Validate(input))
            {
                // Remove from active locks only — stays in always-locked list
                _lockService.RemoveLock(appToUnlock);
                Logger.Log($"Session unlocked app: {appToUnlock}");
                System.Windows.MessageBox.Show(
                    $"'{appToUnlock}' is unlocked for this session.\nIt will be locked again on next startup.",
                    "Session Unlock",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
            }
            else
            {
                System.Windows.MessageBox.Show("Incorrect password!", "Authentication Failed",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void RemoveAlwaysLockApp()
        {
            string? appToRemove = SelectedApp;
            if (string.IsNullOrEmpty(appToRemove) || !AlwaysLockedApps.Contains(appToRemove))
            {
                appToRemove = AlwaysLockedApps.FirstOrDefault();
            }

            if (appToRemove == null) return;

            string input = Microsoft.VisualBasic.Interaction.InputBox(
                $"Enter password to permanently remove always-lock for '{appToRemove}':",
                "Remove Always Lock", "");

            if (string.IsNullOrEmpty(input)) return;

            if (_authService.Validate(input))
            {
                _lockService.RemoveAlwaysLock(appToRemove);
                AlwaysLockedApps.Remove(appToRemove);
                Logger.Log($"Removed always lock: {appToRemove}");
            }
            else
            {
                System.Windows.MessageBox.Show("Incorrect password!", "Authentication Failed",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private void ChangePassword()
        {
            string newPassword = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter new password : ",
                "Change Password",
                "");
            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                _authService.ChangePassword(newPassword);
            }
        }
    }
}