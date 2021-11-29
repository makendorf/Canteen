
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AgsXMPP;
using AgsXMPP.Protocol.X.Muc;
using AgsXMPP.Protocol.Client;

namespace CanteetJabberBot
{
    class Program
    {
        static XmppClientConnection xmpp;
        static Jid Room;

        static void Main(string[] args)
        {
            string hostname = "bvkgroup.ru";
            string username = "shnyakin";
            string password = "rJB7XrE3";
            xmpp = new XmppClientConnection
            {
                Server = hostname,
                Port = 5222,
                Username = username,
                Password = password,
                Resource = "Work",
                Priority = 0,
                AutoResolveConnectServer = false,
                UseStartTLS = true,
                
            };
            xmpp.Open();
            Console.WriteLine(xmpp.ClientVersion);
            Console.WriteLine(xmpp.StreamVersion);
            Console.WriteLine(xmpp.MyJID);
            xmpp.OnLogin += new ObjectHandler(OnLoginEvent);
            xmpp.OnMessage += new MessageHandler(xmpp_OnMessage);
            xmpp.OnError += new ErrorHandler(xmpp_OnError);

            string ololo = Console.ReadLine();
            while (true)
            {
                ololo = Console.ReadLine();
                int delay = 1000;
                switch (ololo)
                {
                    case "exit": return;
                    case "reconnect": xmpp.Close(); System.Threading.Thread.Sleep(delay); xmpp.Open(); continue;
                }

                xmpp.Send(new Message(Room, MessageType.GroupChat, ololo));
            }


        }

        static void xmpp_OnError(object sender, Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        static void xmpp_OnMessage(object sender, Message msg)
        {
            Console.WriteLine(msg.From + ": " + msg.Body);
            if (msg.XDelay != null) return;
            switch (msg.Body)
            {
                case "!test": xmpp.Send(new Message(Room, MessageType.GroupChat, "*\nI'm here!")); break;
                case "няшка": xmpp.Send(new Message(Room, MessageType.GroupChat, "*\nНяшка - мой_ник")); break;
                case "ololo": xmpp.Send(new Message(Room, MessageType.GroupChat, "*\nqwerty")); break;
            }
        }

        static void OnLoginEvent(object sender)
        {
            Console.WriteLine("Присоединён.");
            xmpp.SendMyPresence();

            MucManager mucManager = new MucManager(xmpp);
            Room = new Jid("shnyakin@bvkgroup.ru");

            mucManager.AcceptDefaultConfiguration(Room);
            mucManager.JoinRoom(Room, "shnyakin");
        }
    }
}