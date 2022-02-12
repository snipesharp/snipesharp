using StringExtensions;

namespace Cli {
    class Output {
        public static void PrintLogo(){
            Console.WriteLine("===== [SnipeSharp] =====".Centered());
        }

        public static void Inform(string message){
            Console.WriteLine("[info]".Blue() + $" {message}");
        }

        public static void Warn(string message){
            Console.WriteLine("[warning]".Yellow() + $" {message}");
        }

        public static void Error(string message){
            Console.WriteLine("[error]".Red() + $" {message}");
        }

        public static void Input(string message){
            Console.Write("[input]".Blue() + $" {message}: ");
        }
    }
}