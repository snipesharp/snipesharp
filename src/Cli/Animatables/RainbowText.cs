using DataTypes.SetText;

namespace Cli.Animatables
{
    public class RainbowText
    {
        private Animatable animation;

        public RainbowText(string text) {
            this.animation = new Animatable(5, (frame) => {
                Console.Write(this.Colorize(text));
                SetText.DisplayCursor(false);
                Console.Write(SetText.MoveLeft(text.Length));
            }, 300);
        }

        private string Colorize(string text) {
            // convert to string list
            List<string> splitted = text.ToCharArray().Select( c => c.ToString()).ToList();

            // insert random colors in between chars
            var colors = new string[] { SetText.Blue, SetText.Cyan, SetText.DarkBlue, SetText.White };
            Random rand = new Random();
            for(int i = text.Length - 1; i >= 0; i--)
            splitted.Insert(i, colors[rand.Next(0, colors.Length)]);
            
            // join back to string
            return String.Join("", splitted) + SetText.ResetAll;
        }

        public void Cancel() {
            this.animation.Cancel();
            SetText.DisplayCursor(true);
            Console.WriteLine();
        }
    }
}