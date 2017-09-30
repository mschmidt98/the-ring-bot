using Laggson.Common.Notifications;
using uPLibrary.Networking.M2Mqtt;
using System;
using System.Net;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Notifications
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress IP = IPAddress.Parse("172.16.0.1");

            MqttClient Client = new MqttClient(IP);

            // register to message received 
            Client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            Client.Connect(Guid.NewGuid().ToString());

            if(Client.IsConnected)
                NotifyMeSempai("Verbindung", "erfolgreich");
            else
                NotifyMeSempai("Verbindung", "fehlgeschlagen");

            Client.Subscribe(new string[] { "/theringbot/ring" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        static public void NotifyMeSempai(string ueberschrift, string inhalt)
        {
            ToastNotifier.Init("Meine Anwendung");

            var message = new MessageItem(ueberschrift, inhalt);
            ToastNotifier.Show(message);
        }

        static void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var MessageContent = System.Text.Encoding.Default.GetString(e.Message);
            NotifyMeSempai("Klingelt!:", MessageContent);
        }
    }
}
