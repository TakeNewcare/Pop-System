#define trig 2
#define echo 3

byte senddata[2];
long duration;
float distance; 

bool a = true;
bool b = true;

void setup() {
  Serial.begin(9600);
  pinMode(trig, OUTPUT);
  pinMode(echo, INPUT);

}


void loop() {
  digitalWrite(trig, LOW); 
  delay(2);
  
  digitalWrite(trig, HIGH);  
  delay(10);  

  digitalWrite(trig, LOW); 
  
  duration = pulseIn(echo, HIGH);
  
  distance = ((float)(340 * duration)/ 10000) / 2;
  int distance2 = (int)(distance-0.5);

  
  if(distance2 < 8){

    senddata[0] = 5;
    senddata[1] = 1;
    Serial.write(senddata, 2);
    delay(2000);
   }else{
    
    delay(1000);
    }
   
    
   


//   if (distance >= 10) {
//    b = true; // 다시 전송 가능 상태로 설정
 // }


}
