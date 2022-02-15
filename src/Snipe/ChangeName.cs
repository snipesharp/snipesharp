using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snipesharp.Snipe
{
    internal class ChangeName
    {
        public string GetResponseMessage(int code)
        {
            switch (code)
            {
                case 400: return "Name is invalid, longer than 16 characters or contains characters other than (a-zA-Z0-9_)";
                case 403: return "Name is unavailable (Either taken or has not become available)";
                case 401: return "Unauthorized (Bearer token expired or is not correct)";
                case 429: return "Too many requests sent";
                case 500: return "Timed out (API lagged out and could not respond)";
                case 200: return "Success (Name changed)";
                default: return "Unknown";
            }
        }
    }
}
