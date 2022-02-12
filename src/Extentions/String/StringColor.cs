namespace StringExtensions
{
    public static class StringColor
    {
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
    }
}