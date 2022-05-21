using FS;
using Cli;
using Utils;
using DataTypes;
using DataTypes.Auth;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.SetText;
using Cli.Names;
using Snipe;

// get and log snipesharp version
string currentVersion = new Snipesharp().GetAssemblyVersion();

// prepare everything and welcome the user
await Initialize(currentVersion);

// handle args
await HandleArgs(currentVersion);

// let the user authenticate
AuthResult authResult = await Core.Auth();

if(!Account.v.prename && authResult.loginMethod != "Bearer Token") if (!await Stats.CanChangeName(Account.v.Bearer)) Cli.Output.ExitError($"{Account.v.MicrosoftEmail} cannot change username yet.");
string? username = await Utils.Stats.GetUsername(Account.v.Bearer);

// handle prename account and change configs
HandleAccountType(currentVersion, username);

// fetch names list now to see if they are empty or not
// will be used later if needed
List<string> namesList = FileSystem.GetNames();

// test rate limiting
if (Core.arguments.ContainsKey("--test-rl")) await TestRatelimit();

// first time setup
if (DataTypes.Config.v.firstTime) Cli.Output.Inform(Cli.Templates.TFileSystem.FSInforms.Names);

// prompt the user for name choices
var nameOption = Snipesharp.GetNameToSnipe();

// snipe periodically
if (Config.v.interval != null) await Sniper.ShootPeriodically(nameOption);

// handle each option individualy
if (nameOption == TNames.LetMePick) await Names.handleSingleName(authResult);
if (nameOption == TNames.UseNamesJson) await Names.handleNamesList(authResult, namesList);
if (nameOption == TNames.ThreeCharNames) await Names.handleThreeLetter(authResult);
if (nameOption == TNames.PopularNames) await Names.handlePopularNames(authResult);
if (
    nameOption != TNames.PopularNames && nameOption != TNames.LetMePick &&
    nameOption != TNames.UseNamesJson && nameOption != TNames.ThreeCharNames
) await Names.handleSingleName(authResult, nameOption);


// don't exit automatically
Output.Inform("Finished sniping, press any key to exit");
if (Core.pid != PlatformID.Unix)
    Fix.Windows.QuickEdit(true);
Console.ReadKey();

static async Task Initialize(string currentVersion) {

    // delete snipesharp.old
    try { File.Delete("snipesharp.old"); } catch (Exception e) { FS.FileSystem.Log(e.ToString()); }

    // set console window title
    Console.Title = $"snipesharp {currentVersion}";

    // delete latest log file
    if (File.Exists(FileSystem.latestLogFile)) File.Delete(FileSystem.latestLogFile);

    // log verison
    FS.FileSystem.Log($"Running version: {currentVersion}");

    // attempt to fix windows cmd colors
    if (Core.pid != PlatformID.Unix)
    Fix.Windows.FixCmd();

    // disable quick edit
    if (Core.pid != PlatformID.Unix)
    Fix.Windows.QuickEdit(false);

    // attempt to fix cursor not showing after close
    Fix.TerminateHandler.FixCursor();

    // dispose discord rpc on close
    Fix.TerminateHandler.FixRpc();

    // clear the console before execution
    Console.Clear();
    SetText.DisplayCursor(true);

    // welcome the user
    Output.PrintLogo();
    if (Config.v.firstTime) Cli.Output.Inform(TFileSystem.FSInforms.ConfigSetup);

    // create, load and log config
    FileSystem.PrepareConfig();
    FS.FileSystem.Log("Using config: " + System.Text.Json.JsonSerializer.Serialize(Config.v, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));

    // install snipesharp
    if (Core.arguments.ContainsKey("--install")) Snipesharp.Install();

    // execute auto update
    if (!Core.arguments.ContainsKey("--disable-auto-update")) FS.FileSystem.Log(await Utils.AutoUpdate.Update(currentVersion));

    // load account config
    FileSystem.PrepareAccount();
    
    // create and load name list
    if (!FileSystem.NamesFileExists()) FileSystem.SaveNames(new List<string>());

    // create example names file
    FileSystem.SaveNames(new List<string> { "example1", "example2", "example3" }, "example.names.json");

    // disable-discordrpc arg
    if (Core.arguments.ContainsKey("--disable-discordrpc")) {
        Config.v.EnableDiscordRPC = false;
        Cli.Output.Inform("EnableDiscordRPC set to false");
    }

    // enable-discordrpc arg
    if (Core.arguments.ContainsKey("--enable-discordrpc")) {
        Config.v.EnableDiscordRPC = true;
        Cli.Output.Inform("EnableDiscordRPC set to true");
    }

    // start discord rpc
    if (Config.v.EnableDiscordRPC) Utils.DiscordRPC.Initialize();
}

