using FS;
using Cli;
using Cli.Templates;
using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Snipe;

// create and load config
Config config = FileSystem.GetConfig();
FileSystem.SaveConfig(config);

// attempt to fix windows cmd colors
if (Cli.Core.pid != PlatformID.Unix)
WindowsFix.FixCmd();

// clear the console before execution
Console.Clear();
SetText.DisplayCursor(true);

// welcome the user
Output.PrintLogo();

// display prompt
string loginMethod = FileSystem.AccountFileExists()
    ? new SelectionPrompt("Login method: ", "From previous session", "Bearer Token", "Mojang Account").result
    : new SelectionPrompt("Login method: ", "Bearer Token", "Mojang Account").result;

// obtain login info based on login method choice
var account = new Account();
if (loginMethod == "Bearer Token") {
    account.Bearer = Input.Request<string>(Requests.Bearer);
    var spinnerAuth = new Spinner();
    spinnerAuth.Cancel();
    if (await Auth.AuthWithBearer(account.Bearer))
    {
        Output.Success($"Successfully authenticated");
        Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
    }
    else
    {
        Output.ExitError("Failed to authenticate using bearer");
    }
}
else if (loginMethod == "Mojang Account") {
    account.Email = Input.Request<string>(Requests.Email, validator:Validators.Credentials.Email);
    account.Password = Input.Request<string>(Requests.Password, hidden: true);
    // todo mojang auth 
    Output.Inform($"Not authenticated (Mojang login not implemented)");
}
else {
    var loadedAccount = FileSystem.GetAccount();
    if (loadedAccount.Bearer != null) {
        var spinnerAuth = new Spinner();
        spinnerAuth.Cancel();
        if (await Auth.AuthWithBearer(loadedAccount.Bearer))
        {
            Output.Success($"Successfully authenticated");
            Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
            account.Bearer = loadedAccount.Bearer;
        }
        else
        {
            Output.ExitError("Failed to authenticate using bearer");
        }
    }
    else {
        // todo mojang auth
        Output.Inform($"Not authenticated (Mojang login not implemented)");
    }
}

// save account
if (loginMethod != "From previous session") FileSystem.SaveAccount(account);

// require initial information
string name = Input.Request<string>("Name to snipe: ");
long delay = Input.Request<long>("Custom delay in ms: ");

// calculate total wait time
var spinner = new Spinner();
var waitTime = await Snipe.Droptime.GetMilliseconds(name) - delay;
spinner.Cancel();

// countdown animation
var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

// actually wait for the wait time
Console.WriteLine(waitTime);
Thread.Sleep(TimeSpan.FromMilliseconds(waitTime));
countDown.Cancel();

for (int i = 0; i < config.SendPacketsCount; i++)
{
    await ChangeName.Change(name, account.Bearer);
    Thread.Sleep(config.MillisecondsBetweenPackets);
}

// don't exit automatically
Output.Inform("Press any key to continue...");
Console.ReadKey();