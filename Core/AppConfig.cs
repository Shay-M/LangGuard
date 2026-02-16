namespace LangGuard.Core
{
    public sealed class AppConfig
    {
        public int IdleTimeoutMs { get; set; } = 4000;

        public bool PlayOnEveryKey { get; set; } = false;

        public bool BeepOnLanguageChange { get; set; } = true;

        public bool StartWithWindows { get; set; } = true;

        public string? SoundHebrewPath { get; set; } = null;
        public string? SoundEnglishPath { get; set; } = null;

        public ModifierKeys HotkeyModifiers { get; set; } = ModifierKeys.Control | ModifierKeys.Alt;
        public int HotkeyVk { get; set; } = 0x54; // 'T'
    }
}
