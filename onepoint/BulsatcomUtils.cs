using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace onepoint
{
    public class BulsatcomUtils
    {
        private const string Platform = "pcweb";
        private const string PlatformVersion = "0.01";
        private readonly string _baseUrl;
        private string _challengeKey;
        private string _ssbulsatapiKey;

        private JObject jsonData;


        public BulsatcomUtils(string baseUrl)
        {
            _baseUrl = baseUrl;
        }


        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            // Make the initial auth
            using (var client = new HttpClient())
            {
                #region Set Request Headers
                client.DefaultRequestHeaders.Add("Host", "api.iptv.bulsat.com");
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "bg-BG,bg;q=0.8,en;q=0.6");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
                client.DefaultRequestHeaders.Add("Referer", "https://test.iptv.bulsat.com/iptv-login.php");
                client.DefaultRequestHeaders.Add("Origin", "https://test.iptv.bulsat.com");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                #endregion Set Request Headers
                // First Auth request
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/auth"))
                {
                    var response = await client.SendAsync(request);
                    // Get response headers
                    if (!response.Headers.TryGetValues("challenge", out var values))
                        return false;
                    _challengeKey = values.FirstOrDefault();
                    if (!response.Headers.TryGetValues("ssbulsatapi", out values))
                        return false;
                    _ssbulsatapiKey = values.FirstOrDefault();
                    // Check if we are already logged
                    if (response.Headers.TryGetValues("logged", out values))
                    {
                        if (values.FirstOrDefault() == "true") return true;
                    }
                }
                // Second Auth request
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/auth"))
                {
                    client.DefaultRequestHeaders.Add("SSBULSATAPI", _ssbulsatapiKey);

                    var postValues = new Dictionary<string, string>()
                    {
                        {"user", username},
                        {"device_id", Platform},
                        {"device_name", Platform},
                        {"os_version", Platform},
                        {"os_type", Platform},
                        {"app_version", PlatformVersion},
                        {"pass", EncryptPassword(password)}
                    };
                    request.Content = new FormUrlEncodedContent(postValues);
                    var response = await client.SendAsync(request);
                    // Check if we are already logged
                    if (response.Headers.TryGetValues("logged", out var values))
                    {
                        if (values.FirstOrDefault() == "true") return true;
                    }
                }
            }

            return false;
        }

        
        public async Task<bool> ChannelAsync()
        {
            // Make the m3u call
            using (var client = new HttpClient())
            {
                #region Set Request Headers
                client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "ssbulsatapi");
                client.DefaultRequestHeaders.Add("SSBULSATAPI", _ssbulsatapiKey);
                #endregion Set Request Headers
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/tv/pcweb/live"))
                {
                    var response = await client.SendAsync(request);

                    // get m3u json data
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var data = response.Content.ReadAsStringAsync().Result;
                        jsonData = JObject.Parse(data);
                        
                        if (jsonData.Count > 0) return true;
                    }
                }
            }

            return false;
        }


        public async Task<bool> EPGAsync()
        {
            // request epg for all channels
            for (int i = 0; i < jsonData.Count; i++)
            {
                string epg_name = jsonData[i]["epg_name"].ToString();

                // Make the epg call
                using (var client = new HttpClient())
                {
                    #region Set Request Headers
                    client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "ssbulsatapi");
                    client.DefaultRequestHeaders.Add("SSBULSATAPI", _ssbulsatapiKey);
                    #endregion Set Request Headers
                    using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/epg/short"))
                    {
                        var postValues = new Dictionary<string, string>()
                        {
                            { "epg", "1week"},
                            { "channel", epg_name}
                        };

                        request.Content = new FormUrlEncodedContent(postValues);
                        var response = await client.SendAsync(request);

                        // get epg json data for channel
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            var data = response.Content.ReadAsStringAsync().Result;
                            JObject jsonItem = JObject.Parse(data);

                            // add program json to m3u list item json
                            jsonData[i]["program"] = jsonItem[0][1]["programme"];
                        }
                    }
                }
            }

            return true;
        }


        private string EncryptPassword(string password)
        {
            try
            {
                using (var crypto = Aes.Create())
                {
                    crypto.Mode = CipherMode.ECB;
                    crypto.BlockSize = 128;
                    crypto.Key = Encoding.ASCII.GetBytes(_challengeKey);
                    crypto.Padding = PaddingMode.None;

                    using (var enc = crypto.CreateEncryptor())
                    {
                        var paddedPassword = password + new string('\0', (16 - password.Length % 16));
                        var encryptedPassword = enc.TransformFinalBlock(
                            Encoding.ASCII.GetBytes(paddedPassword),
                            0,
                            paddedPassword.Length);
                        return Convert.ToBase64String(encryptedPassword);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return String.Empty;
        }
    }
}