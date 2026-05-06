using System.Windows;

namespace SecureAppLocker
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var window = new Views.MainWindow();
            window.Hide(); // start hidden
            window.Show(); // comment this if full stealth
        }
    }
}
