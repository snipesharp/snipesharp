using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Utils;

namespace Snipe
{
    public class Sniper
    {
        public static void Shoot(Config config, Account account, string name) {
            var success = false;
            for (int i = 0; (i < config.sendPacketsCount && !success); i++) {
                success = (int)Name.Change(name, account.Bearer, account.Prename).Result.StatusCode == 200;
                Thread.Sleep(config.PacketSpreadMs);
            }

            // post success
            if (success) {
                Webhook.SendDiscordWebhooks(config, name);
                if (config.AutoSkinChange) Skin.Change(config.SkinUrl, config.SkinType, account.Bearer);
            }
        }

        public async static Task WaitForName(string name, long delay, Account account, string loginMethod, bool fromList=false) {
            // calculate total wait time
            var waitTime = Math.Max(await Droptime.GetMilliseconds(name, !fromList) - delay, 0);

            // countdown animation
            var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

            // wait for the time minus 5 minutes then reauthenticate // async but not awaited
            if (loginMethod == "Microsoft Account" && waitTime > 300000) Reauthenticate(account, waitTime);

            // actually wait for the time
            int msToSleep = (int)TimeSpan.FromMilliseconds(waitTime).TotalMilliseconds;
            Thread.Sleep(msToSleep);

            countDown.Cancel();
            return;
        }

        public static async void Reauthenticate(Account account, long waitTime) {
            // sleep until 5 mins before
            Thread.Sleep((int)waitTime - 299990);

            FS.FileSystem.Log("Refreshing bearer");
            var result = await Auth.AuthMicrosoft(account.MicrosoftEmail, account.MicrosoftPassword);
            account.Bearer = result.bearer;
            FS.FileSystem.SaveAccount(account);
        }
    }
}
