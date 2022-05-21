using DataTypes.SetText;
using System.Text.RegularExpressions;

namespace StringExtensions
{
    public static class StringResponsive
    {
        public static string Centered(this string str, bool rightSpaces=true, char fillChar=' '){
            var clean = new Regex(@"\x1b\[\d+\w").Replace(str, "");

            int spacesToAdd = clean.Length % 2 != 0
                ? (Console.WindowWidth - 1) - clean.Length 
                : Console.WindowWidth - clean.Length;
            string spaces = new string(fillChar, (spacesToAdd / 2));
            return (rightSpaces ? spaces + str + spaces : spaces + str);
        }

        public static string MakeGapRight(this string str, int finalLength) {
            while (str.Length < finalLength) str += ' ';
            return str;
        }

        public static string Cross(this string str){
            int even = Convert.ToInt32(str.Length % 2 == 0);
            var left = str.Substring(0, str.Length / 2);
            var right = str.Substring(str.Length / 2, (str.Length / 2) - even);
            return $"{left}{SetText.DarkRed}|{SetText.ResetAll}{right}";
        }
    }
}