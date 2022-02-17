using DataTypes.SetText;

namespace Fix
{
    public class TerminateHandler
    {
        // makes it so that the console cursor appears
        // after snipe craft is closed
        public static void FixCursor(){
            Console.CancelKeyPress += (s, ev) =>
            {
                SetText.DisplayCursor(true);
                ev.Cancel = true;
                Environment.Exit(0);
            };
        }
    }
}