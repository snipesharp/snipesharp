using Cli.Animatables;
using Cli.Templates;
using DataTypes;
using DataTypes.SetText;
using Utils;

namespace Snipe
{
    public class Sniper
    {
        public static void Shoot(Account account, string name) {
            var success = false;
            for (int i = 0; (i < Config.v.sendPacketsCount && !success); i++) {
                success = (int)Name.Change(name, account.Bearer, account.Prename).Result.StatusCode == 200;
                Thread.Sleep(Config.v.PacketSpreadMs);
            }

            // post success
            if (success) {
                Webhook.SendDiscordWebhooks(name);
                if (Config.v.AutoSkinChange) Skin.Change(Config.v.SkinUrl, Config.v.SkinType, account.Bearer);
            }
        }

        public static void WaitForName(string name, long droptime, Account account, string loginMethod) {
            // calculate total wait time
            var waitTime = Math.Max(droptime, 0);

            // countdown animation
            var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

            // wait for the time minus 5 minutes then reauthenticate // async but not awaited
            if (loginMethod == TAuth.AuthOptions.Microsoft && waitTime > 300000) Reauthenticate(account, waitTime);

            // actually wait for the time
            int msToSleep = (int)TimeSpan.FromMilliseconds(waitTime).TotalMilliseconds;
            Thread.Sleep(msToSleep);

            countDown.Cancel();
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
