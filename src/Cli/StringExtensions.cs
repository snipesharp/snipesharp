using snipesharp.Cli;

namespace StringExtensions
{
    public static class StringExtension
    {
        /* Add basic responsive printing functionality */
        public static string Centered(this string str)
        {
            int spacesToAdd = str.Length % 2 != 0
                ? (Console.WindowWidth - 1) - str.Length 
                : Console.WindowWidth - str.Length;
            string spaces = new string(' ', (spacesToAdd / 2));
            return spaces + str + spaces;
        }

        public static string Cross(this string str)
        {
            return $"{str.Substring(0, str.Length / 2)}{SetText.DarkRed}|{SetText.ResetAll}{str.Substring(str.Length / 2, (str.Length / 2)-1)}";
        }
    }
}