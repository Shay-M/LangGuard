using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LangGuard.Core
{
    public sealed class KeyboardHookService : IDisposable
    {
        private readonly LanguageService _languageService;
        private readonly SoundService _soundService;

        private AppConfig _cfg;
        private IntPtr _hook = IntPtr.Zero;
        private LowLevelKeyboardProc? _proc;

        private long _lastKeyTick;
        private ushort _lastLang;
        private bool _needBeep = true;

        public KeyboardHookService(AppConfig cfg, SoundService soundService, LanguageService languageService)
        {
            _cfg = cfg;
            _soundService = soundService;
            _languageService = languageService;
            _lastKeyTick = Environment.TickCount64;
            _lastLang = 0;
        }

        public void UpdateConfig(AppConfig cfg)
        {
            _cfg = cfg;
            _soundService.UpdateConfig(cfg);
        }

        public void Start()
        {
            if (_hook != IntPtr.Zero)
                return;

            _proc = HookCallback;
            _hook = SetHook(_proc);
        }

        public void Dispose()
        {
            if (_hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hook);
                _hook = IntPtr.Zero;
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            const int WM_KEYDOWN = 0x0100;
            const int WM_SYSKEYDOWN = 0x0104;

            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                var info = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                if (IsLetterKey(info.vkCode))
                {
                    HandleLetterKeyPress();
                }
            }

            return CallNextHookEx(_hook, nCode, wParam, lParam);
        }

        private void HandleLetterKeyPress()
        {
            if (IsModifierDown())
            {
                _lastKeyTick = Environment.TickCount64;
                return;
            }

            IntPtr hwnd = GetForegroundWindow();
            if (hwnd != IntPtr.Zero && hwnd != _lastHwnd)
            {
                _needBeep = true; // Force a beep on first letter in the new window
                _lastHwnd = hwnd;
            }


            long now = Environment.TickCount64;
            bool isNewSession = (now - _lastKeyTick) > _cfg.IdleTimeoutMs;

            ushort lang = _languageService.GetForegroundLanguageId();

            if (_cfg.BeepOnLanguageChange && lang != 0 && lang != _lastLang)
            {
                _needBeep = true;
            }

            if (_cfg.PlayOnEveryKey || isNewSession || _needBeep)
            {
                _soundService.PlayForLanguage(lang);
                _needBeep = false;
            }

            _lastLang = lang;
            _lastKeyTick = now;
        }

        private static bool IsLetterKey(uint vk)
        {
            return vk >= 0x41 && vk <= 0x5A;
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule!;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static bool IsModifierDown()
        {
            // High-order bit is set if key is down
            bool ctrl = (GetAsyncKeyState(0x11) & 0x8000) != 0; // VK_CONTROL
            bool alt = (GetAsyncKeyState(0x12) & 0x8000) != 0;  // VK_MENU
            bool win = (GetAsyncKeyState(0x5B) & 0x8000) != 0 || (GetAsyncKeyState(0x5C) & 0x8000) != 0; // VK_LWIN/VK_RWIN
            return ctrl || alt || win;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        private IntPtr _lastHwnd = IntPtr.Zero;


    }
}
