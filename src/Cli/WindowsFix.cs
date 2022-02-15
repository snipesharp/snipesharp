using System.Runtime.InteropServices;

namespace Cli
{
    class WindowsFix
    {
        private const int STD_INPUT_HANDLE = -10;
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        public static void FixCmd() {
            var iStdIn = GetStdHandle(STD_INPUT_HANDLE);
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(iStdIn, out uint inConsoleMode)){
                Console.WriteLine("failed to get input console mode");
                Console.ReadKey();
                return;
            }
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode)){
                Console.WriteLine("failed to get output console mode");
                Console.ReadKey();
                return;
            }

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if (!SetConsoleMode(iStdIn, inConsoleMode)){
                Console.WriteLine($"failed to set input console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }
            if (!SetConsoleMode(iStdOut, outConsoleMode)){
                Console.WriteLine($"failed to set output console mode, error code: {GetLastError()}");
                Console.ReadKey();
                return;
            }
        }
    }
}