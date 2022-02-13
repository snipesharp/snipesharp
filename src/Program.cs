using Cli;
using Cli.Animatables;
using DataTypes.SetText;

// clear the console before execution
Console.Clear();
SetText.DisplayCursor(true);

// welcome the user
Output.PrintLogo();

// display prompt
var prompt = new SelectionPrompt("Do you like fifa?", "Yes", "No", "Huh?", "Maybe");

Console.WriteLine("You selected: " + prompt.result);

// require initial information
string name = Input.Request<string>("What name would you like to snipe");
int delay = Input.Request<int>("Custom delay in ms");

// create a dummy spinner for testing
var spinner = new Spinner();

// you can cancel at any time
// and it doesn't block the main thread <3
Thread.Sleep(2000);
spinner.Cancel();

var rainbowText = new RainbowText("Rainbow Text");
Thread.Sleep(6000);
rainbowText.Cancel();

// prepare the sniper
// todo

// inform the user
Output.Inform($"Sniping {SetText.Blue}{name}{SetText.ResetAll} in 55 mins");

// don't exit automatically
Output.Inform("Press any key to continue...");
Console.ReadKey();