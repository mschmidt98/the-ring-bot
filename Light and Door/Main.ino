#include <Adafruit_NeoPixel.h>
#include <ESP8266WiFi.h>
#include <PubSubClient.h>
#include <string.h>

#define PIN 5
#define NUM_LEDS 8
#define BRIGHTNESS 255

Adafruit_NeoPixel strip = Adafruit_NeoPixel(NUM_LEDS, PIN, NEO_GRBW + NEO_KHZ800);
WiFiClient espClient;
PubSubClient client(espClient);

void opening(char* topic, byte* payload, unsigned int length);
void ring(char* topic, byte* payload, unsigned int length);
void reset();

void setup() {
  strip.setBrightness(BRIGHTNESS);
  strip.begin();
  strip.show();

  WiFi.begin("PIT-Hackathon", "Hackathon");
  int t;
  while (WiFi.status() != WL_CONNECTED) {
    for (int i = 0; i < strip.numPixels(); i++) {
      strip.setPixelColor(i, strip.Color( 255, 0, 0));
      strip.show();
      t = i - 1;
      delay(50);
      reset();
      delay(50);
    }
  }
  for (int i = 0; i < strip.numPixels(); i++) {
    strip.setPixelColor(i, strip.Color( 0, 255, 0));
    strip.show();
  }
  delay(60);
  reset();
  client.setServer("172.16.0.1", 1883);
  if (client.connect("Kristian_NodeMCU")) {
    client.subscribe("/theringbot/ring");
    client.subscribe("/theringbot/opening");
    client.setCallback(callback);
    for (int i = 0; i < strip.numPixels(); i++) {
      strip.setPixelColor(i, strip.Color( 255, 0, 0));
      strip.show();
    }
    delay(60);
    reset();
    pinMode(D2, OUTPUT);
    pinMode(D3, OUTPUT);
    pinMode(D5, INPUT);
    pinMode(D6, INPUT);
    digitalWrite(D2, LOW);
    digitalWrite(D3, LOW);
  }
}

void loop() {
  client.loop();
}

void callback(char* topic, byte* payload, unsigned int length) {
  if (strcmp(topic, "/theringbot/ring") == 0) {
    ring(topic, payload, length);
  }
  if (strcmp(topic, "/theringbot/opening") == 0) {
    opening(topic, payload, length);
  }

}
void ring(char* topic, byte* payload, unsigned int length) {
  if ((char)payload[0] == 'k' || (char)payload[0] == 'K') {
    for (int i = 0; i < strip.numPixels(); i++) {
      strip.setPixelColor(i, 255, 255, 255);
      strip.show();
    }
  }
  if ((char)payload[0] == 'o' || (char)payload[0] == 'O') {
    for (int i = 1; i < strip.numPixels(); i = i + 3) {
      strip.setPixelColor(i, 0, 0, 255);
      strip.show();
    }
  }
  for (int t = 0; t < 10; t++) {
    if ((char)payload[0] == 'n' || (char)payload[0] == 'N') {
      for (int i = 1; i < strip.numPixels(); i = i + 3) {
        strip.setPixelColor(i, 0, 255, 0);
        strip.show();
        delay(10);
        reset();
      }
      delay(10);
    }
  }
  delay(600);
  reset();


}

void opening(char* topic, byte* payload, unsigned int length) {
  if ((char)payload[0] == 't' || (char)payload[0] == 'T') {
    digitalWrite(D3, LOW);
    while (digitalRead(D5) == HIGH) {
      for (int i = 1; i < strip.numPixels(); i = i + 3) {
        strip.setPixelColor(i, 0, 0, 255);
        strip.show();
        digitalWrite(D2, HIGH);
        delay(50);
        reset();
        digitalWrite(D2, LOW);
        delay(50);
      }
    }
    client.publish("/theringbot/opening", "o");
    delay(300);
    while (digitalRead(D6) == HIGH) {
      for (int i = 1; i < strip.numPixels(); i = i + 3) {
        strip.setPixelColor(i, 0, 255, 0);
        strip.show();
        digitalWrite(D3, HIGH);
        delay(70);
        reset();
        digitalWrite(D3, LOW);
        delay(70);
      }
    }
    client.publish("/theringbot/opening", "z");
  }
}

void reset() {
  for(int i = 0;  i < strip.numPixels(); i++ ){
  strip.setPixelColor(i, strip.Color( 0, 0, 0));
  strip.show();
  }
}

