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

// handle prename account and change config (runtime only)
if (account.Prename) {
    Config.v.sendPacketsCount =  6;
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
await Sniper.WaitForName(name, delay, account, authResult.loginMethod, useNamesList);
Sniper.Shoot(account, name);

// snipe more if names list is in use
if (useNamesList) {
    // remove sniped name from list and update the file
    if (Config.v.NamesListAutoClean) {
        namesList.Remove(name);
        FileSystem.SaveNames(namesList);
    }

    for (int i = Config.v.NamesListAutoClean ? 0 : 1; i < namesList.Count; i++) {
        await Sniper.WaitForName(namesList[i], delay, account, authResult.loginMethod, true);
        Sniper.Shoot(account, namesList[i]);

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