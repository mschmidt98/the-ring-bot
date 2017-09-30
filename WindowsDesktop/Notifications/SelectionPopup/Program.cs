﻿using Laggson.Common.Notifications;
using uPLibrary.Networking.M2Mqtt;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;
using System.IO;

namespace SelectionPopup
{
    class Program
    {
        static bool IsAntwortMoeglich = false;
        static string AktBild = "";

        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ToastNotifier.Init("Tür-Cam");
            ToastNotifier.ToastClicked += ToastNotifier_ToastClicked;

            Subscribe();
        }

        static public void Subscribe()
        {
            MqttClient Client = new MqttClient("172.16.0.1");

            // register to message received 
            Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            Client.Connect("WinNotification");

            if (!Client.IsConnected)
                NotifyMeSempai("Fehler!:", "Verbindung fehlgeschlagen");

            Client.Subscribe(new string[] { "/theringbot/ring", "/theringbot/pic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        private static void ToastNotifier_ToastClicked(object sender, EventArgs e)
        {
            if (!IsAntwortMoeglich)
                return;

            Application.Run(new Form1(AktBild));
        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            AktBild = "";
            var MessageContent = System.Text.Encoding.Default.GetString(e.Message);

            AktBild = $@"myimage_{DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss")}.jpg";

            if (e.Topic.Equals("/theringbot/ring"))
            {
                IsAntwortMoeglich = false;
                NotifyMeSempai("Benachrichtigung:", TranslateContent(MessageContent));
            }
            if(e.Topic.Equals("/theringbot/pic"))
            {
                IsAntwortMoeglich = true;

                var bytes = Convert.FromBase64String(MessageContent);
                File.WriteAllBytes(AktBild, bytes);
                
                NotifyMeSempai("Benachrichtigung:", "Es klingelt!", Path.GetFullPath(AktBild));
            }
        }

        static public void NotifyMeSempai(string ueberschrift, string inhalt)
        {
            var message = new MessageItem(ueberschrift, inhalt);
            ToastNotifier.Show(message);
        }

        static public void NotifyMeSempai(string ueberschrift, string inhalt, string imgPfad)
        {
            var message = new MessageItem(ueberschrift, inhalt,"",imgPfad);
            ToastNotifier.Show(message);
        }
        static public string TranslateContent(string eingabe)
        {
            if (eingabe.Equals("k") || eingabe.Equals("K"))
                return "Es klingelt";
            else if (eingabe.Equals("o") || eingabe.Equals("O"))
                return "Jemand ist Unterwegs";
            else if (eingabe.Equals("n") || eingabe.Equals("N"))
                return "Jemand hat Abgelehnt";
            else
                return "ERROR";
        }

        public static void SendToMQTT(string antwort)
        {

            MqttClient Client = new MqttClient("172.16.0.1");

            // register to message received 
            Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            Client.Connect("WinNotification");

            var response = System.Text.Encoding.UTF8.GetBytes(antwort);
            Client.Publish("/theringbot/ring", response);

            Client.Subscribe(new string[] { "/theringbot/ring", "/theringbot/pic" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }
    }
}
