using FS;
using Cli;
using Utils;
using Snipe;
using DataTypes;
using DataTypes.Auth;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.SetText;

// prepare everything and welcome the user
Initialize();

// let the user authenticate
AuthResult authResult = await Core.Auth();
Account account = authResult.account;

if(!await Stats.CanChangeName(account.Bearer)){
    Cli.Output.ExitError($"{account.MicrosoftEmail} cannot change username yet.");
}

// handle prename account and change config (runtime only)
if (account.Prename) {
    var maxPackets2 = !Convert.ToBoolean(
    new SelectionPrompt("Sniping using a prename account, switch to 2 max packets sent?", 
        new string[] { "Yes [suggested]", "No" }).answerIndex);
    Config.v.SendPacketsCount = maxPackets2 ? 2 : Config.v.SendPacketsCount;
    Output.Inform(TAuth.AuthInforms.NoNameHistory);
}

// fetch names list now to see if they are empty or not
// will be used later if needed
List<string> namesList = FileSystem.GetNames();

// prompt the user for name choices
var nameOption = new SelectionPrompt("What name/s would you like to snipe?",
    new string[] {
        TNames.LetMePick,
        TNames.UseNamesJson,
        TNames.ThreeLetterNames,
        TNames.EnglishNames
    },
    new string[] {
        namesList.Count == 0 ? TNames.UseNamesJson : "",
        TNames.ThreeLetterNames,
        TNames.EnglishNames
    }
).result;

// handle each option individualy
if(nameOption == TNames.LetMePick) await handleSingleName(authResult, account);
if(nameOption == TNames.UseNamesJson) await handleNamesJson(authResult, account, namesList);
if(nameOption == TNames.ThreeLetterNames) await handleThreeLetter(authResult, account);
if(nameOption == TNames.EnglishNames) await handleEnglishNames(authResult, account);

// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
Console.ReadKey();

static void Initialize() {
    // delete latest log file
    if (File.Exists(FileSystem.latestLogFile)) File.Delete(FileSystem.latestLogFile);

    // attempt to fix windows cmd colors
    if (Core.pid != PlatformID.Unix)
    Fix.Windows.FixCmd();

    // attempt to fix cursor not showing after close
    Fix.TerminateHandler.FixCursor();

    // dispose discord rpc on close
    Fix.TerminateHandler.FixRpc();

    // clear the console before execution
    Console.Clear();
    SetText.DisplayCursor(true);

    // welcome the user
    Output.PrintLogo();

    // create and load config
    FileSystem.PrepareConfig();

    // create and load name list
    if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

    // create example names file
    FileSystem.SaveNames(new List<string> { "example1", "example2", "example3" }, "names.example.json");

    // start discord rpc
    Utils.DiscordRPC.Initialize();
}


static async Task<long> GetDelay(){
    var suggestedOffset = await Offset.CalcSuggested();
    return Input.Request<long>($"Offset in ms [suggested: {suggestedOffset}ms]: ");
}

static async Task handleSingleName(AuthResult authResult, Account account){
    string name = Input.Request<string>("Name to snipe: ");
    long delay = await GetDelay();
    var dropTime = Math.Max(0, await Droptime.GetMilliseconds(name, true) - delay);
    Sniper.WaitForName(name, dropTime, account, authResult.loginMethod);
    Sniper.Shoot(account, name);
}

static async Task handleNamesJson(AuthResult authResult, Account account, List<string> namesList){
    long delay = await GetDelay();
    for (int i = 0; i < namesList.Count; i++) {
        var dropTime = Math.Max(0, await Droptime.GetMilliseconds(namesList[i], false) - delay);
        if(dropTime > 0){
            Sniper.WaitForName(namesList[i], dropTime, account, authResult.loginMethod);
            Sniper.Shoot(account, namesList[i]);
        }

        // remove sniped name from list and update the file
        if (Config.v.NamesListAutoClean) {
            namesList = FileSystem.GetNames();
            namesList.Remove(namesList[i--]);
        }
        FileSystem.SaveNames(namesList);
    }
}

static async Task handleThreeLetter(AuthResult authResult, Account account){
    // todo
}

static async Task handleEnglishNames(AuthResult authResult, Account account){
    // todo
}