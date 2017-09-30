using Laggson.Common.Notifications;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Notifications
{
    class Program
    {
        static void Main(string[] args)
        {
            ToastNotifier.Init("The RingBot");
            Subscribe();
        }

        /// <summary>
        /// Aboniert den MQTT-Service, sodass <see cref="Client_MqttMsgPublishReceived(object, MqttMsgPublishEventArgs)"/>
        /// aufgerufen wird, wenn eine Nachricht eintrifft.
        /// </summary>
        private static void Subscribe()
        {
            string serverIp = "172.16.0.1";
            MqttClient Client = new MqttClient(serverIp);

            // register to message received 
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.Connect("DesktopNotification");

            if (!Client.IsConnected)
                NotifyMeSempai("Verbindung", "fehlgeschlagen");

            Client.Subscribe(new string[] { "/theringbot/ring" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        /// <summary>
        /// Zeigt die angegebene Nachricht als Windows-Toast an.
        /// </summary>
        /// <param name="ueberschrift">Die Überschrift der Meldung.</param>
        /// <param name="inhalt">Der Inhalt der Meldung.</param>
        static public void NotifyMeSempai(string ueberschrift, string inhalt)
        {
            var message = new MessageItem(ueberschrift, inhalt);
            ToastNotifier.Show(message);
        }

        static void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var msg = System.Text.Encoding.UTF8.GetString(e.Message);
            NotifyMeSempai("Nachricht:", msg);
        }
    }
}
