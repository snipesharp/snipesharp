using System.Text.Json;
using System.Net.Http.Headers;

namespace Snipe
{
    public class Droptime
    {
        // define all endpoints here
        private static Func<string, string> UrlCkm = name => $"http://api.coolkidmacho.com/droptime/{name}";
        private static Func<string, string> UrlStar = name => $"https://api.star.shopping/droptime/{name}";

        // define json return types
        private struct UnixJSON { public int unix { get; set; } }
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

        public static async Task<int> GetMilliseconds(string username){
            var ckmData = await Fetch<UnixJSON>(UrlCkm(username));
            var starData = await Fetch<UnixJSON>(UrlStar(username));
            int timestamp = Math.Max(ckmData.unix, starData.unix);

            // couldn't find the timestamp
            if(timestamp == 0) Cli.Output.ExitError(Cli.Errors.NoDroptime(username));

            // convert timstamp to time left (in ms) and return
            int now = (int) new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            int msLeft = Math.Max(0, timestamp - now);
            return msLeft;
        }
    }
}