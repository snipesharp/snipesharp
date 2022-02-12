namespace Cli
{
    public class Core
    {
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
    }
}