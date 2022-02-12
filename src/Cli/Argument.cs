namespace Cli
{
    public class Argument {
        public string name;
        public string? data;

        public Argument(string arg){
            var splitted = arg.Split("=");

            // save the name of the argument
            this.name = splitted[0];

            // if it has data (Ex: output=1)
            if(splitted.Length > 1) this.data = String.Join("=", splitted.Skip(1));
        }
    }
}
