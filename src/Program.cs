using FS;
using Cli;
using Utils;
using DataTypes;
using DataTypes.Auth;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.SetText;
using Cli.Names;
			
HandleArgs();

// prepare everything and welcome the user
Initialize();

// let the user authenticate
AuthResult authResult = await Core.Auth();

if(!Account.v.prename && authResult.loginMethod != "Bearer Token") if (!await Stats.CanChangeName(Account.v.Bearer)) Cli.Output.ExitError($"{Account.v.MicrosoftEmail} cannot change username yet.");
string? username = await Utils.Stats.GetUsername(Account.v.Bearer);

// handle prename account and change config (runtime only)
if (Account.v.prename) {
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

// first time setup
if (DataTypes.Config.v.firstTime) Cli.Output.Inform(Cli.Templates.TFileSystem.FSInforms.Names);

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
if(nameOption == TNames.LetMePick) await Names.handleSingleName(authResult);
if(nameOption == TNames.UseNamesJson) await Names.handleNamesList(authResult, namesList);
if(nameOption == TNames.ThreeCharNames) await Names.handleThreeLetter(authResult);

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

    // load account config
    FileSystem.PrepareAccount();
    
    // create and load name list
    if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

    // create example names file
    FileSystem.SaveNames(new List<string> { "example1", "example2", "example3" }, "example.names.json");

    // start discord rpc
    if (Config.v.EnableDiscordRPC) Utils.DiscordRPC.Initialize();
}

static async void HandleArgs() {
    string argName = "";
    if(Core.arguments.ContainsKey("--name")) argName = Core.arguments["--name"].data!;

    if(Core.arguments.ContainsKey("--email") && Core.arguments.ContainsKey("--password")){
        Config.v.EnableDiscordRPC = false;
        Config.v.ShowTargetNameDRPC = false;
        Config.v.ShowUsernameDRPC = false;
        Account.v.MicrosoftEmail = Core.arguments["--email"].data!;
        Account.v.MicrosoftPassword = Core.arguments["--password"].data!;
        Initialize();

        string? username = await Utils.Stats.GetUsername(Account.v.Bearer);
        if (!String.IsNullOrEmpty(username)) Console.Title = $"snipesharp - Logged in as {username}";

        var temp = new AuthResult {
            loginMethod = TAuth.AuthOptions.Microsoft
        };
        if (argName == "l" || argName == "list") await Names.handleNamesList(temp, FileSystem.GetNames());
        if (argName == "3" || argName == "3char") await Names.handleThreeLetter(temp);
        await Names.handleSingleName(temp, argName);
        Console.ReadKey();
        return;
    }

    if(Core.arguments.ContainsKey("--bearer")){
        Config.v.EnableDiscordRPC = false;
        Config.v.ShowTargetNameDRPC = false;
        Config.v.ShowUsernameDRPC = false;
        Account.v.Bearer = Core.arguments["--bearer"].data!;
        Initialize();

        string? username = await Utils.Stats.GetUsername(Account.v.Bearer);
        if (!String.IsNullOrEmpty(username)) Console.Title = $"snipesharp - Logged in as {username}";

        var temp = new AuthResult {
            loginMethod = TAuth.AuthOptions.Microsoft
        };
        await Names.handleThreeLetter(temp);
        Console.ReadKey();
        return;
    }
}