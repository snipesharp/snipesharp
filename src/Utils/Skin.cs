using System.Text.Json;
using System.Net.Http.Headers;

namespace Utils
{
    public class Skin
    {
        public static async void Change(string skinUrl, string skinType, string bearer) {
            if (string.IsNullOrEmpty(skinUrl)) skinUrl = DataTypes.Config.v.defaultSkin;
            if (string.IsNullOrEmpty(skinType)) skinType = "classic";
            
            // prepare http client
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer.Trim());
            string json = JsonSerializer.Serialize(
                new {url = skinUrl, variant = skinType }, new JsonSerializerOptions { WriteIndented = true });

            // make the actual call for changing the skin
            var result = await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile/skins",
            new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
            FS.FileSystem.Log($"Auto skin change: {result}");
        }
    }
}
