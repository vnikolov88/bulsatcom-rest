using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using onepoint.Models;

namespace onepoint
{
    public static class DefaultRequestHeadersEx
    {
        public static void AddBulsatcomHeaders(this HttpRequestHeaders headers)
        {
            #region Set Request Headers
            headers.Add("Host", "api.iptv.bulsat.com");
            headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36");
            headers.Add("Accept", "*/*");
            headers.Add("Accept-Language", "bg-BG,bg;q=0.8,en;q=0.6");
            headers.Add("Accept-Encoding", "utf-8");
            headers.Add("Referer", "https://test.iptv.bulsat.com/iptv-login.php");
            headers.Add("Origin", "https://test.iptv.bulsat.com");
            headers.Add("Connection", "keep-alive");
            #endregion Set Request Headers
        }
    }
    

    public class BulsatcomUtils
    {
        private const string Platform = "pcweb";
        private const string PlatformVersion = "0.01";
        private readonly string _baseUrl;
        private string _challengeKey;
        private string _ssbulsatapiKey;


        public BulsatcomUtils(string baseUrl)
        {
            _baseUrl = baseUrl;
        }


        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            // Make the initial auth
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.AddBulsatcomHeaders();
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
        
        public async Task<List<ChannelModel>> ChannelAsync()
        {
            // Make the channels call
            using (var client = new HttpClient())
            {
                #region Set Request Headers
                client.DefaultRequestHeaders.AddBulsatcomHeaders();
                client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "ssbulsatapi");
                client.DefaultRequestHeaders.Add("SSBULSATAPI", _ssbulsatapiKey);
                client.DefaultRequestHeaders.Add("accept", "Application/JSON");
                #endregion Set Request Headers
                using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/tv/pcweb/live"))
                {
                    var response = await client.SendAsync(request);
                    
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = response.Content.ReadAsStringAsync().Result;

                        var channels = JsonConvert.DeserializeObject<List<ChannelModel>>(result);
                        return channels;
                    }
                }
            }

            return null;
        }


        public async Task<List<ChannelModel>> EPGAsync(List<ChannelModel> channels)
        {
            // request epg for every channel
            for (int i = 0; i < channels.Count; i++)
            {
                // make call only if there is program for nownext in channel which come whit channale request
                if (channels[i].program != null && channels[i].program.title.Length > 0)
                {
                    using (var client = new HttpClient())
                    {
                        #region Set Request Headers
                        client.DefaultRequestHeaders.AddBulsatcomHeaders();
                        client.DefaultRequestHeaders.Add("Access-Control-Request-Headers", "ssbulsatapi");
                        client.DefaultRequestHeaders.Add("SSBULSATAPI", _ssbulsatapiKey);
                        #endregion Set Request Headers
                        using (var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/epg/short"))
                        {
                            // 'nownext' / '1day' / '1week'
                            var postValues = new Dictionary<string, string>()
                        {
                            { "epg", "1day"},
                            { "channel", channels[i].epg_name}
                        };

                            request.Content = new FormUrlEncodedContent(postValues);
                            var response = await client.SendAsync(request);

                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                var result = response.Content.ReadAsStringAsync().Result;

                                var epg = JsonConvert.DeserializeObject<EpgModel>(JObject.Parse(result)[channels[i].epg_name].ToString());

                                // fix for missing epg dummy records from server which have epg record, but dont have epg data
                                if (epg.programme.Count > 0 && epg.programme[0].title.Length > 0)
                                {
                                    channels[i].epg = epg;
                                }
                            }
                        }
                    }
                }
            }

            return channels;
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