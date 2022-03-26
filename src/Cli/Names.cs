using FS;
using Snipe;
using Utils;
using DataTypes;
using DataTypes.Auth;

namespace Cli.Names
{
    public class Names
    {
        static async Task<long> GetDelay(){
            var suggestedOffset = await Offset.CalcSuggested();
            if(Core.arguments.ContainsKey("--offset")) { 
                if (int.TryParse(Core.arguments["--offset"].data!, out int offsetArg)) {
                    Cli.Output.Inform($"Offset set to {offsetArg}");
                    return offsetArg;
                }
                Cli.Output.Error($"{Core.arguments["--offset"].data!} is not a valid offset value");
            }
            if(Core.arguments.ContainsKey("--bearer")) {
                Cli.Output.Inform($"Offset set to {suggestedOffset}");
                return suggestedOffset;
            }
            if(Core.arguments.ContainsKey("--email") && Core.arguments.ContainsKey("--password")) {
                Cli.Output.Inform($"Offset set to {suggestedOffset}");
                return suggestedOffset;
            }
            if (DataTypes.Config.v.firstTime) Cli.Output.Inform(Cli.Templates.TFileSystem.FSInforms.OffsetExplanation);
            return Input.Request<long>($"Offset in ms [suggested: {suggestedOffset}ms]: ");
        }

        public static async Task handleSingleName(AuthResult authResult, string name=""){
            if (string.IsNullOrEmpty(name)) name = Input.Request<string>("Name to snipe: ");
            long delay = await GetDelay();
            FS.FileSystem.Log($"Offset set to {delay}ms");
            var dropTime = Math.Max(0, await Droptime.GetMilliseconds(name, true) - delay);
            Sniper.WaitForName(name, dropTime, authResult.loginMethod);
            Sniper.Shoot(name);
        }

        public static async Task handleNamesList(AuthResult authResult, List<string> namesList, bool fromJsonFile=true){
            long delay = await GetDelay();
            FS.FileSystem.Log($"Offset set to {delay}ms");
            for (int i = 0; i < namesList.Count; i++) {
                var dropTime = Math.Max(0, await Droptime.GetMilliseconds(namesList[i], false) - delay);
                if(dropTime > 0){
                    Sniper.WaitForName(namesList[i], dropTime, authResult.loginMethod);
                    Sniper.Shoot(namesList[i]);
                }

                // remove sniped name from list and update the file
                if (Config.v.NamesListAutoClean) {
                    if(fromJsonFile) namesList = FileSystem.GetNames();
                    namesList.Remove(namesList[i--]);
                }
                if(fromJsonFile) FileSystem.SaveNames(namesList);
            }
        }

        public static async Task handleThreeLetter(AuthResult authResult){
            var scraped = await Scrape.Get3LetterNames();
            await handleNamesList(authResult, scraped, false);
        }
    }
}