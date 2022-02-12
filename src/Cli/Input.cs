namespace Cli {
    class Input {
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