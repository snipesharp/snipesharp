namespace Snipe
{
    public class Auth
    {
        public static async Task<string> AuthWithBearer(string bearer)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer.Trim());
                HttpResponseMessage response = await client.GetAsync("https://api.minecraftservices.com/minecraft/profile/namechange");
                HttpContent content = response.Content;
                return content.ReadAsStringAsync().Result;
            }
        }
        // todo
        public static async Task<string> AuthMojang(string email, string password, string sq1, string sq2, string sq3)
        {
            return "";
        }
    }
}