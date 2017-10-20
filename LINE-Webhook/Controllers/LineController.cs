using LINE_Webhook.API.LINE;
using LINE_Webhook.Models;
using RUSE.API.LINE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace LINE_Webhook.Controllers
{
    [RoutePrefix("callback")]
    public class LINEController : ApiController
    {
        [HttpPost]
        [Route]
        [Signature]
       
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (request != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                Trace.TraceInformation("request content " + content);
                LineWebhookModels dataEvent = JsonConvert.DeserializeObject<LineWebhookModels>(content);               

                List<SendMessage> msgs = new List<SendMessage>();
                SendMessage sm = new SendMessage();
                foreach (Event e in dataEvent.events)
                {
                    sm.type = Enum.GetName(typeof(MessageType), e.type);
                    sm.text = e.message.text;
                    Trace.TraceInformation("sm " + JsonConvert.SerializeObject(sm));
                    msgs.Add(sm);
                    Trace.TraceInformation("msgs " + JsonConvert.SerializeObject(msgs));
                    ReplyBody rb = new ReplyBody()
                    {
                        replyToken = e.replyToken,
                        messages = msgs
                    };
                    

                    var message = JsonConvert.SerializeObject(rb);
                    Trace.TraceInformation("message " + message);
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WebConfigurationManager.AppSettings["AccessToken"]);

                        var dataString = new StringContent(message, Encoding.UTF8, "application/json");

                        var result = await client.PostAsync("https://api.line.me/v2/bot/message/reply", dataString);

                        if (!result.IsSuccessStatusCode)
                        {
                            throw new LineRequestException(result);
                        }
                    }

                }               


                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
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
                Trace.TraceInformation("LineRequestException " + Message);
            }
        }
       /*
        private List<SendMessage> procMessage(ReceiveMessage m)
        {
            List<SendMessage> msgs = new List<SendMessage>();
            SendMessage sm = new SendMessage()
            {
                type = Enum.GetName(typeof(MessageType), m.type)
            };
            switch (m.type)
            {
                case MessageType.sticker:
                    sm.packageId = m.packageId;
                    sm.stickerId = m.stickerId;
                    break;
                case MessageType.text:
                    sm.text = m.text;
                    break;
                default:
                    sm.type = Enum.GetName(typeof(MessageType), MessageType.text);
                    sm.text = "Sorry, I am just an echo robot, can only reply to the basic map and text message Oh!";
                    break;
            }
            msgs.Add(sm);
            return msgs;
        }
        */
    
    }
}
