using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using facebook.Models;
using facebook.Repository;
using Microsoft.AspNet.SignalR;

namespace facebook
{
    public class ChatHub : Hub
    {
        public void Send(string username,string userId,string aliciAdi, string aliciId, string message)
        {
            var rep = new FacebookRepository();
            try
            {
                rep.SohbetKaydet(userId, aliciId, message);
            }
            catch (Exception e)
            {
               // sohbet kaydedilirken bir hata oluştu.
            }

            var messageModel = new MessageModel();
            messageModel.Nick = username;
            messageModel.SendDate = DateTime.Now;
            messageModel.Message = message;

            var messageJson = new JavaScriptSerializer().Serialize(messageModel);

            Clients.All.sendMessage(username, userId, aliciAdi, aliciId, messageJson);
        }
    }
}