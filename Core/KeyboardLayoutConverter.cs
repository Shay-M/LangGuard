using System.Collections.Generic;
using System.Text;

namespace LangGuard.Core
{
    public static class KeyboardLayoutConverter
    {
        private static readonly Dictionary<char, char> EnToHe = new()
        {
            ['q'] = '/', ['w'] = '\'', ['e'] = 'ק', ['r'] = 'ר', ['t'] = 'א',
            ['y'] = 'ט', ['u'] = 'ו', ['i'] = 'ן', ['o'] = 'ם', ['p'] = 'פ',
            ['a'] = 'ש', ['s'] = 'ד', ['d'] = 'ג', ['f'] = 'כ', ['g'] = 'ע',
            ['h'] = 'י', ['j'] = 'ח', ['k'] = 'ל', ['l'] = 'ך',
            ['z'] = 'ז', ['x'] = 'ס', ['c'] = 'ב', ['v'] = 'ה',
            ['b'] = 'נ', ['n'] = 'מ', ['m'] = 'צ',
            [';'] = 'ף', [','] = 'ת', ['.'] = 'ץ', ['/'] = '.',
        };

        private static readonly Dictionary<char, char> HeToEn = BuildReverse(EnToHe);

        public static string Toggle(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            int enHits = 0;
            int heHits = 0;

            foreach (char c in input)
            {
                char lower = char.ToLowerInvariant(c);
                if (EnToHe.ContainsKey(lower)) enHits++;
                if (HeToEn.ContainsKey(c)) heHits++;
            }

            return enHits >= heHits ? Convert(input, EnToHe) : Convert(input, HeToEn);
        }

        private static string Convert(string input, Dictionary<char, char> map)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                char lower = char.ToLowerInvariant(c);
                if (map.TryGetValue(lower, out char mapped))
                    sb.Append(mapped);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private static Dictionary<char, char> BuildReverse(Dictionary<char, char> forward)
        {
            var reverse = new Dictionary<char, char>();
            foreach (var kvp in forward)
            {
                if (!reverse.ContainsKey(kvp.Value))
                    reverse[kvp.Value] = kvp.Key;
            }
            return reverse;
        }
    }
}
