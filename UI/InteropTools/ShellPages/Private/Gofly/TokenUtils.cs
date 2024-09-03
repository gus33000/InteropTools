using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace PhotoshoppedUUPCLI
{
    public class TokenUtils
    {
        public async static Task<string> GetTokenForMSA(string Mail, string Password)
        {
            var token = await GetBearerTokenForScope(Mail, Password, $"service::dcat.update.microsoft.com::MBI_SSL");
            var plainTextBytes = Encoding.Unicode.GetBytes("t=" + token + "&p="); //.TrimEnd('=')
            string encodedText = Convert.ToBase64String(plainTextBytes);
            return encodedText;
        }

        public async static Task<string> GetBearerTokenForScope(string Mail, string Password, string TargetScope, string ClientId = "ms-app://s-1-15-2-1929064262-2866240470-255121345-2806524548-501211612-2892859406-1685495620/")
        {
            string retVal = string.Empty;
            Mail = WebUtility.UrlEncode(Mail);
            Password = WebUtility.UrlEncode(Password);
            TargetScope = WebUtility.UrlEncode(TargetScope);
            ClientId = WebUtility.UrlEncode(ClientId);
            HttpWebRequest hwreq = (HttpWebRequest)WebRequest.Create($"https://login.live.com/oauth20_authorize.srf?client_id={ClientId}&scope={TargetScope}&response_type=token&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf");
            string MSPOK = string.Empty;
            string PPFT = string.Empty;
            string urlPost = string.Empty;
            
            try
            {
                HttpWebResponse hwresp = (HttpWebResponse)(await hwreq.GetResponseAsync());
                
                foreach (string oCookie in hwresp.Headers["Set-Cookie"].Split(','))
                {
                    if (oCookie.Trim().StartsWith("MSPOK"))
                    {
                        MSPOK = oCookie.Trim().Substring(6, oCookie.IndexOf(';') - 6);
                        MSPOK = WebUtility.UrlEncode(MSPOK);
                        break;
                    }
                }
                
                string responsePlain = string.Empty;
                using (var reader = new StreamReader(hwresp.GetResponseStream(), Encoding.UTF8))
                {
                    responsePlain = reader.ReadToEnd();
                }
                PPFT = responsePlain.Substring(responsePlain.IndexOf("name=\"PPFT\""));
                PPFT = PPFT.Substring(PPFT.IndexOf("value=") + 7);
                PPFT = PPFT.Substring(0, PPFT.IndexOf('\"'));
                urlPost = responsePlain.Substring(responsePlain.IndexOf("urlPost:") + 9);
                urlPost = urlPost.Substring(0, urlPost.IndexOf('\''));
            }
            catch { return string.Empty; }

            HttpClientHandler httpClientHandler = new HttpClientHandler();
            httpClientHandler.AllowAutoRedirect = false;

            CookieContainer hwreqCC = new CookieContainer();
            hwreqCC.Add(new Uri("https://login.live.com"), new Cookie("MSPOK", MSPOK) { Domain = "login.live.com" });
            httpClientHandler.CookieContainer = hwreqCC;
            
            var client = new HttpClient(httpClientHandler);

            StringContent queryString = new StringContent($"login={Mail}&passwd={Password}&PPFT={PPFT}", Encoding.UTF8);

            queryString.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            byte[] POSTByteArray = Encoding.UTF8.GetBytes($"login={Mail}&passwd={Password}&PPFT={PPFT}");
            queryString.Headers.ContentLength = POSTByteArray.Length;

            var hwresp2 = await client.PostAsync(new Uri(urlPost), queryString);
            
            try
            {
                foreach (string oLocationBit in hwresp2.Headers.Location.AbsoluteUri.Split('&'))
                {
                    if (oLocationBit.Contains("access_token"))
                    {
                        retVal = oLocationBit.Substring(oLocationBit.IndexOf("access_token") + 13);
                        if (retVal.Contains("&"))
                            retVal = retVal.Substring(0, retVal.IndexOf('&'));
                        break;
                    }
                }
            }
            catch { return string.Empty; }
            return WebUtility.UrlDecode(retVal);
        }
    }
}
