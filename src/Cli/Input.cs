using DataTypes.SetText;

namespace Cli {
    class Input {
        // small method for requesting input from the user
        public static T Request<T>(string requestMessage, bool hidden=false){
            while(true){
                try {
                    Output.Input(requestMessage);
                    var input = !hidden ? Console.ReadLine() : ReadHidden();
                    T? converted = (T)Convert.ChangeType(input, typeof(T));
                    if(converted != null) return converted;
                    throw new Exception();
                } catch {
                    Output.Error(Errors.ExpectedType("int"));
                }
            }
        }

        public static string ReadHidden(){
            var result = new List<char>();

            ConsoleKeyInfo input;
            while((input = Console.ReadKey()).Key != ConsoleKey.Enter){
                if(input.Key == ConsoleKey.Backspace){
                    result.RemoveAt(result.Count() - 1);
                    Console.Write(SetText.MoveLeft(1) + " " + SetText.MoveLeft(1));
                    continue;
                }

                result.Add(input.KeyChar);
                Console.Write(SetText.MoveLeft(1));
                Console.Write('*');
            }

            // go the next line after enter is pressed
            Console.WriteLine();

            return String.Join("", result);
        }
    }
}