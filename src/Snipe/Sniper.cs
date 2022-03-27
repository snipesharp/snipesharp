using Cli.Animatables;
using Cli.Templates;
using DataTypes;
using DataTypes.SetText;
using Utils;

namespace Snipe
{
    public class Sniper
    {
        public static async Task Shoot(string name) {
            for (int i = 0; (i < Config.v.SendPacketsCount); i++) {
                if (Config.v.awaitFirstPacket && i == 0) {
                    await Name.Change(name, i, Account.v.prename);
                    continue;
                }
                Name.Change(name, i, Account.v.prename);
            }
        }

        public static void WaitForName(string name, long droptime, string loginMethod) {
            // update discord rpc
            if (Config.v.ShowTargetNameDRPC) Utils.DiscordRPC.SetSniping(name, droptime);

            // countdown animation
            var countDown = new CountDown(droptime, $"Sniping {SetText.DarkBlue + SetText.Bold}{name}{SetText.ResetAll} in " + "{TIME}");

            // wait for the time minus 5 minutes then reauthenticate // async but not awaited
            if (Config.v.EnableBearerRefreshing && loginMethod == TAuth.AuthOptions.Microsoft && droptime > 300000) Reauthenticate(droptime);

            // actually wait for the time
            int msToSleep = (int)TimeSpan.FromMilliseconds(droptime).TotalMilliseconds;
            if (Config.v.debug) {
                var now = DateTime.Now;
                Cli.Output.Inform($"Should snipe @ {SetText.Blue}{now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - now.Millisecond).Second}s{now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - now.Millisecond).Millisecond}ms" +
                $"{SetText.ResetAll}, {now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Second}s{now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Millisecond}ms" + 
                $" & {now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Second}s{now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Millisecond}ms");
            }
            Thread.Sleep((msToSleep <= 0 ? 0 : (msToSleep - DateTime.Now.Millisecond) - 50)); // take away the current milliseconds
            countDown.Cancel();
            Snipesharp.snipeTime = DateTime.Now.AddMilliseconds(50);
        }

        public static async Task Reauthenticate(long waitTime) {
            // sleep until 5 mins before
            await Task.Delay((int)waitTime - 299990);

            FS.FileSystem.Log("Refreshing bearer");
            var result = await Auth.AuthMicrosoft(Account.v.MicrosoftEmail, Account.v.MicrosoftPassword);
            Account.v.Bearer = result.bearer;
            FS.FileSystem.UpdateAccount();

            SetText.DisplayCursor(false);
        }
    }
}
