using FS;
using Cli;
using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Snipe;
using Utils;

// delete previous log file
if (FileSystem.LogFileExists()) File.Delete(FileSystem.GetLatestLogPath());

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
Config config = FileSystem.GetConfig().Fix();
FileSystem.SaveConfig(config);

// create and load name list
if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

// create example names file
FileSystem.SaveNames(new List<string> { "example1", "example2" }, "names.example.json");

// let the user authenticate
var authResult = await Core.Auth();
var account = authResult.account;

// set name
string namesListAnswer = "No";
var names = new List<string>();
try { names = FileSystem.GetNames(); } catch(System.Text.Json.JsonException e) { Output.Error($"Error while reading {SetText.Red}names.json{SetText.ResetAll}: Invalid value at line {e.LineNumber+1}, column {e.BytePositionInLine}"); }
if (names.Count > 0) namesListAnswer = new SelectionPrompt("Found names in names.json, use the list?", "Yes", "No").result;
if (namesListAnswer == "Yes") names = FileSystem.GetNames();
string name = namesListAnswer == "No" ? Input.Request<string>("Name to snipe: ") : names[0];

// calculate suggested offset
var suggestedOffset = await Offset.CalcSuggested();

// require initial information
long delay = Input.Request<long>($"Offset in ms [suggested: {suggestedOffset}ms]: ");

// wait for name to drop then shoot
await Sniper.WaitForName(name, delay, namesListAnswer == "Yes");
Sniper.Shoot(config, account, name);

// snipe more if names list is in use
if (namesListAnswer == "Yes") {
    // remove sniped name from list and update the file
    if (config.NamesListAutoClean) {
        names = FileSystem.GetNames(); // update from file before updating to file
        names.Remove(name);
    }
    FileSystem.SaveNames(names);

    for (int i = config.NamesListAutoClean ? 0 : 1; i < names.Count; i++) {
        if (authResult.loginMethod == "Microsoft Account") {
            account.Bearer = await Snipe.Auth.AuthMicrosoft(account.MicrosoftEmail, account.MicrosoftPassword);
            FileSystem.SaveAccount(account);
        }
        await Sniper.WaitForName(names[i], delay, true);
        Sniper.Shoot(config, account, names[i]);

        // remove sniped name from list and update the file
        if (config.NamesListAutoClean) {
            names = FileSystem.GetNames();
            names.Remove(names[i]);
            i--;
        }
        FileSystem.SaveNames(names);
    }
}

// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
Console.ReadKey();