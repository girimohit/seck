using System.ComponentModel;
using System.Collections.ObjectModel;
using SecureAppLocker.Services;

namespace SecureAppLocker.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public readonly ProcessService _processService;
        public ObservableCollection<string> RunningApps { get; set; }

        public MainViewModel()
        {
            _processService = new ProcessService();
            RunningApps = new ObservableCollection<string>();
            LoadProcesses();
        }

        public void LoadProcesses()
        {
            var processes = _processService.GetRunningProcess();
            RunningApps.Clear();
            foreach(var process in processes)
            {
                RunningApps.Add(process);   
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