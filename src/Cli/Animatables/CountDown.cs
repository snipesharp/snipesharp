using DataTypes.SetText;

namespace Cli.Animatables
{
    public class CountDown
    {
        public static string DateFromMs(int milliseconds){
            TimeSpan t = TimeSpan.FromMilliseconds(milliseconds);
            return string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms", 
                t.Hours, 
                t.Minutes, 
                t.Seconds, 
                t.Milliseconds);
        }

        private Animatable animation;

        public CountDown() {
            this.animation = new Animatable(1, (frame) => {
                // switch(frame) {
                //     case 0: Console.Write("/"); break;
                //     case 1: Console.Write("—"); break;
                //     case 2: Console.Write("\\"); break;
                //     case 3: Console.Write("|"); break;
                // }
                // Console.Write(SetText.MoveLeft(1));
                // todo
            }, 1);
        }

        public void Cancel() {
            this.animation.Cancel();
        }
    }
}