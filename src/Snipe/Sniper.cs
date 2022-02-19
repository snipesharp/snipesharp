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
                success = (int)Name.Change(name, account.Bearer).Result.StatusCode == 200;
                Thread.Sleep(config.PacketSpreadMs);
            }

            // post success
            if (success)
            {
                Webhook.SendDiscordWebhooks(config, name);
                if (config.AutoSkinChange) Skin.Change(config.SkinUrl, config.SkinType, account.Bearer);
            }
        }
        public async static Task WaitForName(string name, long delay, bool fromList=false)
        {
            // calculate total wait time
            var spinner = new Spinner();
            var waitTime = Math.Max(await Droptime.GetMilliseconds(name, !fromList) - delay, 0);
            spinner.Cancel();

            // countdown animation
            var countDown = new CountDown(waitTime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

            // actually wait for the right time
            Thread.Sleep(TimeSpan.FromMilliseconds(waitTime));
            countDown.Cancel();

            return;
        }
    }
}
