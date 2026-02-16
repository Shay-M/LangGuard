using System.Windows;
using Forms = System.Windows.Forms;
using LangGuard.Core;
using LangGuard.UI;

namespace LangGuard
{
    public partial class App : System.Windows.Application
    {
        private Forms.NotifyIcon? _tray;
        private HotkeyManager? _hotkey;
        private KeyboardHookService? _keyboardHook;

        private readonly ConfigService _configService = new ConfigService();
        private AppConfig _config = new AppConfig();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _config = _configService.Load();

            StartupManager.ApplyStartupSetting(_config.StartWithWindows);

            _tray = new Forms.NotifyIcon
            {
                Visible = true,
                Text = "LangGuard",
                Icon = new System.Drawing.Icon(System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "tray.ico"))
            };

            var menu = new Forms.ContextMenuStrip();

            var settingsItem = new Forms.ToolStripMenuItem("Settings");
            settingsItem.Click += (_, _) =>
            {
                var win = new SettingsWindow(_configService, _config);
                bool? ok = win.ShowDialog();
                if (ok == true)
                {
                    ReloadConfigAndApply();
                }
            };

            var convertClipboardItem = new Forms.ToolStripMenuItem("Convert Clipboard (EN↔HE)");
            convertClipboardItem.Click += (_, _) =>
            {
                LangGuard.Core.SelectedTextTransformer.ConvertClipboardText();
                System.Media.SystemSounds.Asterisk.Play();
            };

            var exitItem = new Forms.ToolStripMenuItem("Exit");
            exitItem.Click += (_, _) => Shutdown();

            menu.Items.Add(settingsItem);
            menu.Items.Add(new Forms.ToolStripSeparator());
            menu.Items.Add(convertClipboardItem);
            menu.Items.Add(new Forms.ToolStripSeparator());
            menu.Items.Add(exitItem);

            _tray.ContextMenuStrip = menu;

            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            SetupHotkey();

            _keyboardHook = new KeyboardHookService(_config, new SoundService(_config), new LanguageService());
            _keyboardHook.Start();
        }

        private void ReloadConfigAndApply()
        {
            _config = _configService.Load();
            StartupManager.ApplyStartupSetting(_config.StartWithWindows);

            _keyboardHook?.UpdateConfig(_config);

            _hotkey?.Dispose();
            _hotkey = null;
            SetupHotkey();
        }

        private void SetupHotkey()
        {
            _hotkey = new HotkeyManager(1);
            _hotkey.HotkeyPressed += () =>
            {
                LangGuard.Core.SelectedTextTransformer.ConvertClipboardText();
                System.Media.SystemSounds.Asterisk.Play();
            };

            _hotkey.Register(_config.HotkeyModifiers, _config.HotkeyVk);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _keyboardHook?.Dispose();
            _hotkey?.Dispose();

            if (_tray != null)
            {
                _tray.Visible = false;
                _tray.Dispose();
            }
        }
    }
}