static async Task HandleArgs(string currentVersion) {
    // --prename handled in
    // --name handled in Snipesharp.cs
    // --skip-gc-redeem handled in Stats.cs & here
    // --dont-verify handled here AND in Core.cs
    // --await-first-packet handled in Sniper.cs
    // --offset handled in Names.cs
    // --disable-auto-update, --disable-discordrpc, --enable-discordrpc & --install handled in Initialize()

    if (Core.arguments.ContainsKey("--help")) Output.PrintHelp();
    if (Core.arguments.ContainsKey("--webhook-url")) {
        string webhookUrl = Core.arguments["--webhook-url"].data!;
        if (!string.IsNullOrEmpty(webhookUrl)) Config.v.CustomDiscordWebhookUrl = webhookUrl;
    }
    if (Core.arguments.ContainsKey("--prename")) Account.v.prename = true;
    if (Core.arguments.ContainsKey("-v") || Core.arguments.ContainsKey("--version")) {
        Output.Inform($"snipesharp {currentVersion}");
        Environment.Exit(0);
    }

        /* GETDROPPING API */
    // --pop-minsearches
    // --pop-length
    // --pop-lengthOption
    // --pop-language
    if (Core.arguments.ContainsKey("--pop-minsearches")) { 
        if (int.TryParse(Core.arguments["--pop-minsearches"].data!, out int minSearches)) {
            if (minSearches >= 0 && minSearches <= 10000) {
                Config.v.PopSearches = minSearches;
                Cli.Output.Inform($"PopSearches set to {minSearches}");
            }
            else Cli.Output.Error($"PopSearches must be greater or equal to 0 and lower or equal to 10000");
        }
        else Cli.Output.Error($"{Core.arguments["--pop-minsearches"].data!} is not a valid PopSearches value");
    }

    if (Core.arguments.ContainsKey("--pop-length")) { 
        if (int.TryParse(Core.arguments["--pop-length"].data!, out int length)) {
            if (length >= 3 && length <= 16) {
                Config.v.PopLength = length;
                Cli.Output.Inform($"PopLength set to {length}");
            }
            else Cli.Output.Error($"PopLength must be greater or equal to 3 and lower or equal to 16");
        }
        else Cli.Output.Error($"{Core.arguments["--pop-length"].data!} is not a valid PopLength value");
    }

    if (Core.arguments.ContainsKey("--pop-lengthOption")) { 
        if (int.TryParse(Core.arguments["--pop-lengthOption"].data!, out int length)) {
            if (length >= 3 && length <= 16) {
                Config.v.PopLengthOption = length;
                Cli.Output.Inform($"PopLengthOption set to {length}");
            }
            else Cli.Output.Error($"PopLengthOption must be greater or equal to 0 and lower or equal to 10000");
        }
        else Cli.Output.Error($"{Core.arguments["--pop-lengthOption"].data!} is not a valid PopLengthOption value");
    }

    if (Core.arguments.ContainsKey("--pop-language")) {
        string[] supportedLanguages = {
            "dutch", "english", "french", "german",
            "italian", "polish", "portuguese", "spanish"
        };
        if (int.TryParse(Core.arguments["--pop-language"].data!, out int language))
            Cli.Output.Error($"PopLanguage cannot be a number");
        else if (!supportedLanguages.Contains(Core.arguments["--pop-language"].data!.ToLower()))
            Cli.Output.Error($"{Core.arguments["--pop-language"].data!} is not a supported language, supported languages are: " + string.Join(" ", supportedLanguages));
        else {
            string lang = Core.arguments["--pop-language"].data!.ToLower();
            Config.v.PopLanguage = lang;
            Output.Inform("PopLanguage set to " + lang);
        }
    }

    if (Core.arguments.ContainsKey("--await-first-packet") && !Core.arguments.ContainsKey("--await-packets")) {
        Cli.Output.Warn($"The second name change packet will be sent {SetText.Red}after a response is received{SetText.ResetAll} from the first one!");
        Config.v.awaitFirstPacket = true;
    }
    if (Core.arguments.ContainsKey("--await-packets")) {
        Cli.Output.Warn($"Every name change packet will be sent {SetText.Red}after a response is received{SetText.ResetAll} from the one before it!");
        Config.v.awaitPackets = true;
    }
    if (Core.arguments.ContainsKey("--debug")) Config.v.debug = true;
    if (Core.arguments.ContainsKey("--spread")) { 
        if (int.TryParse(Core.arguments["--spread"].data!, out int packetSpreadMs)) {
            Config.v.PacketSpreadMs = packetSpreadMs;
            Cli.Output.Inform($"PacketSpreadMs set to {packetSpreadMs}");
        }
        else Cli.Output.Error($"{Core.arguments["--spread"].data!} is not a valid PacketSpreadMs value");
    }
    if (Core.arguments.ContainsKey("--packet-count")) { 
        if (int.TryParse(Core.arguments["--packet-count"].data!, out int sendPacketsCount)) {
            Config.v.SendPacketsCount = sendPacketsCount;
            Cli.Output.Inform($"SendPacketsCount set to {sendPacketsCount}");
        }
        else Cli.Output.Error($"{Core.arguments["--packet-count"].data!} is not a valid SendPacketsCount value");
    }
    if (Core.arguments.ContainsKey("--username")) { 
        Config.v.DiscordWebhookUsername = Core.arguments["--username"].data!;
        Cli.Output.Inform($"DiscordWebhookUsername set to {Core.arguments["--username"].data!}");
    }
    if (Core.arguments.ContainsKey("--asc")) {
        Config.v.AutoSkinChange = true;
        Cli.Output.Inform("AutoSkinChange set to true");
    }
    if (Core.arguments.ContainsKey("--asc-url")) {
        Config.v.SkinUrl = Core.arguments["--asc-url"].data!;
        Cli.Output.Inform($"SkinUrl set to ${Core.arguments["--asc-url"].data!}");
    }
    if (Core.arguments.ContainsKey("--periodically")) {
        if (int.TryParse(Core.arguments["--periodically"].data!, out int interval)) {
            Config.v.interval = interval * 60000;
            Cli.Output.Inform($"Interval set to {interval} minutes");
        }
        else Cli.Output.Error($"{Core.arguments["--periodically"].data!} is not a valid interval value (int)");
    }
    if (Core.arguments.ContainsKey("--email") && Core.arguments.ContainsKey("--password")){
        // exit if one of credentials is empty
        if (string.IsNullOrEmpty(Core.arguments["--email"].data!) || string.IsNullOrEmpty(Core.arguments["--password"].data!)) Cli.Output.ExitError($"Credentials can't be empty. Use {SetText.Blue}snipesharp --help{SetText.ResetAll} if you need help with using arguments.");

        // set account credentials
        Account.v.MicrosoftEmail = Core.arguments["--email"].data!;
        Account.v.MicrosoftPassword = Core.arguments["--password"].data!;
        Account.v.Bearer = Auth.AuthMicrosoft(Account.v.MicrosoftEmail, Account.v.MicrosoftPassword).Result.bearer;

        // verify the account works
        if (string.IsNullOrEmpty(Account.v.Bearer)) {
            Output.Error(TAuth.AuthInforms.FailedMicrosoft);
            return;
        }
        Cli.Output.Success(TAuth.AuthInforms.SuccessAuthMicrosoft);

        // update account file
        if (!Core.arguments.ContainsKey("--dont-verify")) FileSystem.UpdateAccount();

        // handle prename account and change config
        string? username = await Utils.Stats.GetUsername(Account.v.Bearer);
        HandleAccountType(currentVersion, username);

        var temp = new AuthResult {
            loginMethod = TAuth.AuthOptions.Microsoft
        };

        // get name to snipe if --name wasnt specified
        string argName = Snipesharp.GetNameToSnipe();

        // test rate limiting
        if (Core.arguments.ContainsKey("--test-rl")) await TestRatelimit();

        // snipe periodically
        if (Config.v.interval != null) await Sniper.ShootPeriodically(argName);

        if (argName == "l" || argName == TNames.UseNamesJson) await Names.handleNamesList(temp, FileSystem.GetNames());
        if (argName == "3" || argName == TNames.ThreeCharNames) await Names.handleThreeLetter(temp);
        if (argName == TNames.LetMePick) await Names.handleSingleName(temp);
        if (argName == "p" || argName == TNames.PopularNames) await Names.handlePopularNames(temp);
        if (
            argName != TNames.PopularNames && argName != "p" && argName != "3" &&
            argName != "l" && argName != TNames.LetMePick
        ) await Names.handleSingleName(temp, argName);

        // don't exit automatically
        if (Core.pid != PlatformID.Unix)
            Fix.Windows.QuickEdit(true);
        Console.ReadKey();
        Environment.Exit(0);
    }
    if (Core.arguments.ContainsKey("--mojang-email") && Core.arguments.ContainsKey("--mojang-password")){
        // exit if one of credentials is empty
        if (string.IsNullOrEmpty(Core.arguments["--mojang-email"].data!) || string.IsNullOrEmpty(Core.arguments["--mojang-password"].data!)) Cli.Output.ExitError($"Credentials can't be empty. Use {SetText.Blue}snipesharp --help{SetText.ResetAll} if you need help with using arguments.");

        // set account credentials
        Account.v.MojangEmail = Core.arguments["--mojang-email"].data!;
        Account.v.MojangPassword = Core.arguments["--mojang-password"].data!;
        Account.v.Bearer = await Auth.AuthMojang(Account.v.MojangEmail, Account.v.MojangPassword);

        // verify the account works
        if (string.IsNullOrEmpty(Account.v.Bearer)) {
            Output.Error(TAuth.AuthInforms.FailedMojang);
            return;
        }
        Cli.Output.Success(TAuth.AuthInforms.SuccessAuthMojang);

        // update account file
        if (!Core.arguments.ContainsKey("--dont-verify")) FileSystem.UpdateAccount();


        // handle prename account and change config
        string? username = await Utils.Stats.GetUsername(Account.v.Bearer);
        HandleAccountType(currentVersion, username);

        if (!String.IsNullOrEmpty(username)) { 
            Console.Title = $"snipesharp {currentVersion} - Logged in as {username}";
            if (Config.v.EnableDiscordRPC && Config.v.ShowUsernameDRPC) Utils.DiscordRPC.SetDescription($"Logged in as {username}");
        }

        var temp = new AuthResult {
            loginMethod = TAuth.AuthOptions.Mojang
        };

        // get name to snipe if --name wasnt specified
        string argName = Snipesharp.GetNameToSnipe();

        // test rate limiting
        if (Core.arguments.ContainsKey("--test-rl")) await TestRatelimit();

        // snipe periodically
        if (Config.v.interval != null) await Sniper.ShootPeriodically(argName);

        if (argName == "l" || argName == TNames.UseNamesJson) await Names.handleNamesList(temp, FileSystem.GetNames());
        if (argName == "3" || argName == TNames.ThreeCharNames) await Names.handleThreeLetter(temp);
        if (argName == TNames.LetMePick) await Names.handleSingleName(temp);
        if (argName == "p" || argName == TNames.PopularNames) await Names.handlePopularNames(temp);
        if (
            argName != TNames.PopularNames && argName != "p" && argName != "3" &&
            argName != "l" && argName != TNames.LetMePick
        ) await Names.handleSingleName(temp, argName);

        // don't exit automatically
        if (Core.pid != PlatformID.Unix)
            Fix.Windows.QuickEdit(true);
        Console.ReadKey();
        Environment.Exit(0);
    }
    if (Core.arguments.ContainsKey("--bearer")){
        // exit if bearer is empty
        if (string.IsNullOrEmpty(Core.arguments["--bearer"].data!)) Cli.Output.ExitError($"Bearer can't be empty. Use {SetText.Blue}snipesharp --help{SetText.ResetAll} if you need help with using arguments.");

        // set account bearer
        Account.v.Bearer = Core.arguments["--bearer"].data!;

        // verify the credentials work
        if(!Core.arguments.ContainsKey("--dont-verify")) {
            if (!await Auth.AuthWithBearer(Account.v.Bearer)) {
                Output.Error(TAuth.AuthInforms.FailedBearer);
                return;
            }
            Cli.Output.Success(TAuth.AuthInforms.SuccessAuth);
        }
        else Output.Warn("Not verifying bearer validity because --dont-verify was used");

        // update account file
        if (!Core.arguments.ContainsKey("--dont-verify")) FileSystem.UpdateAccount();

        // handle prename account and change config
        string? username = await Utils.Stats.GetUsername(Account.v.Bearer);
        HandleAccountType(currentVersion, username);
        
        if (!String.IsNullOrEmpty(username)) {
            Console.Title = $"snipesharp {currentVersion} - Logged in as {username}";
            if (Config.v.EnableDiscordRPC && Config.v.ShowUsernameDRPC) Utils.DiscordRPC.SetDescription($"Logged in as {username}");
        }

        var temp = new AuthResult {
            loginMethod = TAuth.AuthOptions.BearerToken
        };

        // get name to snipe if --name wasnt specified
        string argName = Snipesharp.GetNameToSnipe();

        // test rate limiting
        if (Core.arguments.ContainsKey("--test-rl")) await TestRatelimit();

        // snipe periodically
        if (Config.v.interval != null) await Sniper.ShootPeriodically(argName);

        if (argName == "l" || argName == TNames.UseNamesJson) await Names.handleNamesList(temp, FileSystem.GetNames());
        if (argName == "3" || argName == TNames.ThreeCharNames) await Names.handleThreeLetter(temp);
        if (argName == TNames.LetMePick) await Names.handleSingleName(temp);
        if (argName == "p" || argName == TNames.PopularNames) await Names.handlePopularNames(temp);
        if (
            argName != TNames.PopularNames && argName != "p" && argName != "3" &&
            argName != "l" && argName != TNames.LetMePick
        ) await Names.handleSingleName(temp, argName);

        // don't exit automatically
        if (Core.pid != PlatformID.Unix)
            Fix.Windows.QuickEdit(true);
        Console.ReadKey();
        Environment.Exit(0);
    }
}

static async Task TestRatelimit() {
    await Sniper.Shoot(Core.arguments.ContainsKey("--name") ? Core.arguments["--name"].data! : "abc");
    Console.ReadKey();
    Environment.Exit(0);
}

static void HandleAccountType(string currentVersion, string username) {
    if (Account.v.prename) {
        var maxPackets2 = Core.arguments.ContainsKey("--prename") ? true : !Convert.ToBoolean(
        new SelectionPrompt("Sniping using a prename account, switch to 2 max packets sent?", 
            new string[] { "Yes [suggested]", "No" }).answerIndex);
        Config.v.SendPacketsCount = maxPackets2 ? 2 : Config.v.SendPacketsCount;
        if (maxPackets2) Output.Inform(TAuth.AuthInforms.NoNameHistory);
        Console.Title = $"snipesharp {currentVersion} - Logged in with a prename account";
    }
    else if (!String.IsNullOrEmpty(username)) { 
        Console.Title = $"snipesharp {currentVersion} - Logged in as {username}";
        if (Config.v.ShowUsernameDRPC) Utils.DiscordRPC.SetDescription($"Logged in as {username}");
    }
}