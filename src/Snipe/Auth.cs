using System.Text.RegularExpressions;
using System.Text.Json;

namespace Snipe
{
    public class Auth
    {
        public static async Task<bool> AuthWithBearer(string bearer) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearer.Trim());
            HttpResponseMessage response = await client.GetAsync("https://api.minecraftservices.com/minecraft/profile/namechange");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode) return true;
            else return false;
        }
        
        /// <returns>MC Bearer using Microsoft credentials</returns>
        public static async Task<string> AuthMicrosoft(string email, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0");

                HttpResponseMessage initGet = await client.GetAsync("https://login.live.com/oauth20_authorize.srf?client_id=000000004C12AE6F&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=service::user.auth.xboxlive.com::MBI_SSL&display=touch&response_type=token&locale=en");
                string sFTTag = Validators.Auth.rSFTTagRegex.Matches(initGet.Content.ReadAsStringAsync().Result)[0].Value.Replace("value=\"", "").Replace("\"", "");
                string urlPost = Validators.Auth.rUrlPostRegex.Matches(initGet.Content.ReadAsStringAsync().Result)[0].Value.Replace("urlPost:'", "").Replace("'", "");

                var requestContent = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string,string>("login",email),
                    new KeyValuePair<string,string>("loginfmt",email),
                    new KeyValuePair<string, string>("passwd",password),
                    new KeyValuePair<string, string>("PPFT",sFTTag)
                });

                var postHttpResponse = await client.PostAsync(urlPost, requestContent);
                if (postHttpResponse.RequestMessage.RequestUri.AbsoluteUri.Contains("access_token"))
                {
                    if (postHttpResponse.ToString().Contains("Sign in to")) { Cli.Output.ExitError("Wrong credentials, failed to login"); }
                    if (postHttpResponse.ToString().Contains("Help us protect your account")) { Cli.Output.ExitError("2FA enabled, failed to login"); }
                    string urlData = postHttpResponse.RequestMessage.RequestUri.AbsoluteUri.Split('#')[1];
                    Dictionary<string, string> urlDataDictionary = new Dictionary<string, string>();
                    for (int i = 0; i < urlData.Split('&').Length; i++)
                    {
                        urlDataDictionary.Add(urlData.Split('&')[i].Split('=').First(), urlData.Split('&')[i].Split('=').Last());
                    }
                    if (urlDataDictionary.TryGetValue("access_token", out string token))
                    {
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                        StringContent xboxPayloadContent = XboxPayload.GetContent(XboxPayload.GenerateXboxPayload(token));
                        var xboxPayloadJsonResponse = JsonSerializer.Deserialize<XboxResponse>
                            (client.PostAsync("https://user.auth.xboxlive.com/user/authenticate", xboxPayloadContent).Result.Content.ReadAsStringAsync().Result);

                        StringContent xstsPayloadContent = XboxPayload.GetContent(XboxPayload.GenerateXstsPayload(xboxPayloadJsonResponse.Token));
                        var xstsPayloadHttpResponse =
                            client.PostAsync("https://xsts.auth.xboxlive.com/xsts/authorize", xstsPayloadContent);
                        var xstsPayloadJsonResponse = JsonSerializer.Deserialize<XboxResponse>
                            (xstsPayloadHttpResponse.Result.Content.ReadAsStringAsync().Result);

                        StringContent mcPayloadContent = McPayload.GetContent(McPayload.GenerateMcPayload(xstsPayloadJsonResponse.DisplayClaims.xui[0].uhs, xstsPayloadJsonResponse.Token));
                        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.minecraftservices.com/authentication/login_with_xbox");
                        requestMessage.Content = mcPayloadContent;

                        var mcApiHttpResponse =
                            client.SendAsync(requestMessage).Result.Content.ReadAsStringAsync();
                        var mcApiJsonResponse = JsonSerializer.Deserialize<McApiResponse>
                            (mcApiHttpResponse.Result);

                        if (!String.IsNullOrEmpty(mcApiJsonResponse.access_token.ToString()) && await AuthWithBearer(mcApiJsonResponse.access_token.ToString())) return mcApiJsonResponse.access_token.ToString();
                    }
                    else Cli.Output.ExitError("Failed to get access_token");
                }
                else
                {
                    Cli.Output.Error($"Failed to login possibly due to Microsoft suspecting suspicious activities. Try following this tutorial to fix this: {DataTypes.SetText.SetText.Cyan}https://github.com/snipesharp/snipesharp/wiki/How-to-fix-failed-Microsoft-login {DataTypes.SetText.SetText.ResetAll}");
                }
            }
            return null;
        }
        // todo
        public static async Task<bool> AuthMojang(string email, string password, string sq1, string sq2, string sq3)
        {
            return false;
        }
    }
    public class McApiResponse
    {
        public string username { get; set; }
        public object roles { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }
    public class McPayload
    {
        public string identityToken { get; set; }
        public bool ensureLegacyEnabled { get; set; }
        public static McPayload GenerateMcPayload(string userHash, string xstsToken)
        {
            return new McPayload
            {
                identityToken = $"XBL3.0 x={userHash};{xstsToken}",
                ensureLegacyEnabled = true
            };
        }
        public static StringContent GetContent(McPayload payload)
        {
            return new StringContent(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented=true}), System.Text.Encoding.UTF8, "application/json");
        }
    }
    public class XboxPayload
    {
        public object Properties { get; set; }
        public string RelyingParty { get; set; }
        public string TokenType { get; set; }
        public static XboxPayload GenerateXboxPayload(string token)
        {
            return new XboxPayload
            {
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = token
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            };
        }
        public static XboxPayload GenerateXstsPayload(string token)
        {
            return new XboxPayload
            {
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[]
                    {
                        token
                    }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            };
        }
        public static StringContent GetContent(XboxPayload xboxPayload)
        {
            return new StringContent
                (JsonSerializer.Serialize(xboxPayload, new JsonSerializerOptions { WriteIndented = true }), System.Text.Encoding.UTF8, "application/json");
        }
    }
    public class XboxResponse
    {
        public string IssueInstant { get; set; }
        public string NotAfter { get; set; }
        public string Token { get; set; }
        public DisplayClaimsObject DisplayClaims { get; set; }
    }
    public class DisplayClaimsObject
    {
        public XuiObject[] xui { get; set; }
    }
    public class XuiObject
    {
        public string uhs { get; set; }
    }
}