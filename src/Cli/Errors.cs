namespace Cli
{
    public class Errors
    {
        public static string ExpectedType(Type type){
            return $"Input is of incorrect type. Expected {type}";
        }   
    }
}