using FS;
using DataTypes;
using Cli.Templates;
using Cli.Animatables;

namespace Cli
{
    public class Core
    {
        public static Dictionary<string, Argument> arguments = ParseArgs();
        public static PlatformID pid = Environment.OSVersion.Platform;

        // parse the command line arguments and convert them to a dictionary
        private static Dictionary<string, Argument> ParseArgs(){
            List<string> args = Environment.GetCommandLineArgs().Skip(1).ToList();
            var parsedArguments = new Dictionary<string, Argument>();
            
            args.ForEach(arg => {
                var parsed = new Argument(arg);
                parsedArguments[parsed.name] = parsed;
            });

            return parsedArguments;
        }

        public static async Task<Account> Auth(){
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
                if (await Snipe.Auth.AuthWithBearer(account.Bearer))
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
                account.MojangEmail = Input.Request<string>(Requests.Email, validator:Validators.Credentials.Email);
                account.MojangPassword = Input.Request<string>(Requests.Password, hidden: true);
                // todo mojang auth 
                Output.Inform($"Not authenticated (Mojang login not implemented)");
            }
            else {
                var loadedAccount = FileSystem.GetAccount();
                if (loadedAccount.Bearer != null) {
                    var spinnerAuth = new Spinner();
                    spinnerAuth.Cancel();
                    if (await Snipe.Auth.AuthWithBearer(loadedAccount.Bearer))
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

            return account;
        }
    }
}