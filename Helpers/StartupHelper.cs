using Microsoft.Win32;

namespace SecureAppLocker.Helpers
{
    public static class StartupHelper
    {
        public static void EnableAutoStart()
        {
            var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.SetValue("SecureAppLocker",
                System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        public static void DisableAutoStart()
        {
            var key = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            key?.DeleteValue("SecureAppLocker", false);
        }
    }
}