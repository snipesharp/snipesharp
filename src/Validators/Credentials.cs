using System.Text.RegularExpressions;

namespace Validators
{
    public class Credentials
    {
        // define regex values here
        private static Regex rEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        private static Regex rBearer = new Regex(@"Bearer\s[\d|a-f]{8}-([\d|a-f]{4}-){3}[\d|a-f]{12}$");

        // actual helper functions here
        public static Func<string, bool> Email = email => rEmail.Match(email).Success;
        public static Func<string, bool> Bearer = bearer => rBearer.Match(bearer).Success;
    }
}