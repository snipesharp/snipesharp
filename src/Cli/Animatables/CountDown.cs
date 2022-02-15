using DataTypes.SetText;
using System.Text.RegularExpressions;

namespace Cli.Animatables
{
    public class CountDown
    {
        public static string DateFromMs(int milliseconds){
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s", 
                t.Hours, 
                t.Minutes, 
                t.Seconds);
        }

        private Animatable animation;

        public CountDown(int waitMs, string placeholder="{TIME}") {
            SetText.DisplayCursor(false);
            this.animation = new Animatable(1, (frame) => {
                // clear the current line
                Console.Write(SetText.MoveLeft(1000) + new string(' ', Console.WindowWidth) + SetText.MoveLeft(1000));

                // prepare message
                string timeLeft = DateFromMs(waitMs);
                string msg = Regex.Replace(placeholder, @"{TIME}", timeLeft);

                // print
                Console.Write(msg);
                Console.Write(SetText.MoveLeft(1000));

                // fix weird windows only bug
                Console.WriteLine();
                Console.Write(SetText.MoveUp(1));
                
                waitMs -= 1000;
            }, 1000);
        }

        public void Cancel() {
            this.animation.Cancel();
            SetText.DisplayCursor(true);
        }
    }
}