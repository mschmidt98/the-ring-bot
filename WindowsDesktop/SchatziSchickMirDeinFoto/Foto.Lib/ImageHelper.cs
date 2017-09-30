using Laggson.Common.Notifications;
using System;
using System.IO;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Foto.Lib
{
    public class ImageHelper
    {
        private MqttClient Client { get; set; }

        public ImageHelper()
        {
            string serverIp = "172.16.0.1";
            Client = new MqttClient(serverIp);

            // register to message received 
            Client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Client.Connect("SchatziSchickMirEinFoto");

            if (!Client.IsConnected)
                NotifyMeSempai("Fehler", "Verbindung fehlgeschlagen.");

            Client.Subscribe(new string[] { "/theringbot/ring" }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        /// <summary>
        /// Aboniert den MQTT-Service, sodass <see cref="Client_MqttMsgPublishReceived(object, MqttMsgPublishEventArgs)"/>
        /// aufgerufen wird, wenn eine Nachricht eintrifft.
        /// </summary>
        private void Subscribe()
        {
        }

        /// <summary>
        /// Zeigt die angegebene Nachricht als Windows-Toast an.
        /// </summary>
        /// <param name="ueberschrift">Die Überschrift der Meldung.</param>
        /// <param name="inhalt">Der Inhalt der Meldung.</param>
        private void NotifyMeSempai(string ueberschrift, string inhalt)
        {
            var message = new MessageItem(ueberschrift, inhalt);
            ToastNotifier.Show(message);
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = System.Text.Encoding.UTF8.GetString(e.Message);
            if (message != "k")
                return;

            // TODO: Do stuff
            var response = System.Text.Encoding.UTF8.GetBytes(GetLatestImageString());
            Client.Publish("/theringbot/pic", response);
        }

        private string GetLatestImageString()
        {
            string ftpAddress = "ftp://172.16.0.1/NX-4207_00626E623140/snap/";
            
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpAddress);
                request.Credentials = new NetworkCredential("user1", "123456");
                request.Method = WebRequestMethods.Ftp.ListDirectory;

                var response = (FtpWebResponse)request.GetResponse();
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);
                var names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                var nameArray = names.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                Console.WriteLine("Fehler");
            }

            return null;
        }
    }
}
