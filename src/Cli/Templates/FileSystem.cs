using FS;
using System.Text.Json;
using DataTypes.SetText;

namespace Cli.Templates
{
    public class TFileSystem
    {
        public struct FSInforms {
            public static string ConfigSetup = $"Welcome to snipesharp! All config files can be found in \"{DataTypes.SetText.SetText.Blue}{FileSystem.snipesharpFolder}{DataTypes.SetText.SetText.White}\"";
            public static string SelectionPromptUsage = $"You're currently in a 'Selection Prompt', you can navigate it by using the {DataTypes.SetText.SetText.Blue}arrow keys{DataTypes.SetText.SetText.ResetAll} on your keyboard. Press {DataTypes.SetText.SetText.Blue}Enter{DataTypes.SetText.SetText.ResetAll} when you've chosen your option.";
            public static string OffsetExplanation = "The offset is a number which determines how many milliseconds earlier to start sniping name(s). It is suggested to use the suggested value";
            public static string Names = $"You must now choose which name(s) to snipe. Since this is your first time using snipesharp and you haven't set up a name list yet, you can only choose from the other 2 options. To setup a name list follow this simple tutorial: {DataTypes.SetText.SetText.Blue}https://snipesharp.xyz/namelists{DataTypes.SetText.SetText.ResetAll}";
            public static string AccountSafety = $"This file contains sensitive information below line 50, don't scroll if someone can see the contents of this file\n";
            public static Func<Tuple<string, JsonException>, string> CannotReadFile = t => $"Error while reading {SetText.Red}{t.Item1}{SetText.ResetAll}: Invalid value at line {t.Item2.LineNumber + 1}, column {t.Item2.BytePositionInLine}";
        }
    }
}