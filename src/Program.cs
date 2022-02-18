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
List<string> names = FileSystem.GetNames();
FileSystem.SaveNames(names);

// create example names file
FileSystem.SaveNames(new List<string> { "example1", "example2" }, "names.example.json");

// clear the console before execution
Console.Clear();
SetText.DisplayCursor(true);

// welcome the user
Output.PrintLogo();

// let the user authenticate
var account = await Core.Auth();

// require initial information
string name = Input.Request<string>("Name to snipe: ");
long delay = Input.Request<long>("Offset in ms: ");

// calculate total wait time
var spinner = new Spinner();
var waitTime = Math.Max(await Snipe.Droptime.GetMilliseconds(name) - delay, 0);
spinner.Cancel();

// countdown animation
var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

// actually wait for the right time
Thread.Sleep(TimeSpan.FromMilliseconds(waitTime));
countDown.Cancel();

// perform name sniping
var success = false;
for (int i = 0; (i < config.sendPacketsCount && !success); i++) {
    success = (int)ChangeName.Change(name, account.Bearer).Result.StatusCode == 200;
    Thread.Sleep(config.PacketSpreadMs);
}

// post success
if (success) {
    Webhook.SendDiscordWebhooks(config, name);
    ChangeSkin.Change(config.SkinUrl, config.SkinType, account.Bearer);
}

// don't exit automatically
Output.Inform("Press any key to continue...");
Console.ReadKey();