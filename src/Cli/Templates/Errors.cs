using DataTypes.SetText;

namespace Cli.Templates
{
    public class Errors
    {
        public static Func<Type, string> ExpectedType = type => $"Input is of incorrect type. Expected {type}";
        public static Func<string, string> NoDroptime = username => $"Couldn't find droptime for {SetText.Red}{username}{SetText.ResetAll}";
    }
}