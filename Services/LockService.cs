using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SecureAppLocker.Services
{
    public class LockService
    {
        private readonly HashSet<string> _lockedApps = new();
        private readonly HashSet<string> _alwaysLockedApps = new();
        private readonly string _alwaysLockedFile = "always_locked.json";

        public LockService()
        {
            LoadAlwaysLocked();
            // On startup, enforce all always-locked apps immediately
            foreach (var app in _alwaysLockedApps)
            {
                _lockedApps.Add(app);
            }
        }

        // ── Session Locks (in-memory only) ──

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

        // ── Always-Locked (persisted to disk) ──

        public void AddAlwaysLock(string appName)
        {
            if (!string.IsNullOrWhiteSpace(appName))
            {
                var lower = appName.ToLower();
                _alwaysLockedApps.Add(lower);
                _lockedApps.Add(lower);
                SaveAlwaysLocked();
            }
        }

        public void RemoveAlwaysLock(string appName)
        {
            if (!string.IsNullOrWhiteSpace(appName))
            {
                var lower = appName.ToLower();
                _alwaysLockedApps.Remove(lower);
                _lockedApps.Remove(lower);
                SaveAlwaysLocked();
            }
        }

        public bool IsAlwaysLocked(string appName)
        {
            return !string.IsNullOrWhiteSpace(appName)
                && _alwaysLockedApps.Contains(appName.ToLower());
        }

        public List<string> GetAlwaysLockedApps()
        {
            return new List<string>(_alwaysLockedApps);
        }

        private void SaveAlwaysLocked()
        {
            try
            {
                var json = JsonSerializer.Serialize(_alwaysLockedApps.ToList());
                File.WriteAllText(_alwaysLockedFile, json);
            }
            catch { }
        }

        private void LoadAlwaysLocked()
        {
            if (File.Exists(_alwaysLockedFile))
            {
                try
                {
                    var json = File.ReadAllText(_alwaysLockedFile);
                    var apps = JsonSerializer.Deserialize<List<string>>(json);
                    if (apps != null)
                    {
                        foreach (var app in apps)
                            _alwaysLockedApps.Add(app);
                    }
                }
                catch { }
            }
        }
    }
}