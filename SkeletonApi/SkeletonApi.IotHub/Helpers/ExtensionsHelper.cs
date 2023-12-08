using System.Text.RegularExpressions;
using System.Text;

namespace SkeletonApi.IotHub.Helpers
{

    public static class ExtensionHelper
    {
        public static string GetReverseString(this string str)
        {
            char[] chars = str.ToCharArray();
            char temp;
            for (int i = 1; i < chars.Length; i += 2)
            {
                temp = chars[i];
                chars[i] = chars[i - 1];
                chars[i - 1] = temp;
            }
            string alternateReversedString = new string(chars);
            return alternateReversedString;
        }
        public static string RemoveUnicode(this string arg)
        {
            var a = arg.GetReverseString();
            StringBuilder sb = new StringBuilder(arg.Length);
            foreach (char c in a)
            {
                if ((int)c > 127) // you probably don't want 127 either
                    continue;
                if ((int)c < 32)  // I bet you don't want control characters 
                    continue;
                if (c == '%')
                    continue;
                if (c == '?')
                    continue;
                sb.Append(c);
            }
            var CleanString = Regex.Replace(sb.ToString(), @"\s{2,}", ".").TrimEnd(' ');
            return CleanString.Remove(CleanString.Length - 1, 1);
        }

        public static string[] ConvertCustomStringArrayToArray(this string input)
        {
            // Langkah 1: Hapus tanda kurung siku
            string cleanInput = input.Trim('[', ']');

            // Langkah 2: Pisahkan string menjadi elemen-elemen
            string[] elements = cleanInput.Split(',');

            // Langkah 3: Bersihkan elemen-elemen
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = elements[i].Trim();
            }

            // Hasil akhir
            return elements;
        }
    }
}
