using System.Runtime.InteropServices;

namespace Fix
{
    class Windows
    {
        private const int STD_INPUT_HANDLE = -10;
        private const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_QUICK_EDIT = 0x0040;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        public static void FixCmd() {
            var iStdIn = GetStdHandle(STD_INPUT_HANDLE);
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            if (!GetConsoleMode(iStdIn, out uint inConsoleMode)) return;
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode)) return;

            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if (!SetConsoleMode(iStdIn, inConsoleMode)) return;
            if (!SetConsoleMode(iStdOut, outConsoleMode)) return;
        }
        internal static bool DisableQuickEdit() {

            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            // get current console mode
            uint consoleMode;
            if (!GetConsoleMode(consoleHandle, out consoleMode)) {
                Cli.Output.Error("Unable to get console mode");
                return false;
            }

            // Clear the quick edit bit in the mode flags
            consoleMode &= ~ENABLE_QUICK_EDIT;

            // set the new mode
            if (!SetConsoleMode(consoleHandle, consoleMode)) {
                Cli.Output.Error("Unable to disable console quick edit");
                return false;
            }

            return true;
        }
    }
}