#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <DHT.h>

IPAddress ip(192, 168, 1, 64);
IPAddress gateway(192, 168, 1, 1);
IPAddress subnet(255, 255, 0, 0);

#define WIFI_SSID "SSID"
#define WIFI_PASSWORD "PASSWORD"

ESP8266WebServer server(80);

#define DHT_TYPE DHT11
#define DHT_PIN 2

DHT dht(DHT_PIN, DHT_TYPE);

const float correctTemperature = 2.3f;

void handleRoot() {
  float temperature = dht.readTemperature(), humidity = dht.readHumidity();

  if (isnan(temperature) || isnan(humidity)) {
    Serial.println("Failed to read from sensor!");
    temperature = correctTemperature, humidity = 0;
  }
  
  String response = "{\"temperature\":";
  response += temperature - correctTemperature;
  response += ",\"humidity\":";
  response += humidity;
  response += "}";

  server.send(200, "application/json", response);
}

void setup() {
  WiFi.config(ip, gateway, subnet);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);

  Serial.begin(9600);

  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(500);
  }
  Serial.println("Connected!");

  Serial.println("IP: " + WiFi.localIP().toString());

  server.on("/", handleRoot);
  server.begin();

  Serial.println("HTTP server started!");

  dht.begin();

  Serial.println("Sensor enabled!");
}

void loop() {
  server.handleClient();
}