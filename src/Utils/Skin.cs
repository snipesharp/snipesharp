using System.Text.Json;
using System.Net.Http.Headers;

namespace Utils
{
    public class Skin
    {
        public static async Task Change(string skinUrl, string skinType, string bearer) {
            if (string.IsNullOrEmpty(skinUrl) || string.IsNullOrEmpty(skinType)) return;
            
            // prepare http client
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer.Trim());
            string json = JsonSerializer.Serialize(
                new {url = skinUrl, variant = skinType }, new JsonSerializerOptions { WriteIndented = true });

            // make the actual call for changing the skin
            await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile/skins",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        }
    }
}
