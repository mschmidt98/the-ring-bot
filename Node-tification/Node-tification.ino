#include <ESP8266WiFi.h>
#include <PubSubClient.h>

const int led_R = 16; //D0
const int led_G = 5; //D1
const int led_B = 4; //D2
const int buzzer1 = 13; //D7
const int buzzer2 = 15; //D8
WiFiClient espClient;
PubSubClient client(espClient);

void setup() {
  delay(1000);
  Serial.begin(115200);
  WiFi.begin("PIT-Hackathon", "Hackathon");

  //LED setup
  pinMode(led_R, OUTPUT);
  pinMode(led_G, OUTPUT);
  pinMode(led_B, OUTPUT);
  pinMode(buzzer1, OUTPUT);
  pinMode(buzzer2, OUTPUT);
  analogWrite(led_R, 0);
  analogWrite(led_G, 0);
  analogWrite(led_B, 0);

  //verbinden mit Access Point
  Serial.println();
  Serial.print("\nVerbinden");
  while(WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("verbunden!");
  Serial.print("IP-Adresse: ");
  Serial.println(WiFi.localIP());

  //Verbindung zum MQTT-Server herstellen
  client.setServer("172.16.0.1", 1883);
  if(client.connect("Marius_NodeMCU")) {
    Serial.println("Verbunden mit dem MQTT Server!");
    led_flash(255, 0, 0, 250);
    led_flash(0, 255, 0, 250);
    led_flash(0, 0, 255, 250);
    led_flash(255, 255, 255, 250);
    client.subscribe("/theringbot/ring");
    client.setCallback(callback);
  } else {
    Serial.println(client.state());
  }
}

void loop() {
  client.loop();
}

//Nachricht auslesen & darauf reagieren
void callback(char* topic, byte* payload, unsigned int length) {
  Serial.print("Neue Nachricht [");
  Serial.print(topic);
  Serial.print("] ");
  for (int i=0;i<length;i++) {
    Serial.print((char)payload[i]);
  }
  Serial.println();
  if((char)payload[0] == 'k') {
    led_flash(0, 0, 255, 1000);
    buzz(2, 2, 512);
    Serial.println("Es hat an der Tür geklingelt!");
  } else if((char)payload[0] == 'o') {
    led_flash(0, 255, 0, 1000);
    buzz(1, 1, 1024);
    Serial.println("Zugelassen!");
  } else if((char)payload[0] == 'n') {
    led_flash(255, 0, 0, 1000);
    buzz(6, 6, 175);
    Serial.println("Abgelehnt!");
  } else {
    led_flash(255, 255, 255, 1000);
  }
  payload = 0;
}

//LED mit RGB-Wert blinken lassen
void led_flash(byte red, byte green, byte blue, int ms) {
  analogWrite(led_R, red);
  analogWrite(led_G, green);
  analogWrite(led_B, blue);
  delay(ms);
  analogWrite(led_R, 0);
  analogWrite(led_G, 0);
  analogWrite(led_B, 0);
  delay(ms/4);
}

//beide Summer ansteuern
void buzz(byte delay1, byte delay2, int count) {
  for (int i = 0; i < count; i++) 
  {
    analogWrite(buzzer1, 1023); 
    analogWrite(buzzer2, 1023);
    delay(delay1);
    analogWrite(buzzer1, 0);
    analogWrite(buzzer2, 0);
    delay(delay2);
  }
}

