using DataTypes.SetText;

namespace Cli.Animatables
{
    public class Spinner
    {
        Animatable animation;

        public Spinner() {
            this.animation = new Animatable(4, (frame) => {
                switch(frame) {
                    case 0: Console.Write("/"); break;
                    case 1: Console.Write("—"); break;
                    case 2: Console.Write("\\"); break;
                    case 3: Console.Write("|"); break;
                }
                SetText.DisplayCursor(false);
                Console.Write(SetText.MoveLeft(1));
            }, 300);
        }

        public void Cancel() {
            this.animation.Cancel();
        }
    }
}