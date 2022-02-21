using FS;
using System.Text.Json;
using DataTypes.SetText;

namespace Cli.Templates
{
    public class TFileSystem
    {
        public struct FSInforms {
            public static string ConfigSetup = $"All config files can be found in \"{DataTypes.SetText.SetText.Blue}{FileSystem.snipesharpFolder}{DataTypes.SetText.SetText.White}\"";
            public static string AccountSafety = $"This file contains sensitive information below line 50, don't scroll if someone can see the contents of this file\n";
            public static Func<Tuple<string, JsonException>, string> CannotReadFile = t => $"Error while reading {SetText.Red}{t.Item1}{SetText.ResetAll}: Invalid value at line {t.Item2.LineNumber + 1}, column {t.Item2.BytePositionInLine}";
        }
    }
}