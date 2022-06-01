namespace Utils
{
    public class Snipesharp
    {
        public static string packetResults = "";
        public static DateTime snipeTime = new DateTime();
        public string GetAssemblyVersion() {
            string fullVersion = GetType().Assembly.GetName().Version.ToString();
            return fullVersion.Substring(0, fullVersion.Length - 2);
        }
        public static string GetNameToSnipe() {
            if (!Cli.Core.arguments.ContainsKey("--name")) {
                List<string> argNamesList = FS.FileSystem.GetNames();
                return new Cli.Animatables.SelectionPrompt("What name(s) would you like to snipe?",
                    new string[] {
                        Cli.Templates.TNames.LetMePick,
                        Cli.Templates.TNames.UseNamesJson,
                        Cli.Templates.TNames.ThreeCharNames,
                        Cli.Templates.TNames.PopularNames
                    },
                    new string[] {
                        argNamesList.Count == 0 ? Cli.Templates.TNames.UseNamesJson : "",
                    }
                ).result;
            }
            string nameOption = "";
            switch (Cli.Core.arguments["--name"].data!) {
                case "p": nameOption = Cli.Templates.TNames.PopularNames; break;
                case "l": nameOption = Cli.Templates.TNames.UseNamesJson; break;
                case "3": nameOption = Cli.Templates.TNames.ThreeCharNames; break;
                default: nameOption = Cli.Core.arguments["--name"].data!; break;
            }
            return nameOption;
        }
    }
}