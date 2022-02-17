using System.Text.RegularExpressions;

namespace Validators
{
    public class Auth
    {
        public static Regex rSFTTagRegex = new Regex("value=\"(.+?)\"");
        public static Regex rUrlPostRegex = new Regex("urlPost:'(.+?)'");
    }
}
