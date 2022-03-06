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
            // first time setup
            if (DataTypes.Config.v.firstTime) Cli.Output.Inform(Cli.Templates.TFileSystem.FSInforms.SelectionPromptUsage);
            
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
            if (loginMethod == TAuth.AuthOptions.BearerToken) await HandleBearer(1, true);
            else if (loginMethod == TAuth.AuthOptions.Microsoft) await HandleMicrosoft(1, true);
            else {
                var handleFromFileResult = await HandleFromFile();
                loginMethod = handleFromFileResult.Choice;
            }

            // save account and return
            FileSystem.SaveAccount();
            return new AuthResult { loginMethod = loginMethod };
        }

        public static async Task HandleMicrosoft(int attempt, bool newLogin=false, bool askForEmail=true){
            
            // get new credentials
            if (newLogin) {
                if (askForEmail) Account.v.MicrosoftEmail = Input.Request<string>(
                    TRequests.MicrosoftEmail, 
                    validator: Validators.Credentials.Email
                );
                Account.v.MicrosoftPassword = Input.Request<string>(
                    TRequests.MicrosoftPassword,
                    hidden: true
                );
            }
            
            // get bearer with microsoft credentials
            var authResult = await Snipe.Auth.AuthMicrosoft(Account.v.MicrosoftEmail, Account.v.MicrosoftPassword);

            // if bearer not returned, retry
            if (String.IsNullOrEmpty(authResult.bearer)) {
                FS.FileSystem.Log(TAuth.AuthInforms.FailedMicrosoft + $" - attempt {attempt}");
                await HandleMicrosoft(++attempt, true, authResult.error != "Wrong password");
                return;
            }

            Account.v.Bearer = authResult.bearer;
            Account.v.prename = authResult.prename;
            Output.Success(attempt == 3 ? TAuth.AuthInforms.SuccessAuthMicrosoft + ", third time's a charm" : TAuth.AuthInforms.SuccessAuthMicrosoft);
        }

        public static async Task HandleBearer(int attempt, bool newBearer=false){
            // prompt for bearer token
            if (newBearer) Account.v.Bearer = Input.Request<string>(TRequests.Bearer);

            // retry if invalid bearer
            if(!await Snipe.Auth.AuthWithBearer(Account.v.Bearer)) {
                Output.Error(TAuth.AuthInforms.FailedBearer);
                await HandleBearer(++attempt, true);
                return;
            }

            // validate the token
            Output.Warn(TAuth.AuthInforms.WarnBearer);
            Output.Success(attempt == 3 ? TAuth.AuthInforms.SuccessAuth + ", third time's a charm" : TAuth.AuthInforms.SuccessAuth);
        }

        private static async Task<HandleFromFileResult> HandleFromFile() {
            // determine available methods
            List<string> availableMethods = new List<string>();
            if (!String.IsNullOrEmpty(Account.v.Bearer))
                availableMethods.Add(TAuth.AuthOptions.BearerToken);
            if (
                !String.IsNullOrEmpty(Account.v.MicrosoftPassword) && 
                !String.IsNullOrEmpty(Account.v.MicrosoftEmail)
            ) availableMethods.Add(TAuth.AuthOptions.Microsoft);

            // determine final auth method
            string choice = availableMethods.Count > 1
                ? new SelectionPrompt(
                    TAuth.AuthInforms.ManyLoginMethods,
                    availableMethods.ToArray()).result
                : availableMethods[0];

            // authenticate the chosen method
            if (choice == TAuth.AuthOptions.BearerToken) await HandleBearer(1);
            if (choice == TAuth.AuthOptions.Microsoft) await HandleMicrosoft(1);

            return new HandleFromFileResult { Choice = choice };
        }

        // return type of handlefromFile
        private struct HandleFromFileResult {
            public string Choice { get; set; }
        }
    }
}