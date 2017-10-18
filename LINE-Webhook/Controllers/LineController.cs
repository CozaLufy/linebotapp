﻿using LINE_Webhook.API.LINE;
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
using System.Web.Http;

namespace LINE_Webhook.Controllers
{
    [RoutePrefix("line")]
    public class LINEController : ApiController
    {
        
        [HttpPost]
        [Route]
        [Signature]
       
        public IHttpActionResult webhook([FromBody] LineWebhookModels data)
        {
            if (data == null) return BadRequest();
            if (data.events == null) return BadRequest();
            Trace.TraceInformation("data.events " + data.events.ToString());
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
 
        /*
         [HttpPost]
         [Route]
         public async Task<HttpResponseMessage> Post(HttpRequestMessage request)
         {
             //return Request.CreateResponse(HttpStatusCode.OK, WebConfigurationManager.AppSettings["ChannelSecret"]);

             //if (!await VaridateSignature(request))
                 //return Request.CreateResponse(HttpStatusCode.BadRequest);
             return Request.CreateResponse(HttpStatusCode.OK);

         }
         */
        /*
        private async Task<bool> VaridateSignature(HttpRequestMessage request)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(WebConfigurationManager.AppSettings["ChannelSecret"]));
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(await request.Content.ReadAsStringAsync()));
            var contentHash = Convert.ToBase64String(computeHash);
            var headerHash = Request.Headers.GetValues​​("X-Line-Signature").First();

            return contentHash == headerHash;
        }

        */

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


    }
}
