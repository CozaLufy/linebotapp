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
        [Route]
        /*
        [HttpGet]
        public IEnumerable<string> Get()
        {
            Trace.TraceInformation("request Get ");
            
             System.Threading.Thread.Sleep(3000);
             List<SendMessage> msgs = new List<SendMessage>();
             var msgTxt = new SendMessage() { type = "text", text = "Please wait" };
             msgs.Add(msgTxt);
             ReplyBody rb = new ReplyBody()
             {
                 replyToken = "0ac6df88f38a4cca8b8473728e61355b",
                 messages = msgs
             };
             var message = JsonConvert.SerializeObject(rb);
             Trace.TraceInformation("message " + message);
             Trace.TraceInformation("request Get Sleep");  
             
            List<PostbackTemplateAction> postBackAction = new List<PostbackTemplateAction>();
            postBackAction.Add(new PostbackTemplateAction { type = "postback", label = "Buy", data = "action=buy&itemid=123" });
            postBackAction.Add(new PostbackTemplateAction { type = "postback", label = "Buy", data = "action=buy&itemid=123" });
            Template template = new Template() {
                type = "buttons",
                thumbnailImageUrl = "https://example.com/bot/images/image.jpg",
                title = "Menu",
                text = "Please select",
                actions = postBackAction
            };
            ButtonsTemplate buttonsTemplate = new ButtonsTemplate { type = "template", altText = "this is a buttons template" , template = template };
            Trace.TraceInformation("buttonsTemplate " + JsonConvert.SerializeObject(buttonsTemplate));
            return new string[] { "value1", "value2" };
        }
         */     

        [HttpPost]       
        [Signature]
        public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
        {
            if (request != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                Trace.TraceInformation("request content " + content);
                LineWebhookModels dataEvent = JsonConvert.DeserializeObject<LineWebhookModels>(content);
                /*
                List<SendMessage> msgWait = new List<SendMessage>();
                var msgTxtWait = new SendMessage() { type = "text", text = "Please wait" };
                msgWait.Add(msgTxtWait);
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
                    
                    ReplyBody rbMsgs = new ReplyBody()
                    {
                        replyToken = e.replyToken,
                        messages = msgs
                    };
                    /*
                      List<PostbackTemplateAction> postBackAction = new List<PostbackTemplateAction>();
                      postBackAction.Add(new PostbackTemplateAction { type = "postback", label = "Buy", data = "action=buy&itemid=123" });
                      postBackAction.Add(new PostbackTemplateAction { type = "postback", label = "Add to cart", data = "action=buy&itemid=123" });
                      //postBackAction.Add(new PostbackTemplateAction { type = "uri", label = "View detail", uri = "http://www.binaryoptionsu.com/wp-content/uploads/2012/12/Bear-Market.png" });
                      Template btnTemplate = new Template()
                      {
                          type = "buttons",
                          thumbnailImageUrl = "http://www.binaryoptionsu.com/wp-content/uploads/2012/12/Bear-Market.png",
                          title = "Menu",
                          text = "Please select",
                          actions = postBackAction
                      };
                      List<ButtonsTemplate> buttonsTemplate = new List<ButtonsTemplate>();
                      buttonsTemplate.Add(new ButtonsTemplate { type = "template", altText = "this is a buttons template", template = btnTemplate });
                      //Trace.TraceInformation("buttonsTemplate " + JsonConvert.SerializeObject(buttonsTemplate));
                      // msgs.Add(buttonsTemplate);
                      ReplyBodyTemplate rbMsgs = new ReplyBodyTemplate()
                      {
                          replyToken = e.replyToken,
                          messages = buttonsTemplate
                      };

                      //var messageWait = JsonConvert.SerializeObject(rbWait);
                      */
                    var message = JsonConvert.SerializeObject(rbMsgs);
                    //Trace.TraceInformation("messageWait " + messageWait);
                    //replyMessage(messageWait);
                    //System.Threading.Thread.Sleep(3000);
                    Trace.TraceInformation("message " + message);
                    replyMessage(message);

                }

                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.NotAcceptable);
        }

        public async void replyMessage(string message)
        {
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
