using FS;
using Cli;
using Utils;
using Snipe;
using DataTypes;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.SetText;

// prepare everything and welcome the user
Initialize();

// let the user authenticate
var authResult = await Core.Auth();
Account account = authResult.account;
String loginMethod = authResult.loginMethod;

if(!await Stats.CanChangeName(account.Bearer)){
    Cli.Output.ExitError($"{account.MicrosoftEmail} cannot change username yet.");
}

// handle prename account and change config (runtime only)
if (account.Prename) {
    var maxPackets2 = !Convert.ToBoolean(
    new SelectionPrompt("Sniping using a prename account, switch to 2 max packets sent?", "Yes", "No").answerIndex);
    Config.v.sendPacketsCount = maxPackets2 ? 2 : Config.v.sendPacketsCount;
    Output.Inform(TAuth.AuthInforms.NoNameHistory);
}

// handle names.json file
var useNamesList = false;
var namesList = FileSystem.GetNames();
if (namesList.Count > 0) useNamesList = !Convert.ToBoolean(
    new SelectionPrompt("Found names in names.json, use the list?", "Yes", "No").answerIndex);

// set name to either first of list or prompted
string name = !useNamesList ? Input.Request<string>("Name to snipe: ") : namesList[0];

// prompt for delay (offset)
var suggestedOffset = await Offset.CalcSuggested();
long delay = Input.Request<long>($"Offset in ms [suggested: {suggestedOffset}ms]: ");

// wait for name to drop then shoot
var dropTime = Math.Max(0, await Droptime.GetMilliseconds(name, !useNamesList) - delay);
if(dropTime > 0){
    Sniper.WaitForName(name, dropTime, account, authResult.loginMethod);
    Sniper.Shoot(account, name);
}

// snipe more if names list is in use
if (useNamesList) {
    // remove sniped name from list and update the file
    if (Config.v.NamesListAutoClean) {
        namesList.Remove(name);
        FileSystem.SaveNames(namesList);
    }

    for (int i = Config.v.NamesListAutoClean ? 0 : 1; i < namesList.Count; i++) {
        dropTime = Math.Max(0, await Droptime.GetMilliseconds(namesList[i], !useNamesList) - delay);
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

// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
Console.ReadKey();

static void Initialize() {
    // delete previous log file
    if (FileSystem.LogFileExists()) File.Delete(FileSystem.logFile);

    // attempt to fix windows cmd colors
    if (Core.pid != PlatformID.Unix)
    Fix.Windows.FixCmd();

    // attempt to fix cursor not showing after close
    Fix.TerminateHandler.FixCursor();

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
}