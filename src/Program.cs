using Cli;
using Cli.Animatables;
using DataTypes.SetText;

// clear the console before execution
Console.Clear();
SetText.DisplayCursor(true);

// welcome the user
Output.PrintLogo();

// display prompt
string loginMethod = new SelectionPrompt("Login method: ", "Bearer Token", "Mojang Account").result;

// obtain login info based on login method choice
// todo actual authentication part
if (loginMethod == "Bearer Token") {
    string bearer = Input.Request<string>(
        $"Paste your {SetText.Blue}Bearer Token{SetText.ResetAll}: ",
        validator: Validators.Credentials.Bearer
    );
    Output.Inform($"{SetText.Blue}Successfully authenticated");
}
else if (loginMethod == "Mojang Account") {
    string email = Input.Request<string>(
        $"Enter your Mojang account {SetText.Blue}Email{SetText.ResetAll}: ",
        validator:Validators.Credentials.Email
    );
    string password = Input.Request<string>($"Enter your Mojang account {SetText.Blue}Password{SetText.ResetAll}: ", true);
    Output.Inform($"{SetText.Blue}Successfully authenticated");
}
else {
    // todo read from file
}

// require initial information
string name = Input.Request<string>("Name to snipe: ");
int delay = Input.Request<int>("Custom delay in ms: ");

// create a dummy spinner for testing
var spinner = new Spinner();

// you can cancel at any time
// and it doesn't block the main thread <3
Thread.Sleep(1000);
spinner.Cancel();

// prepare the sniper
// todo

// inform the user
Output.Inform($"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in 55 mins");

// don't exit automatically
Output.Inform("Press any key to continue...");
Console.ReadKey();