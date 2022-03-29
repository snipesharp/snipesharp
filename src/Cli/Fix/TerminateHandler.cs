using DataTypes.SetText;

namespace Fix
{
    public class TerminateHandler
    {
        // makes it so that the console cursor appears
        // after snipesharp is closed
        public static void FixCursor(){
            Console.CancelKeyPress += (s, ev) => {                
                // show the cursor back to the user
                SetText.DisplayCursor(true);

                // clear space from the cursor to the bottom
                for(int i = 0; i < Console.WindowHeight - 10; i++)
                Console.WriteLine(new string(' ', Console.WindowWidth));

                // go back to starting position
                Console.Write(SetText.MoveUp(Console.WindowHeight - 10));
                Console.Write(SetText.MoveLeft(Console.WindowWidth));

                // re-enable quickedit
                if (Cli.Core.pid != PlatformID.Unix)
                    Windows.QuickEdit(true);

                // exit the app
                ev.Cancel = true;
                Environment.Exit(0);
            };
        }

        // safely dispose discord rpc
        public static void FixRpc(){
            Console.CancelKeyPress += (s, ev) => {
                // dispose disocrd rpc client
                Utils.DiscordRPC.Deinitialize();
            };
        }
    }
}