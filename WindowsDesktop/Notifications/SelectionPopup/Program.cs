using Laggson.Common.Notifications;
using uPLibrary.Networking.M2Mqtt;
using System;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Windows.Forms;
using System.IO;

namespace SelectionPopup
{
    class Program
    {
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
            Application.Run(new Form1("myimage.jpg"));
            
        }
        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var MessageContent = System.Text.Encoding.Default.GetString(e.Message);

            string PicPfad = @"myimage.jpg";

            if(e.Topic.Equals(" / theringbot/ring"))
                NotifyMeSempai("Benachrichtigung:", TranslateContent(MessageContent));
            if(e.Topic.Equals("/theringbot/pic"))
            {
                var bytes = Convert.FromBase64String(MessageContent);

                File.WriteAllBytes(PicPfad, bytes);
                
                NotifyMeSempai("Benachrichtigung:", "Es klingelt!", Path.GetFullPath(PicPfad));

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
            {
                return "Es klingelt";
            }
            else if (eingabe.Equals("o") || eingabe.Equals("O"))
            {
                return "Jemand ist Unterwegs";
            }
            else if (eingabe.Equals("n") || eingabe.Equals("N"))
            {
                return "Jemand hat Abgelehnt";
            }
            else
            {
                return "ERROR";
            }
        }
    }
}
