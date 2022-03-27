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
            for (int i = 0; (i < Config.v.SendPacketsCount); i++) {
                if (Config.v.awaitFirstPacket && i == 0) {
                    await Name.Change(name, Account.v.prename);
                    continue;
                }
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
            if (Config.v.debug) Cli.Output.Inform($"Got msToSleep @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
            if (Config.v.debug)
                Cli.Output.Inform($"Should snipe @ {SetText.Blue}{DateTime.Now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond).Second}s{DateTime.Now.AddMilliseconds(msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond).Millisecond}ms" +
                $"{SetText.ResetAll}, {DateTime.Now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Second}s{DateTime.Now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond) + DataTypes.Config.v.PacketSpreadMs).Millisecond}ms" + 
                $" & {DateTime.Now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Second}s{DateTime.Now.AddMilliseconds((msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond) + (DataTypes.Config.v.PacketSpreadMs*2)).Millisecond}ms");
            if (Config.v.debug) Cli.Output.Inform($"Sleeping @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
            Thread.Sleep(msToSleep <= 0 ? 0 : msToSleep - DateTime.Now.Millisecond); // take away the current milliseconds

            if (Config.v.debug) Cli.Output.Inform($"Cancelling countdown @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
            countDown.Cancel();
            if (Config.v.debug) Cli.Output.Inform($"Cancelling finished @ {DateTime.Now.Second}s{DateTime.Now.Millisecond}ms");
            return;
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
