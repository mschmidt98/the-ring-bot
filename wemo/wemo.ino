#include <PubSubClient.h>
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <WiFiUdp.h>
#include <functional>
#include "switch.h"
#include "UpnpBroadcastResponder.h"
#include "CallbackFunction.h"

// prototypes
boolean connectWifi();

//on/off callbacks
void eingangstuerON();
void eingangstuerOff();
void gehelosOn(); 
void gehelosOff();
void ablehnenOn(); 
void ablehnenOff();

// Change this before you flash
const char* ssid = "PIT-Hackathon";
const char* password = "Hackathon";

boolean wifiConnected = false;

UpnpBroadcastResponder upnpBroadcastResponder;

Switch *eingangstuer = NULL;
Switch *gehelos = NULL;
Switch *ablehnen = NULL;

WiFiClient espClient;
PubSubClient client(espClient);
void setup()
{
  Serial.begin(9600);

  // Initialise wifi connection
  wifiConnected = connectWifi();

  if (wifiConnected) {
    upnpBroadcastResponder.beginUdpMulticast();

    // Define your switches here. Max 14
    // Format: Alexa invocation name, local port no, on callback, off callback
    eingangstuer = new Switch("eingangstuer", 80, eingangstuerON, eingangstuerOff);
    gehelos = new Switch("oeffnen", 81, gehelosOn, gehelosOff);
    ablehnen = new Switch("test", 82, ablehnenOn, ablehnenOff);

    Serial.println("Adding switches upnp broadcast responder");
    upnpBroadcastResponder.addDevice(*eingangstuer);
    upnpBroadcastResponder.addDevice(*gehelos);
    upnpBroadcastResponder.addDevice(*ablehnen);
  }
}

void loop()
{
  if (wifiConnected) {
    upnpBroadcastResponder.serverLoop();

    gehelos->serverLoop();
    eingangstuer->serverLoop();
    ablehnen->serverLoop();
  }
}

void callback(char* topic, byte* payload, unsigned int length) {}

void eingangstuerON() {
  Serial.print("Switch 1 turn on ...");


  client.setServer("172.16.0.1", 1883);
  if (client.connect("Alexa_NodeMCU")) {
    client.publish("/theringbot/ring", "k");
  }
}

void eingangstuerOff() {
  Serial.print("Switch 1 turn off ...");
}

void gehelosOn() {
  Serial.print("Switch 2 turn on ...");

   client.setServer("172.16.0.1", 1883);
  if (client.connect("Alexa_NodeMCU")) {
    client.publish("/theringbot/opening", "t");
  }
}

void gehelosOff() {
  Serial.print("Switch 2 turn off ...");
}

void ablehnenOn() {
  Serial.print("Switch 2 turn on ...");

   client.setServer("172.16.0.1", 1883);
  if (client.connect("Alexa_NodeMCU")) {
    client.publish("/theringbot/ring", "n");
  }
}

void ablehnenOff() {
  Serial.print("Switch 2 turn off ...");
}


// connect to wifi – returns true if successful or false if not
boolean connectWifi() {
  boolean state = true;
  int i = 0;

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  Serial.println("");
  Serial.println("Connecting to WiFi");

  // Wait for connection
  Serial.print("Connecting ...");
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
    if (i > 10) {
      state = false;
      break;
    }
    i++;
  }

  if (state) {
    Serial.println("");
    Serial.print("Connected to ");
    Serial.println(ssid);
    Serial.print("IP address: ");
    Serial.println(WiFi.localIP());
  }
  else {
    Serial.println("");
    Serial.println("Connection failed.");
  }

  return state;
}
