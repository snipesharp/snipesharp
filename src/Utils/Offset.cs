using System.Net.NetworkInformation;

namespace Utils
{
    public class Offset
    {
        // ! 0 ping with vpns
        public static async Task<int> CalcSuggested(){
            // https://serverfault.com/questions/307946/measure-server-speed-and-deduct-the-network-latency   
            return (int)(await AveragePing() * 1.5);
        }

        private static async Task<int> AveragePing(){
            Ping ping = new Ping();
            double[] pings = new double[4];
            for(int i = 0; i < 4; i++){
                PingReply result = await ping.SendPingAsync("api.minecraftservices.com");
                // if for some reason the api doesn't work exit with error
                if(result.Status != IPStatus.Success) Cli.Output.ExitError("Cannot ping minecraftservices api.");
                pings[i] = result.RoundtripTime;
            }
            return (int)pings.ToList().Average();
        }
    }
}