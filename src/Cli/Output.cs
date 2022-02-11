namespace Cli {
    class Output {
        public static void PrintLogo(){
            Console.WriteLine("===== [SnipeSharp] =====");
        }

        public static void Inform(string message){
            Console.WriteLine($"[info] {message}");
        }
    }
}