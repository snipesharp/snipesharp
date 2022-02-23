using DataTypes.SetText;

namespace Cli.Templates
{
    public class TErrors
    {
        public static Func<Type, string> ExpectedType = type => $"Input is of incorrect type. Expected {type}";
        public static Func<string, string> NoDroptime = username => $"Couldn't find droptime for {SetText.Red}{username}{SetText.ResetAll}";
        public static string PressAnyKey = "Press any key to continue...";
        public static string NoNameHistory = "Account has no name history.";
        public static string CannotConnectAPI = "Cannot connect to api.mojang.com";
    }
}