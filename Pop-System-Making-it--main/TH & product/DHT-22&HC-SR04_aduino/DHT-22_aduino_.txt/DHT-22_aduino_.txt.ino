#include <Wire.h> 
#include <LiquidCrystal_I2C.h>
LiquidCrystal_I2C lcd(0x27,16,2); 

#include "DHT.h"
#define DHTPIN 2
#define DHTTYPE DHT22
DHT dht(DHTPIN, DHTTYPE);


byte On[8] = { 
        B00000,
        B01110,
        B10001,
        B01110,
        B00100,
        B11111,
        B10000,
        B11111
};
byte Do[8] = {
        B00000,
        B11111,
        B10000,
        B10000,
        B11111,
        B00000,
        B00100,
        B11111
};
byte s[8] = {
        B00100,
        B01010,
        B10001,
        B00000,
        B11111,
        B00000,
        B10101,
        B11111
};

void setup()
{
  Serial.begin(9600);
  lcd.init();
  lcd.backlight();

  lcd.createChar(0,On);  
  lcd.createChar(1,Do);  
  lcd.createChar(2,s);


  dht.begin();
}


byte senddata[4];
byte recv[4];
int i;
float h;
float t;

void loop()
{
  h = dht.readHumidity();
  t = dht.readTemperature();

  if (isnan(h) || isnan(t)) {
    Serial.println(F("온습도 체크 실패"));
  return;
  }
  else{
  
    lcd.clear();
    lcd.setCursor(0,0);
    
    lcd.write(0);
    lcd.write(1);
    lcd.print(" : ");
    lcd.print(t);
    lcd.print("'C");
    
   
    lcd.setCursor(0,1);
    lcd.write(2);
    lcd.write(1);
    lcd.print(" : ");
    lcd.print(h);
    lcd.print("%");

    i++;

    if(i == 6 or i == 0){
      senddata[0] = ((t*100)/100); 
      senddata[1] = ((int)(t*100)%100);
      senddata[2] = ((h*100)/100); 
      senddata[3] = ((int)(h*100)%100);
      Serial.write(senddata, 4);
      i=1;
      }
    }

      delay(1000);
    
}
