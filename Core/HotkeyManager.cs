using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace LangGuard.Core
{
    public sealed class HotkeyManager : IDisposable
    {
        private readonly HwndSource _source;
        private readonly int _id;

        public event Action? HotkeyPressed;

        public HotkeyManager(int id)
        {
            _id = id;

            var p = new HwndSourceParameters("LangGuardHotkeyMessageOnly")
            {
                Width = 0,
                Height = 0,
                WindowStyle = 0,
                ParentWindow = new IntPtr(-3) // HWND_MESSAGE
            };

            _source = new HwndSource(p);
            _source.AddHook(WndProc);
        }

        public bool Register(ModifierKeys modifiers, int vk)
        {
            UnregisterHotKey(_source.Handle, _id);
            return RegisterHotKey(_source.Handle, _id, (uint)modifiers, (uint)vk);
        }

        public void Dispose()
        {
            UnregisterHotKey(_source.Handle, _id);
            _source.RemoveHook(WndProc);
            _source.Dispose();
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;

            if (msg == WM_HOTKEY && wParam.ToInt32() == _id)
            {
                handled = true;
                HotkeyPressed?.Invoke();
            }

            return IntPtr.Zero;
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }

    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 0x0001,
        Control = 0x0002,
        Shift = 0x0004,
        Win = 0x0008
    }
}
