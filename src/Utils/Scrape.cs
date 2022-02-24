using Cli.Animatables;
using System.Text.RegularExpressions;

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
    }
}