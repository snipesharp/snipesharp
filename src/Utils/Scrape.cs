using Cli.Animatables;
using System.Text.RegularExpressions;
using System.Text.Json;
using DataTypes.SetText;

namespace Utils
{
    public class Scrape
    {
        public static async Task<List<string>> Get3LetterNames(){
            string htmlContent = await GetHtmlContent("https://3name.xyz/list");
            Regex rx = new Regex("<div class=\"username-list-item username-list-item-timer\">(.+)<\\/div>");

            // get all matches and convert them to a list
            var matches = rx.Matches(htmlContent)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .ToList();

            return matches;
        }

        private static async Task<string> GetHtmlContent(string url){
            var spinner = new Spinner();

            // make the actual get request
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(url);
            
            // return the response
            spinner.Cancel();
            return response.Content.ReadAsStringAsync().Result;
        }

        public static async Task<string> getPopularName() {
            // json
            var json = JsonSerializer.Serialize(new {
                minSearches = DataTypes.Config.v.PopSearches,
                length = DataTypes.Config.v.PopLength,
                lengthOption = DataTypes.Config.v.PopLengthOption,
                language = DataTypes.Config.v.PopLanguage
            });

            // string content
            StringContent jsonContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // request until success
            HttpClient client = new HttpClient();
            while (true) {
                var response = await client.PostAsync("http://api.snipesharp.xyz:5150/getdropping", jsonContent);

                // return if successful
                if (response.IsSuccessStatusCode) {
                    var deserialized = JsonSerializer.Deserialize<GetDroppingAPI>(await response.Content.ReadAsStringAsync());
                    Cli.Output.Inform($"{SetText.Bold}{SetText.Blue}{deserialized.name}{SetText.ResetAll} has {SetText.Blue}{deserialized.searches}{SetText.ResetAll} NameMC searches");

                    return DataTypes.Config.v.PopLowercaseOnly ? deserialized.name.ToLower() : deserialized.name;
                }
                
                Cli.Output.Error("Failed to fetch popular name" + ((((int)response.StatusCode) == 429) ? " due to rate limiting" : "") + ", trying again in ~3 minutes");
                Thread.Sleep(((1000 * 60) * 3) + new Random().Next(2000, 10000)); // add randomness so that in case of multiple instances, not all instances send requests at the same time
            }
        }
    }
    public class GetDroppingAPI {
        public string? name {get;set;}
        public int? searches {get;set;}
        public string? timeAvailable {get;set;}
        public string? dateAvailable {get;set;}
    }
}