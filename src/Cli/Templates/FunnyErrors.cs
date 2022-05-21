namespace Cli.Templates
{
    public static class TFunnyErrors
    {
        private static List<string> msges = new string[] {
            "One of us really sucks at this",
            "RIp in piece",
            "L + Ratio",
            ":pensive:",
            "U a bitch and yo mama fat",
        }.ToList();

        public static string GetRandom(){
            Random random = new Random();
            int pos = random.Next(msges.Count);
            return msges[pos];
        }
    }
}