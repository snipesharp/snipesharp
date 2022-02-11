namespace Cli
{
    public class Argument {
        public string name;
        public string? data;

        public Argument(string arg){
            var splited = arg.Split("=");

            // save the name of the argument
            this.name = splited[0];

            // if it has data (Ex: output=1)
            if(splited.Length > 1) this.data = String.Join("=", splited.Skip(1));
        }
    }
}