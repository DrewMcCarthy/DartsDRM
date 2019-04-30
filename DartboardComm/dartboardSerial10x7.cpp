/*
 The circuit:
 * RX is digital pin 10 (connect to TX of other device)
 * TX is digital pin 11 (connect to RX of other device)

 Note:
 Not all pins on the Mega and Mega 2560 support change interrupts,
 so only the following can be used for RX:
 10, 11, 12, 13, 50, 51, 52, 53, 62, 63, 64, 65, 66, 67, 68, 69

 Not all pins on the Leonardo and Micro support change interrupts,
 so only the following can be used for RX:
 8, 9, 10, 11, 14 (MISO), 15 (SCK), 16 (MOSI).

*/
#include <SoftwareSerial.h>
SoftwareSerial mySerial(10,11); // RX, TX

int outputs[] = {4, 5, 6, 14, 17, 22, 27};
const int outcount = 7;
int inputs[] = {12, 16, 18, 19, 20, 21, 23, 24, 25, 26};
const int incount = 10;

void setup() 
{
  Serial.begin(9600);
  // set the data rate for the SoftwareSerial port
  mySerial.begin(9600);
  
  for(int i = 0; i < outcount; ++i) 
  {
    pinMode(outputs[i], OUTPUT);
    digitalWrite(outputs[i], HIGH);
  }

  for(int i = 0; i < incount; ++i) 
  {
    pinMode(inputs[i], INPUT);
    digitalWrite(inputs[i], HIGH);
  }
}

void loop() 
{
  for(int out=0; out < outcount; ++out) 
  {
    String segments = "";
    digitalWrite(outputs[out], LOW);
    for(int in = 0; in < incount; ++in) 
    {
      int pinValue = digitalRead(inputs[in]);
      if (pinValue == LOW) 
      {
        segments += (String)inputs[in] + "-" + (String)outputs[out];
        Serial.println(segments);
        mySerial.write(segments.c_str());
        delay(200);
      }
    } 
    digitalWrite(outputs[out], HIGH);
  }
}