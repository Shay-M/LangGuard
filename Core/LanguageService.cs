using System;
using System.Runtime.InteropServices;

namespace LangGuard.Core
{
    public sealed class LanguageService
    {
        public const ushort LangEnglish = 0x0409;
        public const ushort LangHebrew = 0x040D;

        public ushort GetForegroundLanguageId()
        {
            IntPtr hwnd = GetForegroundWindow();
            if (hwnd == IntPtr.Zero)
                return 0;

            uint tid = GetWindowThreadProcessId(hwnd, IntPtr.Zero);
            IntPtr hkl = GetKeyboardLayout(tid);
            return (ushort)((ulong)hkl & 0xFFFF);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(uint idThread);
    }
}
