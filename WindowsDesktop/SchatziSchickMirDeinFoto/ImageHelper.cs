using Laggson.Common.Notifications;
using System;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.IO;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ImageDistributor
{
    public class ImageHelper
    {
        private MqttClient RingClient { get; }
        private MqttClient PicClient { get; }

        public ImageHelper()
        {
            string title = "Der Foto-Verschick-O-Mat";
            ToastNotifier.Init(title);
            Console.Title = title;

            RingClient = CreateClient("ring");
            PicClient = CreateClient("pic");

            Log("Service initialisiert.");
            Log("Zum Beenden Enter drücken...");
            Console.ReadLine();

            Log("Service wird beendet.");
            RingClient.Disconnect();
            PicClient.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transmit">Wenn false, wird für receive erstellt.</param>
        /// <returns></returns>
        private MqttClient CreateClient(string topic)
        {
            string serverIp = "172.16.0.1";
            var ringClient = new MqttClient(serverIp);

            if (topic == "pic")
            {
                ringClient.Connect("SchatziIchSchickDirMeinFoto");
            }
            else
            {
                ringClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                ringClient.Connect("SchatziSchickMirEinFoto");
                ringClient.Subscribe(new string[] { "/theringbot/" + topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }

            if (!ringClient.IsConnected)
                NotifyMeSempai("Fehler", "Verbindung fehlgeschlagen.");


            return ringClient;
        }

        /// <summary>
        /// Zeigt die angegebene Nachricht als Windows-Toast an.
        /// </summary>
        /// <param name="ueberschrift">Die Überschrift der Meldung.</param>
        /// <param name="inhalt">Der Inhalt der Meldung.</param>
        private void NotifyMeSempai(string ueberschrift, string inhalt, string imagePath = null)
        {
            var message = new MessageItem(ueberschrift, inhalt, "", imagePath);
            ToastNotifier.Show(message);
        }

        /// <summary>
        /// Gibt den Text mit der Zeit in der Konsole aus.
        /// </summary>
        /// <param name="message">Die anzuzeigende Nachricht.</param>
        private void Log(string message)
        {
            string date = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
            Console.WriteLine(date + ": " + message);
        }

        private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            var message = GetString(e.Message);
            if (message != "k")
                return;

            Log("Es klingelt.");
            var response = GetLatestImageBytes();

            if (response == null)
            {
                Log("Dummy-Bild wird gesendet.");
                var dummy = ImagePathToBase64("Fehlgeschlagen.png");
                response = GetBytes(dummy);
            }
            else
            {
                Log("Bild wird zurückgesendet.");
            }

            PicClient.Publish("/theringbot/pic", response);
        }

        /// <summary>
        /// Gibt den absoluten Dateinamen des neuesten Bilds auf dem FTP zurück.
        /// </summary>
        /// <returns></returns>
        private byte[] GetLatestImageBytes()
        {
            var path = GetPathOfLatestFile();

            if (string.IsNullOrEmpty(path))
                return null;

            try
            {
                WebClient cl = new WebClient
                {
                    BaseAddress = path,
                    Credentials = new NetworkCredential("user1", "123456")
                };

                var tempPath = Environment.GetEnvironmentVariable("tmp") + "\\LatestCamImg.jpg";
                cl.DownloadFile(path, tempPath);

                var image = ImagePathToBase64(tempPath);
                var bytes = GetBytes(image);

                return bytes;
            }
            catch
            {
                Log("Fehler beim Lesen des Bildes vom FTP-Server.");
                return null;
            }
        }

        /// <summary>
        /// Durchsucht den FTP und gibt die neueste Datei zurück.
        /// </summary>
        /// <returns></returns>
        private string GetPathOfLatestFile()
        {
            const string ftpAddress = "ftp://172.16.0.1/NX-4207_00626E623140/snap/";

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

                if (names.Length == 0)
                {
                    Log("Keine Dateien auf dem FTP-Server gefunden.");
                    return null;
                }

                Array.Sort(nameArray);

                return ftpAddress + nameArray[nameArray.Length - 1];
            }
            catch
            {
                Log("Fehler beim Lesen der Liste vom FTP.");
                return null;
            }
        }

        private byte[] GetBytes(string text)
        {
            return System.Text.Encoding.UTF8.GetBytes(text);
        }

        private string GetString(byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Lädt das Bild des angegebenen Pfads und wandelt es in Base64 um.
        /// </summary>
        /// <param name="imagePath">Der absolute Speicherpfad des Bildes.</param>
        /// <returns></returns>
        private string ImagePathToBase64(string imagePath)
        {
            string base64;

            using (Bitmap bm = new Bitmap(imagePath))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    bm.Save(ms, ImageFormat.Jpeg);
                    base64 = Convert.ToBase64String(ms.ToArray());
                }
            }

            return base64;
        }
    }
}
