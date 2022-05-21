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

            // request
            HttpClient client = new HttpClient();
            var response = await client.PostAsync("http://api.snipesharp.xyz:5150/getdropping", jsonContent);

            // handle failed request
            if (!response.IsSuccessStatusCode) Cli.Output.ExitError("Failed to fetch popular name");

            var deserialized = JsonSerializer.Deserialize<GetDroppingAPI>(await response.Content.ReadAsStringAsync());
            Cli.Output.Inform($"{SetText.Bold}{SetText.Blue}{deserialized.name}{SetText.ResetAll} has {SetText.Blue}{deserialized.searches}{SetText.ResetAll} NameMC searches");

            return deserialized.name;
        }
    }
    public class GetDroppingAPI {
        public string? name {get;set;}
        public int? searches {get;set;}
        public string? timeAvailable {get;set;}
        public string? dateAvailable {get;set;}
    }
}