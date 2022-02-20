using Cli.Animatables;
using DataTypes;
using DataTypes.SetText;
using Utils;

namespace Snipe
{
    public class Sniper
    {
        public static void Shoot(Config config, Account account, string name)
        {
            var success = false;
            for (int i = 0; (i < config.sendPacketsCount && !success); i++)
            {
                success = (int)Name.Change(name, account.Bearer, account.Prename).Result.StatusCode == 200;
                Thread.Sleep(config.PacketSpreadMs);
            }

            // post success
            if (success)
            {
                Webhook.SendDiscordWebhooks(config, name);
                if (config.AutoSkinChange) Skin.Change(config.SkinUrl, config.SkinType, account.Bearer);
            }
        }
        public async static Task WaitForName(string name, long delay, string loginMethod, Account account, bool fromList=false)
        {
            // calculate total wait time
            var waitTime = Math.Max(await Droptime.GetMilliseconds(name, !fromList) - delay, 0);

            // countdown animation
            var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

            // actually wait for the time
            int msToSleep = (int)TimeSpan.FromMilliseconds(waitTime).TotalMilliseconds;
            Thread.Sleep(msToSleep <= 300000 ? msToSleep : msToSleep - 300000);

            // re authenticate with microsoft
            if (loginMethod == "Microsoft Account" && msToSleep <= 300000)
            {
                var timeTook = System.Diagnostics.Stopwatch.StartNew();
                // update bearer

                var result = await Auth.AuthMicrosoft(account.MicrosoftEmail, account.MicrosoftPassword);
                account.Bearer = result.bearer;

                FS.FileSystem.SaveAccount(account);

                timeTook.Stop();
                Thread.Sleep(30000 - (int)timeTook.ElapsedMilliseconds);
            }

            countDown.Cancel();

            return;
        }
    }
}
