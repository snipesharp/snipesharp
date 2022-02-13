using DataTypes.SetText;

namespace Cli.Animatables
{
    public class SelectionPrompt
    {
        private Animatable animation;
        private List<string> options;
        private int currentIndex = 0;
        private int answerIndex = -1;
        public string result = "";

        public SelectionPrompt(string question, params string[] options) {
            this.options = options.ToList();

            // prompt the user
            Console.WriteLine(question);

            // setup animation
            this.animation = new Animatable(1, (frame) => {
                string output = PrintOptions();
                Console.WriteLine(output);
                SetText.DisplayCursor(false);
                Console.Write(SetText.MoveUp(options.Count()));
                Console.Write(SetText.MoveLeft(1000));
            }, 1);

            // block the main thread until the user
            // selects an options
            while(true){
                var input = Console.ReadKey().Key;
                if(input == ConsoleKey.UpArrow) this.currentIndex--;
                if(input == ConsoleKey.DownArrow) this.currentIndex++;
                if(input == ConsoleKey.Enter) this.answerIndex = this.currentIndex;

                // make sure the user can't go out of the range
                if(this.currentIndex < 0) this.currentIndex = 0;
                if(this.currentIndex > options.Count() - 1) this.currentIndex = options.Count() - 1;

                // end the while loop after
                // the user selects an options
                if(this.answerIndex != -1) break;
            }

            // after that clear all lines for new output
            this.animation.Cancel();
            SetText.DisplayCursor(true);
            Console.Write(SetText.MoveLeft(1000));
            var emptyLine = new string(' ', Console.WindowWidth);
            for(int i = 0; i < options.Count(); i++) Console.WriteLine(emptyLine);
            
            // after you clear the console go back
            // to remove emtpy space
            Console.Write(SetText.MoveUp(options.Count()));

            // finally save the result
            this.result = this.options[this.answerIndex];
        }

        public string PrintOptions(){
            var final = new List<string>(); 
            for(int i = 0; i < options.Count(); i++)
            final.Add("> " + (i == currentIndex ? SetText.Blue : "") + options[i] + SetText.ResetAll);
            return String.Join("\n", final);
        }

        public void Close() {
            this.animation.Cancel();
            SetText.DisplayCursor(true);
        }
    }
}