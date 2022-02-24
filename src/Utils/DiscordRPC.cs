using DiscordRPC;
using DataTypes;

namespace Utils
{
    public class DiscordRPC
    {
        private static DiscordRpcClient? client;
        private static string State = "Setting up Snipesharp";
        private static Timestamps Timestamps = new Timestamps(){
            EndUnixMilliseconds = 0,
        };

        public static void Deinitialize(){
            client.Dispose();
        }

        public static void Initialize(){
            client = new DiscordRpcClient(Config.v.DiscordApplicationId);		
            client.Initialize();
            Update();
        }

        public static void SetSniping(string name, long droptime){
            var days = (int)TimeSpan.FromMilliseconds(droptime).Days;
            if(days > 0) {
                State = $"Sniping name \"{name}\" in {days} days";
                Timestamps.EndUnixMilliseconds = 0;
            }
            else {
                State = $"Sniping name \"{name}\"";
                Timestamps.EndUnixMilliseconds = (ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds() + (ulong)droptime;
            }
            Update();
        }

        private static void Update(){
            client.SetPresence(new RichPresence() {
                Details = "A minecraft name sniper/grabber",
                State = State,
                Timestamps = Timestamps.EndUnixMilliseconds > 0 ? Timestamps : null,
                Assets = new Assets() {
                    LargeImageKey = "rpc_icon",
                    LargeImageText = "Snipesharp",
                },
                Party = new Party() {
                    Size = 0,
                    Max = 0,
                },
                Buttons = new Button[] {
                    new Button() {
                        Label = "Snipesharp",
                        Url = "https://github.com/snipesharp/snipesharp"
                    }
                }
            });	
        }
    }
}
