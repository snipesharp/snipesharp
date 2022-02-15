using System.Text.RegularExpressions;

namespace Validators
{
    public class Credentials
    {
        // define regex values here
        private static Regex rEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        // actual helper functions here
        public static Func<string, bool> Email = email => rEmail.Match(email).Success;
    }
}