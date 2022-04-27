using Cli.Templates;
using System.Text.Json;
using System.Net.Http.Headers;

namespace Snipe
{
    public class Droptime
    {
        // define all endpoints here
        private static Func<string, string> UrlStar = name => $"https://api.star.shopping/droptime/{name}";
        private static Func<string, string> UrlMojang = name => $"https://api.star.shopping/droptime/{name}";

        // define json return types
        public struct UnixJSON { public long? unix { get; set; } }
        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // handle fetching json data as object
        private static async Task<T?> Fetch<T>(string endPoint){
            HttpClient httpClient = new HttpClient();

            // prepare request headers
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Sniper");

            // return response or empty (T)
            try { return JsonSerializer.Deserialize<T>(await httpClient.GetStringAsync(endPoint), JsonOptions); }
            catch { return default(T); }
        }

        public static async Task<long> GetMilliseconds(string username, bool exitOnError=false){
            var snipesharpData = await Utils.Stats.GetUnixDroptime(username);
            // create starData but only fetch if our manual fetch is null
            UnixJSON starData = new UnixJSON();
            if (snipesharpData.unix == null) starData = await Fetch<UnixJSON>(UrlStar(username));
            // set timestamp
            int timestamp = Math.Max((int)snipesharpData.unix, (int)starData.unix);

            // couldn't find the timestamp
            Action<string> errorFunction = exitOnError ? Cli.Output.ExitError : Cli.Output.Error;
            if(timestamp == 0) errorFunction(TErrors.NoDroptime(username));

            // convert timstamp to time left (in ms) and return
            long now = DateTimeOffset.Now.ToUnixTimeSeconds();

            long msLeft = Math.Max(0, timestamp - now) * 1000;
            return msLeft;
        }
    }
}