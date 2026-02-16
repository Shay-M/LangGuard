using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace LangGuard.Core
{
    public static class StartupManager
    {
        private const string RunKeyPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string AppName = "LangGuard";

        public static void ApplyStartupSetting(bool enable)
        {
            try
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true);
                if (key == null)
                    return;

                if (!enable)
                {
                    key.DeleteValue(AppName, throwOnMissingValue: false);
                    return;
                }

                string exePath = GetExecutablePath();
                key.SetValue(AppName, $"\"{exePath}\"");
            }
            catch
            {
            }
        }

        private static string GetExecutablePath()
        {
            string path = Process.GetCurrentProcess().MainModule?.FileName ?? "";
            if (!string.IsNullOrWhiteSpace(path))
                return path;

            return Path.Combine(AppContext.BaseDirectory, "LangGuard.exe");
        }
    }
}
