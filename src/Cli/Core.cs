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
                ? new SelectionPrompt("Login method:", new string[] {
                    TAuth.AuthOptions.PreviousSession,
                    TAuth.AuthOptions.BearerToken,
                    TAuth.AuthOptions.Microsoft
                }).result
                : new SelectionPrompt("Login method:", new string[] { 
                    TAuth.AuthOptions.BearerToken,
                    TAuth.AuthOptions.Microsoft
                }).result;
 
            // obtain login info based on login method choice
            Account account = FileSystem.AccountFileExists() ? FileSystem.GetAccount() : new Account();
            if (loginMethod == TAuth.AuthOptions.BearerToken) account = await HandleBearer(account, 1, true);
            else if (loginMethod == TAuth.AuthOptions.Microsoft) account = await HandleMicrosoft(account, 1, true);
            else {
                var handleFromFileResult = await HandleFromFile();
                account = handleFromFileResult.Account;
                loginMethod = handleFromFileResult.Choice;
            }

            // save account and return
            FileSystem.SaveAccount(account);
            return new AuthResult { account = account, loginMethod = loginMethod };
        }

        public static async Task<Account> HandleMicrosoft(Account account, int attempt, bool newLogin=false, bool askForEmail=true){
            
            // get new credentials
            if (newLogin) {
                if (askForEmail) account.MicrosoftEmail = Input.Request<string>(
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

            // if bearer not returned, retry
            if (String.IsNullOrEmpty(authResult.bearer)) {
                FS.FileSystem.Log(TAuth.AuthInforms.FailedMicrosoft + $" - attempt {attempt}");
                return await HandleMicrosoft(account, ++attempt, true, authResult.error != "Wrong password");
            }

            account.Bearer = authResult.bearer;
            account.prename = authResult.prename;
            Output.Success(attempt == 3 ? TAuth.AuthInforms.SuccessAuthMicrosoft + ", third time's a charm" : TAuth.AuthInforms.SuccessAuthMicrosoft);

            return account;
        }

        private static async Task<Account> HandleBearer(Account account, int attempt, bool newBearer=false){
            // prompt for bearer token
            if (newBearer) account.Bearer = Input.Request<string>(TRequests.Bearer);

            // retry if invalid bearer
            if(!await Snipe.Auth.AuthWithBearer(account.Bearer)) {
                Output.Error(TAuth.AuthInforms.FailedBearer);
                return await HandleBearer(account, ++attempt, true);
            }

            // validate the token
            Output.Warn(TAuth.AuthInforms.WarnBearer);
            Output.Success(attempt == 3 ? TAuth.AuthInforms.SuccessAuth + ", third time's a charm" : TAuth.AuthInforms.SuccessAuth);
        
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
            if (choice == TAuth.AuthOptions.BearerToken) account = await HandleBearer(account, 1);
            if (choice == TAuth.AuthOptions.Microsoft) account = await HandleMicrosoft(account, 1);

            return new HandleFromFileResult { Account = account, Choice = choice };
        }

        // return type of handlefromFile
        private struct HandleFromFileResult {
            public Account Account { get; set; }
            public string Choice { get; set; }
        }
    }
}