using WpfClipboard = System.Windows.Clipboard;

namespace LangGuard.Core
{
    public static class SelectedTextTransformer
    {
        public static void ConvertClipboardText()
        {
            if (!WpfClipboard.ContainsText())
                return;

            string text = WpfClipboard.GetText();
            if (string.IsNullOrWhiteSpace(text))
                return;

            string converted = KeyboardLayoutConverter.Toggle(text);
            WpfClipboard.SetText(converted);
        }
    }
}
