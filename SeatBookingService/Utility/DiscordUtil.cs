using Newtonsoft.Json;
using SeatBookingService.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SeatBookingService.Utility
{
    public class DiscordUtil
    {
        //private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> SendMessageError(ErrorDetails content)
        {
            bool result = false;
            string url = "https://discord.com/api/webhooks/992325856645107743/A-OsFVxx6Sz5ccG_bFRFC_SUANlam8B-JahmR0oc11ZdOPW2AC3vo0nZlWNhD42VYD-H";

            try
            {
                using var client = new HttpClient();

                DiscordParam param = new DiscordParam();

                string stack = Regex.Replace(content.stack_trace.Replace(@"\", @"\\"), @"\t|\n|\r", "");

                string message = @"**__" + content.datetimenow + "__** "+
                    "\nUsername: " + content.username + 
                    "\nPath : " + content.path +
                    "\nMethod : " + content.status_code +
                    "\nStatus Code : " + content.status_code +
                    "\nMessage : " + content.message +
                    "\nStack Trace : " + stack.Substring(0, 300)
                    ;

                param.content = message;

                var response = await client.SendAsync(UrlRequest(HttpMethod.Post, new Uri(url), JsonConvert.SerializeObject(param)));
                
                result = true;

            }catch(Exception ex)
            {
                result = false;
            }

            return result;
        }

        public static HttpRequestMessage UrlRequest(HttpMethod method, Uri uri, string param)
        {
            return new HttpRequestMessage
            {
                Method = method,
                RequestUri = uri,
                Content = new StringContent(param, Encoding.UTF8, "application/json")
            };
        }
    }
}
