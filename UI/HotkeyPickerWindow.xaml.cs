using System.Globalization;
using System.Windows;
using System.Windows.Input;
using CoreModifierKeys = LangGuard.Core.ModifierKeys;
using WpfModifierKeys = System.Windows.Input.ModifierKeys;

namespace LangGuard.UI
{
    public partial class HotkeyPickerWindow : Window
    {
        public CoreModifierKeys PickedModifiers { get; private set; }
        public int PickedVk { get; private set; }

        public HotkeyPickerWindow(CoreModifierKeys currentMods, int currentVk)
        {
            InitializeComponent();
            PickedModifiers = currentMods;
            PickedVk = currentVk;
            PreviewText.Text = HotkeyText.Format(currentMods, currentVk);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
                return;
            }

            CoreModifierKeys mods = 0;

            if ((Keyboard.Modifiers & WpfModifierKeys.Control) == WpfModifierKeys.Control) mods |= CoreModifierKeys.Control;
            if ((Keyboard.Modifiers & WpfModifierKeys.Alt) == WpfModifierKeys.Alt) mods |= CoreModifierKeys.Alt;
            if ((Keyboard.Modifiers & WpfModifierKeys.Shift) == WpfModifierKeys.Shift) mods |= CoreModifierKeys.Shift;
            if ((Keyboard.Modifiers & WpfModifierKeys.Windows) == WpfModifierKeys.Windows) mods |= CoreModifierKeys.Win;

            if (e.Key < Key.A || e.Key > Key.Z)
            {
                PreviewText.Text = "Please press Ctrl/Alt/Shift + a letter (A-Z).";
                e.Handled = true;
                return;
            }

            int vk = KeyInterop.VirtualKeyFromKey(e.Key);

            PickedModifiers = mods;
            PickedVk = vk;

            DialogResult = true;
            Close();
        }
    }

    internal static class HotkeyText
    {
        public static string Format(CoreModifierKeys mods, int vk)
        {
            string key = ((char)vk).ToString(CultureInfo.InvariantCulture);

            string text = "";
            if ((mods & CoreModifierKeys.Control) == CoreModifierKeys.Control) text += "Ctrl + ";
            if ((mods & CoreModifierKeys.Alt) == CoreModifierKeys.Alt) text += "Alt + ";
            if ((mods & CoreModifierKeys.Shift) == CoreModifierKeys.Shift) text += "Shift + ";
            if ((mods & CoreModifierKeys.Win) == CoreModifierKeys.Win) text += "Win + ";

            return text + key.ToUpperInvariant();
        }
    }
}
