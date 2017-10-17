using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace LineBotApp.Controllers
{
    public class CallbackController : ApiController
    {
        public async Task<string> Post(JToken token)
        {
            foreach (var msg in token["result"])
            {
                var message = JsonConvert.SerializeObject(new
                {
                    to = new[] { msg["content"]["from"] },
                    toChannel = 1383378250,
                    eventType = "138311608800106203",
                    content = msg["content"]
                });

                var uri = "https://trialbot-api.line.me/v1/events";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=UTF-8");
                    client.DefaultRequestHeaders.Add("X-Line-ChannelID", "");
                    client.DefaultRequestHeaders.Add("X-Line-ChannelSecret", "");
                    client.DefaultRequestHeaders.Add("X-Line-Trusted-User-With-ACL", "");

                    var content = new StringContent(message, Encoding.UTF8, "application/json");

                    var result = await client.PostAsync(uri, content);

                    if (!result.IsSuccessStatusCode)
                    {
                        throw new LineRequestException(result);
                    }
                }
            }

            return "ok";
        }

        public class LineRequestException : Exception
        {
            public override string Message { get; }

            public LineRequestException(HttpResponseMessage response)
            {
                var statusCode = (int)response.StatusCode;
                var reasonPhrase = response.ReasonPhrase;
                var content = response.Content.ReadAsStringAsync().Result;

                Message = string.Join(Environment.NewLine,
                    $"StatusCode: {statusCode}",
                    $"ReasonPhrase: {reasonPhrase}",
                    $"Content: {content}");
            }
        }
    }
}
