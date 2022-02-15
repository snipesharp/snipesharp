using FS;
using Cli;
using Cli.Templates;
using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Snipe;

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
    string result = await Auth.AuthWithBearer(account.Bearer);
    spinnerAuth.Cancel();
    if (String.IsNullOrEmpty(result))
    {
        Output.ExitError("Failed to authenticate using bearer");
    }
    else
    {
        Output.Success($"Successfully authenticated");
        Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
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
        string result = await Auth.AuthWithBearer(loadedAccount.Bearer);
        spinnerAuth.Cancel();
        if (String.IsNullOrEmpty(result))
        {
            Output.ExitError("Failed to authenticate using bearer");
        }
        else
        {
            Output.Success($"Successfully authenticated");
            Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
            account.Bearer = loadedAccount.Bearer;
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
int delay = Input.Request<int>("Custom delay in ms: ");

// calculate total wait time
var spinner = new Spinner();
var waitTime = await Snipe.Droptime.GetMilliseconds(name) - delay;
spinner.Cancel();

// countdown animation
var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

// actually wait for the wait time
Thread.Sleep(waitTime);
countDown.Cancel();

// todo do the actual sniping here
Output.Inform("Proceeding with sniping...");

// don't exit automatically
Output.Inform("Press any key to continue...");
await ChangeName.Change(name, account.Bearer);
Console.ReadKey();