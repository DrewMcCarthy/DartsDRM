/*
This worked on 9/30/2020 to rename an HM-10 BLE module.
The module was connected to a Mega2560 board.
GND -> GND
VCC -> 5v
Rx -> Tx (pin 18)
Tx -> Rx (pin 19)
*/

void setup() {
  Serial.begin(9600);
  Serial1.begin(9600);
}

void loop() {
  if (Serial.available()) {      // If anything comes in Serial (USB),
    Serial1.write(Serial.read());   // read it and send it out Serial1 (pins 0 & 1)
  }

  if (Serial1.available()) {     // If anything comes in Serial1 (pins 0 & 1)
    Serial.write(Serial1.read());   // read it and send it out Serial (USB)
  }
}
