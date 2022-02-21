namespace Cli.Templates
{
    public class TAuth
    {
        public struct AuthOptions {
            public static string PreviousSession = "From previous session";
            public static string BearerToken = "Bearer Token";
            public static string Microsoft = "Microsoft Account";
        }

        public struct AuthInforms {
            public static string SuccessAuth = "Successfully authenticated";
            public static string SuccessAuthBearer = "Successfully authenticated & updated bearer";
            public static string FailedMicrosoft = "Failed to authenticate Microsoft account";
            public static string FailedBearer = "Failed to authenticate using bearer";
            public static string Warn2FA = "Make sure 2 Factor Authentication (2FA) is turned off in your Microsoft account settings";
            public static string WarnBearer = "Bearer tokens reset every 24 hours & on login, sniping will fail if the bearer has expired at snipe time!";
            public static string ManyLoginMethods = "More than one login method previously used, choose one:";
            public static string NoNameHistory = "No name history detected, will perform prename snipe and send 6 packets instead of 3";
        }
    }
}