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
                if (Config.v.awaitPackets || (Config.v.awaitFirstPacket && i == 0)) {
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
            if (Config.v.EnableBearerRefreshing && loginMethod == TAuth.AuthOptions.Microsoft && droptime > 300000) ReauthenticateMs(droptime);
            if (Config.v.EnableBearerRefreshing && loginMethod == TAuth.AuthOptions.Mojang && droptime > 300000) ReauthenticateMojang(droptime);

            // get milliseconds to sleep
            int msToSleep = (int)TimeSpan.FromMilliseconds(droptime).TotalMilliseconds;

            // print snipe times, if debug is on
            if (Config.v.debug) {
                var now = DateTime.Now;

                // construct string to print
                // take away current milliseconds from the milliseconds to sleep
                int msToSleepWithoutCurrentMs = (msToSleep <= 0 ? 0 : msToSleep - now.Millisecond);

                // first packet
                string toPrint = $"Should snipe {SetText.Blue}{name}{SetText.ResetAll} @ {SetText.Blue}{now.AddMilliseconds(msToSleepWithoutCurrentMs).Second}.{now.AddMilliseconds(msToSleepWithoutCurrentMs).Millisecond}s";

                // rest of the packets, if packets arent awaited
                if (!Config.v.awaitFirstPacket && !Config.v.awaitPackets)
                    for (int i = 1; i < Config.v.SendPacketsCount; i++)
                        toPrint +=  (((i + 1) == Config.v.SendPacketsCount) ? $"{SetText.ResetAll} & {SetText.Gray}" : $"{SetText.ResetAll}, {SetText.Gray}") +
                                    $"{now.AddMilliseconds(msToSleepWithoutCurrentMs + (DataTypes.Config.v.PacketSpreadMs * i)).Second}." +
                                    $"{now.AddMilliseconds(msToSleepWithoutCurrentMs + (DataTypes.Config.v.PacketSpreadMs * i)).Millisecond}s";
                Cli.Output.Inform(toPrint);
            }

            // actually wait for the time
            // sleep for msToSleep without the current ms and take away 75ms which will be waited for in Name.Change
            Thread.Sleep((msToSleep <= (75 + DateTime.Now.Millisecond) ? 0 : ((msToSleep - DateTime.Now.Millisecond) - 75)));
            countDown.Cancel();
            Snipesharp.snipeTime = DateTime.Now.AddMilliseconds(75);
        }

        public static async Task ReauthenticateMs(long waitTime) {
            // sleep until 5 mins before
            await Task.Delay((int)waitTime - 299990);

            FS.FileSystem.Log("Refreshing bearer");
            var result = await Auth.AuthMicrosoft(Account.v.MicrosoftEmail, Account.v.MicrosoftPassword);
            Account.v.Bearer = result.bearer;
            FS.FileSystem.UpdateAccount();

            SetText.DisplayCursor(false);
        }
        public static async Task ReauthenticateMojang(long waitTime) {
            // sleep until 5 mins before
            await Task.Delay((int)waitTime - 299990);

            FS.FileSystem.Log("Refreshing bearer");
            var bearer = await Auth.AuthMojang(Account.v.MojangEmail, Account.v.MojangPassword);
            if (bearer != null) {
                Account.v.Bearer = bearer;
                FS.FileSystem.UpdateAccount();
            }

            SetText.DisplayCursor(false);
        }
    }
}
