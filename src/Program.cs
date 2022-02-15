using FS;
using Cli;
using Cli.Templates;
using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;

// attempt to fix windows cmd colors
if(Cli.Core.pid != PlatformID.Unix)
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
    account.Bearer = Input.Request<string>(Requests.Bearer, validator: Validators.Credentials.Bearer);
    Output.Success($"Successfully authenticated");
}
else if (loginMethod == "Mojang Account") {
    account.Email = Input.Request<string>(Requests.Email, validator:Validators.Credentials.Email);
    account.Password = Input.Request<string>(Requests.Password, hidden: true);
    Output.Success($"Successfully authenticated");
}
else {
    var loadedAccount = FileSystem.GetAccount();
    if (loadedAccount.Bearer != null) {
        // todo auth with bearer
    }
    else {
        // todo mojang auth
    }

    Output.Success($"Successfully authenticated");
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

// prepare the sniper
// todo

// inform the user
Output.Inform($"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} at {waitTime}");

// don't exit automatically
Output.Inform("Press any key to continue...");
Console.ReadKey();