using FS;
using Cli;
using Utils;
using DataTypes;
using DataTypes.Auth;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.SetText;
using Cli.Names;
			
if(Core.arguments.ContainsKey("--email") && Core.arguments.ContainsKey("--password")){
    Config.v.EnableDiscordRPC = false;
    Initialize();
    var temp = new AuthResult {
        account = await Core.HandleMicrosoft(new Account() {
            MicrosoftEmail = Core.arguments["--email"].data!,
            MicrosoftPassword = Core.arguments["--password"].data!
        }, 1),
        loginMethod = TAuth.AuthOptions.Microsoft
    };
    await Names.handleThreeLetter(temp, temp.account);
    Console.ReadKey();
    return;
}

if(Core.arguments.ContainsKey("--bearer")){
    Config.v.EnableDiscordRPC = false;
    Initialize();
    var temp = new AuthResult {
        account = await Core.HandleBearer(new Account() {
            Bearer = Core.arguments["--bearer"].data!,
        }, 1),
        loginMethod = TAuth.AuthOptions.Microsoft
    };
    await Names.handleThreeLetter(temp, temp.account);
    Console.ReadKey();
    return;
}

// prepare everything and welcome the user
Initialize();

// let the user authenticate
AuthResult authResult = await Core.Auth();
Account account = authResult.account;

if(!account.prename) if (!await Stats.CanChangeName(account.Bearer)) Cli.Output.ExitError($"{account.MicrosoftEmail} cannot change username yet.");
string? username = await Utils.Stats.GetUsername(account.Bearer);

// handle prename account and change config (runtime only)
if (account.prename) {
    var maxPackets2 = !Convert.ToBoolean(
    new SelectionPrompt("Sniping using a prename account, switch to 2 max packets sent?", 
        new string[] { "Yes [suggested]", "No" }).answerIndex);
    Config.v.SendPacketsCount = maxPackets2 ? 2 : Config.v.SendPacketsCount;
    Output.Inform(TAuth.AuthInforms.NoNameHistory);
    Console.Title = $"snipesharp - Logged in with a prename account";
}
else if (!String.IsNullOrEmpty(username)) { 
    Console.Title = $"snipesharp - Logged in as {username}";
    if (Config.v.ShowUsernameDRPC) Utils.DiscordRPC.SetDescription($"Logged in as {username}");
}

// fetch names list now to see if they are empty or not
// will be used later if needed
List<string> namesList = FileSystem.GetNames();

// prompt the user for name choices
var nameOption = new SelectionPrompt("What name(s) would you like to snipe?",
    new string[] {
        TNames.LetMePick,
        TNames.UseNamesJson,
        TNames.ThreeCharNames,
    },
    new string[] {
        namesList.Count == 0 ? TNames.UseNamesJson : "",
    }
).result;

// handle each option individualy
if(nameOption == TNames.LetMePick) await Names.handleSingleName(authResult, account);
if(nameOption == TNames.UseNamesJson) await Names.handleNamesList(authResult, account, namesList);
if(nameOption == TNames.ThreeCharNames) await Names.handleThreeLetter(authResult, account);

// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
Console.ReadKey();

static void Initialize() {
    // set console window title
    Console.Title = "snipesharp";

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

    // create, load and log config
    FileSystem.PrepareConfig();
    FS.FileSystem.Log("Using config: " + System.Text.Json.JsonSerializer.Serialize(Config.v, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
    
    // create and load name list
    if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

    // create example names file
    FileSystem.SaveNames(new List<string> { "example1", "example2", "example3" }, "example.names.json");

    // start discord rpc
    if (Config.v.EnableDiscordRPC) Utils.DiscordRPC.Initialize();
}