using DataTypes.SetText;
using Cli.Templates;

namespace Cli {
    class Input {
        // small method for requeting input from the user
        public static T Request<T>(string requestMessage, bool hidden=false, Func<T, bool> validator=null){
            while(true){
                try {
                    Output.Input(requestMessage);
                    var input = !hidden ? Console.ReadLine() : ReadHidden();
                    T? converted = (T)Convert.ChangeType(input, typeof(T));
                    if(converted == null) throw new Exception(Errors.ExpectedType(typeof(T)));
                    if(validator != null && !validator(converted)) throw new Exception("Invalid format");
                    return converted;
                } catch (Exception error){
                    Output.Error(error.Message);
                }
            }
        }

        public static string ReadHidden(){
            var result = new List<char>();

            ConsoleKeyInfo input;
            while((input = Console.ReadKey()).Key != ConsoleKey.Enter){
                if(input.Key == ConsoleKey.Backspace){
                    if(result.Count() > 0){
                        result.RemoveAt(result.Count() - 1);
                        string inpt = Cli.Core.pid == PlatformID.Unix ? SetText.MoveLeft(1) + " " : " ";
                        Console.Write(inpt + SetText.MoveLeft(1));
                    }
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