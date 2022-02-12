namespace StringExtensions
{
    public static class StringExtension
    {
        /* Add basic color support */
        public static string Black(this string str){
            return $"\u001b[30m{str}\u001b[0m";
        }

        public static string Red(this string str){
            return $"\u001b[31m{str}\u001b[0m";
        }

        public static string Green(this string str){
            return $"\u001b[32m{str}\u001b[0m";
        }

        public static string Yellow(this string str){
            return $"\u001b[33m{str}\u001b[0m";
        }

        public static string Blue(this string str){
            return $"\u001b[34m{str}\u001b[0m";
        }

        public static string Magenta(this string str){
            return $"\u001b[35m{str}\u001b[0m";
        }

        public static string Cyan(this string str){
            return $"\u001b[36m{str}\u001b[0m";
        }

        public static string White(this string str){
            return $"\u001b[37m{str}\u001b[0m";
        }

        /* Add basic responsive printing functionality */
        public static string Centered(string text){
            int spacesToAdd = text.Length % 2 != 0
                ? (Console.WindowWidth - 1) - text.Length 
                : Console.WindowWidth - text.Length;
            string spaces = new string(' ', (spacesToAdd / 2));
            return spaces + text + spaces;
        }
    }
}