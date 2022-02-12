namespace Cli {
    class Input {
        public static Dictionary<string, Argument> arguments = ParseArgs();

        // parse the command line arguments and convert them to a dictionary
        public static Dictionary<string, Argument> ParseArgs(){
            List<string> args = Environment.GetCommandLineArgs().Skip(1).ToList();
            var parsedArguments = new Dictionary<string, Argument>();
            
            args.ForEach(arg => {
                var parsed = new Argument(arg);
                parsedArguments[parsed.name] = parsed;
            });

            return parsedArguments;
        }

        // small method for requesting input from the user
        public static T Request<T>(string requestMessage){
            while(true){
                try {
                    Cli.Output.Input(requestMessage);
                    var input = Console.ReadLine();
                    T? converted = (T)Convert.ChangeType(input, typeof(T));
                    if(converted != null) return converted;
                    throw new Exception();
                } catch {
                    Cli.Output.Error(Cli.Errors.ExpectedType("int"));
                }
            }
        }
    }
}