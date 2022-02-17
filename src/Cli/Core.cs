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
            Account account = new Account();
            if (loginMethod == "Bearer Token") account = await HandleBearer(account);
            else if (loginMethod == "Mojang Account") account = await HandleMojang(account);
            else account = await HandleSavedSession();

            // save account
            if (loginMethod != "From previous session") FileSystem.SaveAccount(account);

            return account;
        }
    
        private static async Task<Account> HandleMojang(Account account){
            account.MojangEmail = Input.Request<string>(Requests.Email, validator:Validators.Credentials.Email);
            account.MojangPassword = Input.Request<string>(Requests.Password, hidden: true);

            // todo, actual async stuff here
            Output.Inform($"Not authenticated (Mojang login not implemented)");
            return account;
        }

        private static async Task<Account> HandleBearer(Account account){
            // prompt for bearer token
            account.Bearer = Input.Request<string>(Requests.Bearer);
            var spinnerAuth = new Spinner();
            spinnerAuth.Cancel();

            // exit if invalid bearer
            if(!await Snipe.Auth.AuthWithBearer(account.Bearer)) Output.ExitError("Failed to authenticate using bearer");

            // validate the token
            Output.Success($"Successfully authenticated");
            Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
        
            return account;
        }

        private static async Task<Account> HandleSavedSession(){
            var account = FileSystem.GetAccount();

            // @demented Should that still be the HandleMojang, since you 
            // also implemented both Account.MojangEmail/Pass and Microsoft

            // for now leaving code logic as it was before
            // please read and respond
            if(account.Bearer == null) return await HandleMojang(new Account());

            // if bearer is present proceed
            var spinnerAuth = new Spinner();
            if (!await Snipe.Auth.AuthWithBearer(account.Bearer)) Output.ExitError("Failed to authenticate using bearer");
            Output.Success($"Successfully authenticated");
            Output.Warn("Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!");
            account.Bearer = account.Bearer;
            spinnerAuth.Cancel();

            return account;
        }
    }
}