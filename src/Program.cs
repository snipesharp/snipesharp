﻿using Cli;
using DataTypes.SetText;

// clear the console before execution
Console.Clear();

// welcome the user
Output.PrintLogo();

// require initial information
string name = Input.Request<string>("What name would you like to snipe");
int delay = Input.Request<int>("Custom delay in ms");

// prepare the sniper
// todo

// inform the user
Output.Inform($"Sniping {SetText.Blue}{name}{SetText.ResetAll} in 55 mins");