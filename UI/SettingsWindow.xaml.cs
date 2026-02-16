using System.Windows;
using Win32OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using LangGuard.Core;
using CoreModifierKeys = LangGuard.Core.ModifierKeys;

namespace LangGuard.UI
{
    public partial class SettingsWindow : Window
    {
        private readonly ConfigService _service;
        private readonly SoundService _soundService;
        private AppConfig _cfg;

        public SettingsWindow(ConfigService service, AppConfig cfg)
        {
            InitializeComponent();
            _service = service;
            _cfg = cfg;

            _soundService = new SoundService(cfg);

            IdleTimeoutBox.Text = _cfg.IdleTimeoutMs.ToString();
            PlayOnEveryKeyBox.IsChecked = _cfg.PlayOnEveryKey;
            BeepOnLanguageChangeBox.IsChecked = _cfg.BeepOnLanguageChange;
            StartWithWindowsBox.IsChecked = _cfg.StartWithWindows;

            HotkeyBox.Text = HotkeyText.Format(_cfg.HotkeyModifiers, _cfg.HotkeyVk);

            HebrewSoundBox.Text = _cfg.SoundHebrewPath ?? "";
            EnglishSoundBox.Text = _cfg.SoundEnglishPath ?? "";
        }

        private void ChangeHotkey_Click(object sender, RoutedEventArgs e)
        {
            var picker = new HotkeyPickerWindow(_cfg.HotkeyModifiers, _cfg.HotkeyVk)
            {
                Owner = this
            };

            bool? ok = picker.ShowDialog();
            if (ok == true)
            {
                _cfg.HotkeyModifiers = picker.PickedModifiers;
                _cfg.HotkeyVk = picker.PickedVk;
                HotkeyBox.Text = HotkeyText.Format(_cfg.HotkeyModifiers, _cfg.HotkeyVk);
            }
        }

        private void BrowseHebrew_Click(object sender, RoutedEventArgs e)
        {
            string? path = PickAudioFile();
            if (path != null)
            {
                HebrewSoundBox.Text = path;
            }
        }

        private void BrowseEnglish_Click(object sender, RoutedEventArgs e)
        {
            string? path = PickAudioFile();
            if (path != null)
            {
                EnglishSoundBox.Text = path;
            }
        }

        private void TestHebrew_Click(object sender, RoutedEventArgs e)
        {
            bool ok = _soundService.TestPlay(HebrewSoundBox.Text, fallbackHebrew: true, out string? error);
            if (!ok && error != null)
            {
                System.Windows.MessageBox.Show($"Failed to play audio.\n{error}", "LangGuard", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TestEnglish_Click(object sender, RoutedEventArgs e)
        {
            bool ok = _soundService.TestPlay(EnglishSoundBox.Text, fallbackHebrew: false, out string? error);
            if (!ok && error != null)
            {
                System.Windows.MessageBox.Show($"Failed to play audio.\n{error}", "LangGuard", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(IdleTimeoutBox.Text, out int ms) || ms < 200 || ms > 60000)
            {
                System.Windows.MessageBox.Show("Idle timeout must be between 200 and 60000 ms.");
                return;
            }

            _cfg.IdleTimeoutMs = ms;
            _cfg.PlayOnEveryKey = PlayOnEveryKeyBox.IsChecked == true;
            _cfg.BeepOnLanguageChange = BeepOnLanguageChangeBox.IsChecked == true;
            _cfg.StartWithWindows = StartWithWindowsBox.IsChecked == true;

            _cfg.SoundHebrewPath = NormalizePathOrNull(HebrewSoundBox.Text);
            _cfg.SoundEnglishPath = NormalizePathOrNull(EnglishSoundBox.Text);

            _service.Save(_cfg);

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string? PickAudioFile()
        {
            var dialog = new Win32OpenFileDialog
            {
                Filter = "Audio files (*.wav;*.mp3)|*.wav;*.mp3|WAV files (*.wav)|*.wav|MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*",
                CheckFileExists = true,
                Multiselect = false
            };

            bool? ok = dialog.ShowDialog();
            if (ok == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        private static string? NormalizePathOrNull(string text)
        {
            string trimmed = (text ?? "").Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return null;
            }

            return trimmed;
        }
    }
}
