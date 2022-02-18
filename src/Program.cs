using FS;
using Cli;
using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Snipe;
using Utils;

// attempt to fix windows cmd colors
if (Core.pid != PlatformID.Unix)
Fix.Windows.FixCmd();

// attempt to fix cursor not showing after close
Fix.TerminateHandler.FixCursor();

// create and load config
Config config = FileSystem.GetConfig().Fix();
FileSystem.SaveConfig(config);

// create and load name list
if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

// create example names file
FileSystem.SaveNames(new List<string> { "example1", "example2" }, "names.example.json");

// clear the console before execution
Console.Clear();
SetText.DisplayCursor(true);

// welcome the user
Output.PrintLogo();

// let the user authenticate
var account = await Core.Auth();

// set name
string namesListAnswer = "No";
var names = new List<string>();
try { names = FileSystem.GetNames(); } catch(System.Text.Json.JsonException e) { Output.Error($"Error while reading {SetText.Red}names.json{SetText.ResetAll}: Invalid value at line {e.LineNumber+1}, column {e.BytePositionInLine}"); }
if (names.Count > 0) namesListAnswer = new SelectionPrompt("Found names in names.json, use the list?", "Yes", "No").result;
string name = namesListAnswer == "No" ? Input.Request<string>("Name to snipe: ") : names[0];

// set delay
long delay = Input.Request<long>("Offset in ms: ");

// wait for name to drop then shoot
await Sniper.WaitForName(name, delay);
Sniper.Shoot(config, account, name);

// snipe more if names list is in use
if (namesListAnswer == "Yes")
{
    for (int i = 1; i < names.Count; i++)
    {
        await Sniper.WaitForName(names[i], delay);
        Sniper.Shoot(config, account, names[i]);
    }
}

// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
Console.ReadKey();