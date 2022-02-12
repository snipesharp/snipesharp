using DataTypes.SetText;

namespace StringExtensions
{
    public static class StringResponsive
    {
        public static string Centered(this string str){
            int spacesToAdd = str.Length % 2 != 0
                ? (Console.WindowWidth - 1) - str.Length 
                : Console.WindowWidth - str.Length;
            string spaces = new string(' ', (spacesToAdd / 2));
            return spaces + str + spaces;
        }

        public static string Cross(this string str){
            int even = Convert.ToInt32(str.Length % 2 == 0);
            var left = str.Substring(0, str.Length / 2);
            var right = str.Substring(str.Length / 2, (str.Length / 2) - even);
            return $"{left}{SetText.DarkRed}|{SetText.ResetAll}{right}";
        }
    }
}