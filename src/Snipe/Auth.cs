using System.Text.Json;
using System.Net.Http.Headers;
using Cli.Animatables;

namespace Snipe
{
    public class Auth
    {

        /// <returns>true if the account owns Minecraft & can change its username</returns>
        public static async Task<bool> AuthWithBearer(string bearer) {
            bearer = bearer.Trim(); // todo: and regex to remove Bearer: :bearer etc
            if (bearer.Length < 280) DataTypes.Config.v.yggdrasilToken = true;
            if (!await IsWorkingBearer(bearer)) return false;
            if (!DataTypes.Config.v.yggdrasilToken) if (!await Utils.Stats.OwnsMinecraft(bearer)) Cli.Output.ExitError("Account doesn't own Minecraft");
            return true;
        }

        /// <returns>MC Bearer using Mojang credentials if successful, or null if not</returns>
        /// <summary>Currently doesn't support prename sniping</summary>
        public static async Task<string?> AuthMojang(string email, string password) {
            var spinner = new Spinner();

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Cli.Templates.TWeb.UserAgent);
            var response = await client.PostAsync("https://authserver.mojang.com/authenticate", MojangPayload.GetContent(MojangPayload.GenerateMojangPayload(email, password)));
            
            var responseJson = JsonSerializer.Deserialize<MojangResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { WriteIndented = true });

