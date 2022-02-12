namespace Cli
{
    public class Errors
    {
        public static string ExpectedType(string type){
            return $"Input is of incorrect type. Expected {type}";
        }   
    }
}