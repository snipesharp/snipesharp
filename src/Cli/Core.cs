using FS;
using DataTypes;
using Cli.Templates;
using Cli.Animatables;
using DataTypes.Auth;

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

        public static async Task<AuthResult> Auth(){
            // display prompt
            string loginMethod = FileSystem.AccountFileExists()
                ? new SelectionPrompt("Login method:", 
                    TAuth.AuthOptions.PreviousSession,
                    TAuth.AuthOptions.BearerToken,
                    TAuth.AuthOptions.Microsoft
                ).result
                : new SelectionPrompt("Login method:", 
                    TAuth.AuthOptions.BearerToken,
                    TAuth.AuthOptions.Microsoft
                ).result;
 
            // obtain login info based on login method choice
            Account account = FileSystem.AccountFileExists() ? FileSystem.GetAccount() : new Account();
            if (loginMethod == TAuth.AuthOptions.BearerToken) account = await HandleBearer(account, true);
            else if (loginMethod == TAuth.AuthOptions.Microsoft) account = await HandleMicrosoft(account, true);
            else {
                var handleFromFileResult = await HandleFromFile();
                account = handleFromFileResult.Account;
                loginMethod = handleFromFileResult.Choice;
            }

            // save account and return
            FileSystem.SaveAccount(account);
            return new AuthResult { account = account, loginMethod = loginMethod };
        }

        private static async Task<Account> HandleMicrosoft(Account account, bool newLogin=false){
            // warn about 2fa
            Output.Warn(TAuth.AuthInforms.Warn2FA);

            // get new credentials
            if (newLogin) {
                account.MicrosoftEmail = Input.Request<string>(
                    TRequests.MicrosoftEmail, 
                    validator: Validators.Credentials.Email
                );
                account.MicrosoftPassword = Input.Request<string>(
                    TRequests.MicrosoftPassword,
                    hidden: true
                );
            }
            
            // get bearer with microsoft credentials
            var authResult = await Snipe.Auth.AuthMicrosoft(account.MicrosoftEmail, account.MicrosoftPassword);

            // if bearer not returned, exit
            if (String.IsNullOrEmpty(authResult.bearer)) Output.ExitError(TAuth.AuthInforms.FailedMicrosoft);

            account.Bearer = authResult.bearer;
            account.Prename = authResult.prename;
            Output.Success(TAuth.AuthInforms.SuccessAuthMicrosoft);

            return account;
        }

        private static async Task<Account> HandleBearer(Account account, bool newBearer=false){
            // prompt for bearer token
            if (newBearer) account.Bearer = Input.Request<string>(TRequests.Bearer);

            // exit if invalid bearer
            if(!await Snipe.Auth.AuthWithBearer(account.Bearer)) Output.ExitError(TAuth.AuthInforms.FailedBearer);

            // validate the token
            Output.Warn(TAuth.AuthInforms.WarnBearer);
            Output.Success(TAuth.AuthInforms.SuccessAuth);
        
            return account;
        }

        private static async Task<HandleFromFileResult> HandleFromFile() {
            var account = FileSystem.GetAccount();

            // determine available methods
            List<string> availableMethods = new List<string>();
            if (!String.IsNullOrEmpty(account.Bearer))
                availableMethods.Add(TAuth.AuthOptions.BearerToken);
            if (
                !String.IsNullOrEmpty(account.MicrosoftPassword) && 
                !String.IsNullOrEmpty(account.MicrosoftEmail)
            ) availableMethods.Add(TAuth.AuthOptions.Microsoft);

            // determine final auth method
            string choice = availableMethods.Count > 1
                ? new SelectionPrompt(
                    TAuth.AuthInforms.ManyLoginMethods,
                    availableMethods.ToArray()).result
                : availableMethods[0];

            // authenticate the chosen method
            if (choice == TAuth.AuthOptions.BearerToken) account = await HandleBearer(account);
            if (choice == TAuth.AuthOptions.Microsoft) account = await HandleMicrosoft(account);

            return new HandleFromFileResult { Account = account, Choice = choice };
        }

        // return type of handlefromFile
        private struct HandleFromFileResult {
            public Account Account { get; set; }
            public string Choice { get; set; }
        }
    }
}