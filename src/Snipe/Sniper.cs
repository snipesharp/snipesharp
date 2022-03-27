using Cli.Animatables;
using Cli.Templates;
using DataTypes;
using DataTypes.SetText;
using Utils;

namespace Snipe
{
    public class Sniper
    {
        public static async void Shoot(string name) {
            if (DataTypes.Config.v.debug) Cli.Output.Inform($"Sniper.Shoot called @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
            for (int i = 0; (i < Config.v.SendPacketsCount); i++) {
                if (Config.v.awaitFirstPacket && i == 0) {
                    if (DataTypes.Config.v.debug) Cli.Output.Inform($"Calling Name.Change @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
                    await Name.Change(name, Account.v.prename);
                    continue;
                }
                if (DataTypes.Config.v.debug) Cli.Output.Inform($"Calling Name.Change @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
                Name.Change(name, Account.v.prename);
                Thread.Sleep(Config.v.PacketSpreadMs);
            }
        }

        public static void WaitForName(string name, long droptime, string loginMethod) {
            if (Config.v.debug) Cli.Output.Inform($"WaitForName called @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
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
                Cli.Output.Inform($"Got msToSleep @ {now.Second}s{now.Millisecond}ms");
                Cli.Output.Inform($"Should snipe @ {SetText.Blue}{now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - now.Millisecond).Second}s{now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - now.Millisecond).Millisecond}ms" +
                $"{SetText.ResetAll}, {now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Second}s{now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Millisecond}ms" + 
                $" & {now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Second}s{now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Millisecond}ms");
                Cli.Output.Inform($"Sleeping @ {now.Second}s{now.Millisecond}ms");
            }
            Thread.Sleep(msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond); // take away the current milliseconds
            countDown.Cancel();
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
