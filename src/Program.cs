// welcome the user
using snipesharp.Cli;

Cli.Output.PrintLogo();

// require initial information
string name = Cli.Input.Request<string>("What name would you like to snipe");
int delay = Cli.Input.Request<int>("Custom delay in ms");

// prepare the sniper
// todo

// inform the user
Cli.Output.Inform($"Sniping {SetText.Blue}{name}{SetText.ResetAll} in 55 mins");