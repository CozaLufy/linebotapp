using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LINE_Webhook.Models
{
    public class PostbackTemplateAction
    {
        public string type { get; set; }
        public string label { get; set; }
        public string data { get; set; }
        public string uri { get; set; }
    }

    public class Template
    {
        public string type { get; set; }
        public string thumbnailImageUrl { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public List<PostbackTemplateAction> actions { get; set; }
    }

    public class ButtonsTemplate
    {
        public string type { get; set; }
        public string altText { get; set; }
        public Template template { get; set; }
    }

    public class ReplyBodyTemplate
    {
        public string replyToken { get; set; }
        public List<ButtonsTemplate> messages { get; set; }
    }
}