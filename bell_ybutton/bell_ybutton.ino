#include <PubSubClient.h>

/*  
 *  ___   ___  ___  _   _  ___   ___   ____ ___  ____  
 * / _ \ /___)/ _ \| | | |/ _ \ / _ \ / ___) _ \|    \ 
 *| |_| |___ | |_| | |_| | |_| | |_| ( (__| |_| | | | |
 * \___/(___/ \___/ \__  |\___/ \___(_)____)___/|_|_|_|
 *                  (____/ 
 *  NodeMCU采集到的tilt传感器电压值发到MQTT Client
 * Tutorial URL 
 * CopyRight John Yu
 * 
 * Tutorial: http://kookye.com/2016/12/13/use-nodemcu-to-send-push-button-signal-to-remote-mqtt-client/
 * 
 *  weitergepfuscht M.Schmidt
 *
 */


 
#include <ESP8266WiFi.h>

int BUTTON_PIN = 5; 
const char* ssid = "Forum";
const char* password = "HACK2017";
const char* mqtt_server = "172.16.0.1";

WiFiClient espClient;
PubSubClient client(espClient);
long lastMsg = 0;
int lastStatus = 0;

void setup_wifi() {
   delay(100);
  // We start by connecting to a WiFi network
    Serial.print("Connecting to ");
    Serial.println(ssid);
    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) 
    {
      delay(500);
      Serial.print(".");
    }
  randomSeed(micros());
  Serial.println("");
  Serial.println("WiFi connected");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
}

void callback(char* topic, byte* payload, unsigned int length) 
{

  delay(200);
} //end callback

void reconnect() {
  // Loop until we're reconnected
  while (!client.connected()) 
  {
    Serial.print("Attempting MQTT connection...");
    // Create a random client ID
    String clientId = "ESP8266Client-";
    clientId += String(random(0xffff), HEX);
    // Attempt to connect
    //if you MQTT broker has clientID,username and password
    //please change following line to    if (client.connect(clientId,userName,passWord))
    if (client.connect(clientId.c_str()))
    {
      Serial.println("connected");
     //once connected to MQTT broker, subscribe command if any
     // client.subscribe("/theringbot/ring");
    } else {
      Serial.print("failed, rc=");
      Serial.print(client.state());
      Serial.println(" try again in 5 seconds");
      // Wait 5 seconds before retrying
      delay(5000);
    }
  }
} //end reconnect()

void setup() {
  
  Serial.begin(115200);
  setup_wifi();
  client.setServer(mqtt_server, 1883);
  client.setCallback(callback);
  pinMode(BUTTON_PIN,INPUT);
}

long lastActive = 0;

void loop() {
  if (!client.connected()) {
    reconnect();
  }
  client.loop();
  long now = millis();
  bool status;

  
  if (now - lastMsg > 50) {
     lastMsg = now;
     status=digitalRead(BUTTON_PIN);
     
     if(status==HIGH )
     {
          
          if(lastStatus==0 ){
            
            char message[2];
            message[1] = 0;
            
            if(now-lastActive > 10*1000 ){
              
              message[0] = 'k';
              Serial.println(message);
              
              //publish sensor data to MQTT broker
              client.publish("/theringbot/ring", message);
              lastActive = now;
            }
          
          }
          
          lastStatus = 1;
      }
     else
     {
          lastStatus  = 0;
     
     }
     
    }
     
}