            spinner.Cancel();
            DataTypes.Config.v.yggdrasilToken = true;
            if (responseJson == null) return null;
            return responseJson.accessToken;
        }
        
        /// <returns>MC Bearer using Microsoft credentials if successful, or an empty MsAuthResult object if not</returns>
        public static async Task<MsAuthResult> AuthMicrosoft(string email, string password) {
            var spinner = new Spinner();

            // get post url and PPFT
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Cli.Templates.TWeb.UserAgent);
            HttpResponseMessage initGet = await client.GetAsync("https://login.live.com/oauth20_authorize.srf?client_id=000000004C12AE6F&redirect_uri=https://login.live.com/oauth20_desktop.srf&scope=service::user.auth.xboxlive.com::MBI_SSL&display=touch&response_type=token&locale=en");
            string GetResult = initGet.Content.ReadAsStringAsync().Result;
            string sFTTag = Validators.Auth.rSFTTagRegex.Matches(GetResult)[0].Value.Replace("value=\"", "").Replace("\"", "");
            string urlPost = Validators.Auth.rUrlPostRegex.Matches(GetResult)[0].Value.Replace("urlPost:'", "").Replace("'", "");

            var requestContent = new FormUrlEncodedContent(new[] {
                new KeyValuePair<string,string>("login",email),
                new KeyValuePair<string,string>("loginfmt",email),
                new KeyValuePair<string, string>("passwd",password),
                new KeyValuePair<string, string>("PPFT",sFTTag)
            });

            // make the post call
            var postHttpResponse = await client.PostAsync(urlPost, requestContent);

            // if it was successful
            if (postHttpResponse.RequestMessage!.RequestUri!.AbsoluteUri.Contains("access_token")){
                if (postHttpResponse.ToString().Contains("Sign in to")) { Cli.Output.ExitError("Wrong credentials, failed to login"); }
                if (postHttpResponse.ToString().Contains("Help us protect your account")) { Cli.Output.ExitError("2FA enabled, failed to login"); }
                
                //  parse url data to dictionary
                Dictionary<string, string> urlDataDictionary = new Dictionary<string, string>();
                var urlData = postHttpResponse.RequestMessage.RequestUri.AbsoluteUri.Split('#')[1].Split('&');
                for (int i = 0; i < urlData.Length; i++) urlDataDictionary.Add(
                    urlData[i].Split('=').First(), urlData[i].Split('=').Last()
                );

                if (urlDataDictionary.TryGetValue("access_token", out string? token)) {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent xboxPayloadContent = XboxPayload.GetContent(XboxPayload.GenerateXboxPayload(token));
                    var xboxPayloadJsonResponse = JsonSerializer.Deserialize<XboxResponse>
                        (client.PostAsync("https://user.auth.xboxlive.com/user/authenticate", xboxPayloadContent).Result.Content.ReadAsStringAsync().Result);

                    StringContent xstsPayloadContent = XboxPayload.GetContent(XboxPayload.GenerateXstsPayload(xboxPayloadJsonResponse!.Token!));
                     var xstsPayloadHttpResponse =
                        client.PostAsync("https://xsts.auth.xboxlive.com/xsts/authorize", xstsPayloadContent);
                     var xstsPayloadJsonResponse = JsonSerializer.Deserialize<XboxResponse>
                        (xstsPayloadHttpResponse.Result.Content.ReadAsStringAsync().Result);

                    if (xstsPayloadJsonResponse!.DisplayClaims == null){
                        Cli.Output.Error("Microsoft account not linked to an Xbox account");
                        return new MsAuthResult();
                    }

                     // Get MC Bearer
                    StringContent mcPayloadContent = McPayload.GetContent(McPayload.GenerateMcPayload(xstsPayloadJsonResponse!.DisplayClaims.xui![0].uhs!, xstsPayloadJsonResponse!.Token!));
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "https://api.minecraftservices.com/authentication/login_with_xbox");
                    requestMessage.Content = mcPayloadContent;
                        
                    var mcApiHttpResponse = client.SendAsync(requestMessage).Result.Content.ReadAsStringAsync();
                    var mcApiJsonResponse = JsonSerializer.Deserialize<McApiResponse>(mcApiHttpResponse.Result);
                    spinner.Cancel();

                    // Check if account owns MC
                     bool ownsMinecraft = await Utils.Stats.OwnsMinecraft(mcApiJsonResponse.access_token);
                    if (!ownsMinecraft) {
                        Cli.Output.ExitError("Account doesn't own Minecraft");
                    }

                    // Return if prename account
                    if (!await HasNameHistory(mcApiJsonResponse.access_token)) {
                        try { FS.FileSystem.Log($"Successfully authenticated prename Microsoft account & got ..{mcApiJsonResponse.access_token.ToString().Substring(mcApiJsonResponse.access_token.ToString().Length-6)}"); }
                        catch { FS.FileSystem.Log("Invalid bearer"); }
                        return new MsAuthResult{bearer = mcApiJsonResponse.access_token.ToString(), prename = true};
                    }

                    // Check if account can change name since its not a prename account
                    await Utils.Stats.CanChangeName(mcApiJsonResponse.access_token);

                    if (!String.IsNullOrEmpty(mcApiJsonResponse.access_token.ToString())) {
                        try { FS.FileSystem.Log($"Successfully authenticated Microsoft account & got ..{mcApiJsonResponse.access_token.ToString().Substring(mcApiJsonResponse.access_token.ToString().Length-6)}"); }
                        catch { FS.FileSystem.Log("Invalid bearer"); }
                        return new MsAuthResult{bearer = mcApiJsonResponse.access_token.ToString(), prename = false};
                    }
                }
                else Cli.Output.ExitError("Failed to get access_token");
            }
            else {
                // log the response for later debug
                spinner.Cancel();
                string result = await postHttpResponse.Content.ReadAsStringAsync();

                // handle error
                string error = "";
                if (result.Contains("That Microsoft account doesn\\'t exist")) error = "That Microsoft account doesn't exist";
                if (result.Contains("incorrect")) error = "Wrong password";
                if (result.Contains("Help us protect your account")) error = "Account 2-factor authentication enabled";
                if (result.Contains("Please enter the password for your Microsoft account")) error = "Password can't be empty";
                if (!result.Contains("Help us protect your account") && !result.Contains("Please enter the password for your Microsoft account") && !result.Contains("incorrect") && !result.Contains("That Microsoft account doesn\\'t exist")) error = $"Failed due to Microsoft suspecting suspicious activities. Try following this tutorial to fix this: {DataTypes.SetText.SetText.Cyan}https://github.com/snipesharp/snipesharp/wiki/How-to-fix-failed-Microsoft-login {DataTypes.SetText.SetText.ResetAll}";
                Cli.Output.Error(error);
                return new MsAuthResult{error = error};
            }
            return new MsAuthResult();
        }

        /// <summary>Verifies whether the given bearer can be used to Authorize</summary>
        public static async Task<bool> IsWorkingBearer(string bearer) {
            // create spinner
            Cli.Animatables.Spinner spinner = new Cli.Animatables.Spinner();
            
            // prepare http client
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", Cli.Templates.TWeb.UserAgent);

            // mojang bearer
            if (DataTypes.Config.v.yggdrasilToken) {
                StringContent content = new StringContent(JsonSerializer.Serialize(MojangTokenCheckPayload.GenerateMojangPayload(bearer), new JsonSerializerOptions {WriteIndented=true}), System.Text.Encoding.UTF8, "application/json");
                var mojangAuthHttpResponse = await client.PostAsync("https://authserver.mojang.com/validate", content);

                // return Mojang response
                spinner.Cancel();
                return mojangAuthHttpResponse.IsSuccessStatusCode;
            }

            // get MS json response
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var mcOwnershipHttpResponse = await client.GetAsync("https://api.minecraftservices.com/entitlements/mcstore");

            // return MS response
            spinner.Cancel();
            return mcOwnershipHttpResponse.IsSuccessStatusCode;
        }
        
        ///<returns>True if user has name history, false otherwise</returns>
        public async static Task<bool> HasNameHistory(string bearer) {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);
            var response = await client.GetAsync("https://api.minecraftservices.com/minecraft/profile");
            return (int)response.StatusCode == 200;
        }

        /// <summary>Redeems a giftcard from given giftcode.</summary>
        /// <returns>True or false based on success</returns>
        public async static Task<bool> RedeemGiftcard(string giftcode, string bearer) {
            // prepare the http request
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:97.0) Gecko/20100101 Firefox/97.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearer);

            // 
            var spinner = new Spinner();
            var response = await client.PutAsync($"https://api.minecraftservices.com/productvoucher/:{giftcode}", null);
            spinner.Cancel();
            if ((int)response.StatusCode!=200) Cli.Output.Error("Failed to redeem giftcard, try again");
            return (int)response.StatusCode==200;
        }
    }
    public class MojangTokenCheckPayload {
        public string? accessToken {get; set;}
        public static MojangTokenCheckPayload GenerateMojangPayload(string accessToken) {
            return new MojangTokenCheckPayload {
                accessToken = accessToken
            };
        }
    }
    public class MojangResponse {
        public string? accessToken {get;set;}
    }
    public class MojangPayload {
        public MojangAgent? agent {get;set;}
        public string? username {get;set;}
        public string? password {get;set;}
        public string? clientToken {get;set;}
        public string? requestUser {get;set;}
        public static MojangPayload GenerateMojangPayload(string email, string password) {
            return new MojangPayload {
                agent = new MojangAgent{
                    name = "Minecraft",
                    version = 1
                },
                username = email,
                password = password,
                clientToken = "Mojang-API-Client",
                requestUser = "true"
            };
        }
        public static StringContent GetContent(MojangPayload mojangPayload) {
            return new StringContent
                (JsonSerializer.Serialize(
                    mojangPayload,
                    new JsonSerializerOptions { WriteIndented = true }
                ), System.Text.Encoding.UTF8, "application/json");
        }
    }
    public class MojangAgent {
        public string? name {get;set;}
        public int version {get;set;}
    }
    public class McPayload {
        public string? identityToken { get; set; }
        public bool ensureLegacyEnabled { get; set; }
        public static McPayload GenerateMcPayload(string userHash, string xstsToken) {
            return new McPayload {
                identityToken = $"XBL3.0 x={userHash};{xstsToken}",
                ensureLegacyEnabled = true
            };
        }
        public static StringContent GetContent(McPayload payload) {
            return new StringContent(
                JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented=true}),
                System.Text.Encoding.UTF8, "application/json");
        }
    }
    public class XboxPayload {
        public object? Properties { get; set; }
        public string? RelyingParty { get; set; }
        public string? TokenType { get; set; }
        public static XboxPayload GenerateXboxPayload(string token) {
            return new XboxPayload {
                Properties = new {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = token
                },
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT"
            };
        }

        public static XboxPayload GenerateXstsPayload(string token) {
            return new XboxPayload {
                Properties = new {
                    SandboxId = "RETAIL",
                    UserTokens = new[] { token }
                },
                RelyingParty = "rp://api.minecraftservices.com/",
                TokenType = "JWT"
            };
        }
        public static StringContent GetContent(XboxPayload xboxPayload) {
            return new StringContent
                (JsonSerializer.Serialize(
                    xboxPayload,
                    new JsonSerializerOptions { WriteIndented = true }
                ), System.Text.Encoding.UTF8, "application/json");
        }
    }

    public struct MsAuthResult {
        public string bearer { get;set; }
        public bool prename { get;set; }
        public string error {get;set;}
    }
    public struct McApiResponse {
        public string username { get; set; }
        public object roles { get; set; }
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class XboxResponse {
        public string? IssueInstant { get; set; }
        public string? NotAfter { get; set; }
        public string? Token { get; set; }
        public DisplayClaimsObject? DisplayClaims { get; set; }
    }

    public class DisplayClaimsObject { public XuiObject[]? xui { get; set; } }
    public class XuiObject { public string? uhs { get; set; } }
}