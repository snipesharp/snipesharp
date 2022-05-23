using DiscordRPC;
using DataTypes;

namespace Utils
{
    public class DiscordRPC
    {
        private static DiscordRpcClient? client;
        private static string State = "Setting up Snipesharp";
        private static string Details = "The best Minecraft name sniper!";
        private static Timestamps Timestamps = new Timestamps(){
            EndUnixMilliseconds = 0,
        };

        public static void Deinitialize(){
            if(client == null) return;

            client!.Dispose();
        }

        public static void Initialize(){
            client = new DiscordRpcClient(Config.v.discordApplicationId);
            client.Initialize();
            if (Details != null) if (!Details.StartsWith("Logged in")) RandomizeDetails();
            Update();
        }

        public static void RandomizeDetails() {
            var random = new Random().Next(1,4);
            switch (random) {
                case 1: Details = "Name a better MC name sniper"; break;
                case 2: Details = "A fast & easy to use name sniper!"; break;
                case 3: Details = "Better than paid name snipers!"; break;
                default: Details = "The best Minecraft name sniper!"; break;
            }
        }

        public static void SetSniping(string name, long droptime){
            if(client == null) return;

            var days = (int)TimeSpan.FromMilliseconds(droptime).Days;
            if(days > 0) {
                State = $"Sniping name \"{name}\" in {days} day" + (days == 1 ? "" : "s");
                Timestamps.EndUnixMilliseconds = 0;
            }
            else {
                State = $"Sniping name \"{name}\"";
                Timestamps.EndUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds() + (ulong)droptime;
            }
            Update();
        }

        public static void SetDescription(string description) {
            if(client == null) return;

            Details = description;
            Update();
        }

        private static void Update(){
            if(client == null) return;

            client!.SetPresence(new RichPresence() {
                Details = Details,
                State = State,
                Timestamps = Timestamps.EndUnixMilliseconds > 0 ? Timestamps : null,
                Assets = new Assets() {
                    LargeImageKey = "1024p_logo",
                    LargeImageText = "Snipesharp",
                },
                Party = new Party() {
                    Size = 0,
                    Max = 0,
                },
                Buttons = new Button[] {
                    new Button() {
                        Label = "Snipesharp",
                        Url = "https://snipesharp.xyz"
                    }
                }
            });	
        }
    }
}
