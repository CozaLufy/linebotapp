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
    [RoutePrefix("line")]
    public class LINEController : ApiController
    {
        [HttpPost]
        //[HttpGet]
        [Route]
        [Signature]
        /*
         public IHttpActionResult webhook([FromBody] LineWebhookModels data)
         {
             if (data == null) return BadRequest();
             if (data.events == null) return BadRequest();
             Trace.TraceInformation("data.events " + JsonConvert.SerializeObject(data.events));
             foreach (Event e in data.events)
             {
                 if (e.type == EventType.message)
                 {
                     string senderID = "";
                     switch (e.source.type)
                     {
                         case SourceType.user:
                             senderID = e.source.userId;
                             break;
                         case SourceType.room:
                             senderID = e.source.roomId;
                             break;
                         case SourceType.group:
                             senderID = e.source.groupId;
                             break;
                     }
                     Trace.WriteLine("senderID " + senderID);
                     Trace.WriteLine("e.message.text " + e.message.text);
                 }
             }
             return Ok();
         }
*/
        public void Get()
        {
            string content = "{\"events\":[{\"type\":\"message\",\"replyToken\":\"568e2864472844f18c2c6c7a975f38fd\",\"source\":{\"userId\":\"U9064a2310b6a52cdbb0682912ba6179c\",\"type\":\"user\"},\"timestamp\":1508381966322,\"message\":{\"type\":\"text\",\"id\":\"6862464788030\",\"text\":\"Test\"}}]}";
            LineWebhookModels dataEvent = JsonConvert.DeserializeObject<LineWebhookModels>(content);
            /*
            foreach (Event e in dataEvent.events)
            {
                if (e.type == EventType.message)
                {
                    ReplyBody rb = new ReplyBody()
                    {
                        replyToken = e.replyToken,
                        messages = procMessage(e.message)
                    };
                    Trace.TraceInformation("rb " + JsonConvert.SerializeObject(rb));
                    //Reply reply = new Reply(rb);
                    //reply.send();
                }
            }
            */
        }
       
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (request != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                Trace.TraceInformation("request content " + content);
                LineWebhookModels dataEvent = JsonConvert.DeserializeObject<LineWebhookModels>(content);
                /*
                foreach (Event e in dataEvent.events)
                {
                    if (e.type == EventType.message)
                    {
                        ReplyBody rb = new ReplyBody()
                        {
                            replyToken = e.replyToken,
                            messages = procMessage(e.message)
                        };
                        Trace.TraceInformation("rb " + JsonConvert.SerializeObject(rb));
                        //Reply reply = new Reply(rb);
                        //reply.send();

                        var message = JsonConvert.SerializeObject(rb);

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
                }
                */
               
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
