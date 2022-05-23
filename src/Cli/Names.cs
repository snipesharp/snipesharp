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
                if (new string[] {"auto", "suggested"}.Contains(Core.arguments["--offset"].data!)) {
                    Cli.Output.Inform($"Offset set to {suggestedOffset}");
                    return suggestedOffset; 
                }
                if (int.TryParse(Core.arguments["--offset"].data!, out int offsetArg)) {
                    Cli.Output.Inform($"Offset set to {offsetArg}");
                    return offsetArg;
                }
                Cli.Output.Error($"{Core.arguments["--offset"].data!} is not a valid offset value");
            }
            if (DataTypes.Config.v.firstTime) Cli.Output.Inform(Cli.Templates.TFileSystem.FSInforms.OffsetExplanation);
            long offset = Input.Request<long>($"Offset in ms [suggested: {suggestedOffset}ms]: ", false, null!, true);
            Config.v.offset = offset;
            return offset;
        }

        public static async Task handleSingleName(AuthResult authResult, string name="", bool exitOnError=true){
            if (string.IsNullOrEmpty(name)) name = Input.Request<string>("Name to snipe: ");
            long delay = await GetDelay();
            FS.FileSystem.Log($"Offset set to {delay}ms");
            var dropTime = Math.Max(0, await Droptime.GetMilliseconds(name, exitOnError) - delay);
            
            // print droptime and subtract 5ms because for SOME reason it shows a second late but it doesnt in WaitForName
            Cli.Output.Inform($"{DataTypes.SetText.SetText.Blue}{name}{DataTypes.SetText.SetText.ResetAll} drops @ {DataTypes.SetText.SetText.Blue}{DateTime.Now.AddMilliseconds(dropTime - 5).ToString()}{DataTypes.SetText.SetText.ResetAll}");

            Sniper.WaitForName(name, dropTime, authResult.loginMethod);
            Sniper.Shoot(name);
        }

        public static async Task handleNamesList(AuthResult authResult, List<string> namesList, bool fromJsonFile=true){
            long delay = await GetDelay();
            FS.FileSystem.Log($"Offset set to {delay}ms");
            for (int i = 0; i < namesList.Count; i++) {
                var dropTime = Math.Max(0, await Droptime.GetMilliseconds(namesList[i], false));

                // print droptime
                Cli.Output.Inform($"{DataTypes.SetText.SetText.Blue}{namesList[i]}{DataTypes.SetText.SetText.ResetAll} drops @ {DataTypes.SetText.SetText.Blue}{DateTime.Now.AddMilliseconds(dropTime - 5).ToString()}{DataTypes.SetText.SetText.ResetAll}");
            
                if(dropTime > 0){
                    Sniper.WaitForName(namesList[i], dropTime - delay, authResult.loginMethod);
                    Sniper.Shoot(namesList[i]); // awaiting doesnt fix starting a new name before all packets are printed
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

        public static async Task handlePopularNames(AuthResult authResult){
            while (true) await handleSingleName(authResult, await Scrape.getPopularName(), false);
        }
    }
}