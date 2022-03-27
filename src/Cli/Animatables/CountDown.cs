using DataTypes.SetText;
using System.Text.RegularExpressions;

namespace Cli.Animatables
{
    public class CountDown
    {
        public static string DateFromMs(long milliseconds){
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);

            string toFormat = t.Days.Equals(0) || t.Days.Equals(null) ? "" : $"{t.Days:D2}d:";
            toFormat += t.Hours.Equals(0) || t.Hours.Equals(null) ? "" : $"{t.Hours:D2}h:";
            toFormat += t.Minutes.Equals(0) || t.Minutes.Equals(null) ? "" : $"{t.Minutes:D2}m:";
            toFormat += $"{t.Seconds:D2}s";

            return string.Format(toFormat);
        }

        private Animatable animation;

        public CountDown(long waitMs, string placeholder="{TIME}") {
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

        public async Task Cancel() {
            this.animation.Cancel();
            SetText.DisplayCursor(true);
        }
    }
}