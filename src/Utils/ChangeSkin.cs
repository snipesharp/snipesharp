using System.Text.Json;

namespace Utils
{
    public class ChangeSkin
    {
        public static async Task Change(string skinUrl, string skinType, string bearer)
        {
            if (!string.IsNullOrEmpty(skinUrl) && !string.IsNullOrEmpty(skinType))
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer.Trim());
                    string json = JsonSerializer.Serialize(new
                    {
                        variant = skinType,
                        url = skinUrl
                    }, new JsonSerializerOptions { WriteIndented = true });
                    await client.PostAsync($"https://api.minecraftservices.com/minecraft/profile/skins",
                        new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
                }
            }
        }
    }
}
