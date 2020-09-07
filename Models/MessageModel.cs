using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace facebook.Models
{
    public class MessageModel
    {
        public string Nick { get; set; }
        public DateTime SendDate { get; set; }
        public string Message { get; set; }
    }
}