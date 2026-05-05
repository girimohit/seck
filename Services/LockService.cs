using System.Collections.Generic;

namespace SecureAppLocker.Services
{
    public class LockService
    {
        private readonly HashSet<string> _lockedApps = new();

        public void AddLock(string appName)
        {
            if (!string.IsNullOrWhiteSpace(appName))
                _lockedApps.Add(appName.ToLower());
        }

        public void RemoveLock(string appName)
        {
            if (!string.IsNullOrWhiteSpace(appName))
                _lockedApps.Remove(appName.ToLower());
        }

        public bool IsLocked(string appName)
        {
            return _lockedApps.Contains(appName.ToLower());
        }

        public List<string> GetLockedApps()
        {
            return new List<string>(_lockedApps);
        }
    }
}