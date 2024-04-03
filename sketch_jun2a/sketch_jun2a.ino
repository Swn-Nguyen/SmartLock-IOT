#include <Wire.h>
#include <SPI.h>
#include <MFRC522.h>
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>
#include <Fonts/FreeSerif9pt7b.h>

#include <Keypad.h>
#include <string>
#include <EEPROM.h>

#include <NTPClient.h>
#include <WiFiUdp.h>

#include <Arduino.h>
#include <WiFi.h>
#include <WiFiClient.h>

#define SCREEN_WIDTH 128  // OLED display width, in pixels
#define SCREEN_HEIGHT 64  // OLED display height, in pixels
#define PASSWORD_LENGTH 5
#define btn 4
#define RST_PIN 17
#define SS_PIN 5
#define Lock_pin 16

#define buzzer_Pin 15

int oldGio = -1;
int oldPhut = -1;

const char *ssid = "Nguyen Son";
const char *pass = "215386497";
const char * udpAddress= "192.168.108.101";
const int  udpPort= 8080;

WiFiUDP udp;

unsigned long previousMillis = 0;
const unsigned long interval = 1000;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org");
int gio, phut, giay, thu, ngay, thang, nam;
String weekDays[7] = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
String months[13] = { "", "January", "Ferbruary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

int dem, addr;
int *ptrI;
bool openDoor= false;
MFRC522 mfrc522(SS_PIN, RST_PIN);

byte Master[4] = { 51, 254, 103, 189 };  // the xanh 0
byte emtyUID[4] = { 255, 255, 255, 255 };

String currentPassword = "";
String userInput, tempDisplay;
String senddingMessage = "";


char key;

Adafruit_SSD1306 display = Adafruit_SSD1306(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, -1);
const byte n_rows = 4;
const byte n_cols = 4;
char keys[n_rows][n_cols] = {
  { '1', '2', '3', 'A' },
  { '4', '5', '6', 'B' },
  { '7', '8', '9', 'C' },
  { '*', '0', '#', 'D' }
};

byte rowPins[n_rows] = {32, 33, 25, 26}; 
byte colPins[n_cols] = {27, 14, 12, 13};

Keypad keypad = Keypad(makeKeymap(keys), rowPins, colPins, n_rows, n_cols);

//ghi mật khẩu vào bộ nhớ EEPROM
void writePasswordToEEPROM(const char *currentPassword) {
  byte passwordArray[PASSWORD_LENGTH];
  // Chuyển đổi chuỗi ký tự thành mảng byte
  for (int i = 0; i < PASSWORD_LENGTH; i++) {
    passwordArray[i] = currentPassword[i];
    if (currentPassword[i] == '\0') break;  // Dừng khi gặp ký tự kết thúc chuỗi
  }
  // Lưu mảng byte vào EEPROM
  for (int i = 0; i < PASSWORD_LENGTH; i++) {
    EEPROM.write(i, passwordArray[i]);
  }
  EEPROM.commit();  // Cần thực hiện commit để lưu dữ liệu vào EEPROM
}

// đọc mật khẩu từ bộ nhớ EEPROM
void readPasswordFromEEPROM(char *currentPassword) {
  byte passwordArray[PASSWORD_LENGTH];

  // Đọc mảng byte từ EEPROM
  for (int i = 0; i < PASSWORD_LENGTH; i++) {
    passwordArray[i] = EEPROM.read(i);
  }

  // Chuyển đổi mảng byte thành chuỗi ký tự
  for (int i = 0; i < PASSWORD_LENGTH; i++) {
    currentPassword[i] = passwordArray[i];
    if (passwordArray[i] == 0) break;  // Dừng khi gặp ký tự kết thúc chuỗi
  }
}

void getThoigian() {
  thu = timeClient.getDay();
  String weekday = weekDays[thu];
  unsigned long epochTime = timeClient.getEpochTime();
  struct tm *ptm = gmtime((time_t *)&epochTime);
  ngay = ptm->tm_mday;
  thang = ptm->tm_mon + 1;  //1-->11;
  nam = ptm->tm_year + 1900;
  Serial.println();
  Serial.printf("%s Ngay %02d Thang %02d Nam %d", weekday, ngay, thang, nam);
}



void setup() {
  //setup RFID
  Serial.begin(9600);

  Serial.println("Connecting to " + String(ssid));
  WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.print("Connected to ");
  Serial.println(ssid);
  Serial.print("IP address: ");
  Serial.println(WiFi.localIP());
  //This initializes udp and transfer buffer
  udp.begin(udpPort);

  SPI.begin();
  mfrc522.PCD_Init();

  EEPROM.begin(PASSWORD_LENGTH + 4 * 5);

  pinMode(Lock_pin, OUTPUT);


  pinMode(buzzer_Pin, OUTPUT);
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {  // Address 0x3D for 128x64
    Serial.println(("SSD1306 allocation failed"));
    for (;;)
      ;
  }
  
  delay(2000);
  // display.setFont(&FreeSerifItalic9pt7b);
  display.clearDisplay();
  display.setTextSize(1, 2);
  display.setTextColor(WHITE);
  display.setCursor(20, 25);
  display.println("Wellcome home");
  display.display();
  delay(3000);
  display.clearDisplay(); display.display();

  timeClient.begin();
  timeClient.setTimeOffset(25200);  //GMT +7
  while (!timeClient.update()) {
    timeClient.forceUpdate();
  }
  getThoigian();
  
}

void DisplayResult() {
  display.setCursor(90, 25);
  // display.setTextSize(1);
  display.print(tempDisplay);
  display.display();
}

void changePassword() {
  int j = 0;
  bool ConditionMet = false;
  String userInput2 = "";
  while (!ConditionMet) {
    display.setTextSize(1, 2);
    display.setCursor(20, 0);
    display.printf("CHANGE PASSWORD");
    display.setCursor(0, 20);
    display.print("Enter old Password:");
    display.setCursor(0, 40);
    display.print(tempDisplay);
    display.display();
    key = keypad.waitForKey();
    if (key == '#') {
      display.clearDisplay();
      return;
    }
    if (key != 'A' && key != 'B' && key != 'C' && key != 'D' && key != '*' && key != '#') {
      userInput2 += key;
      tempDisplay += "*";
      digitalWrite(buzzer_Pin, HIGH);
      delay(50);
      digitalWrite(buzzer_Pin, LOW);
    }

    char currentPassword[PASSWORD_LENGTH];
    readPasswordFromEEPROM(currentPassword);

    int passwordLength = 0;
    while (currentPassword[passwordLength] != '\0') {
      passwordLength++;
    }

    if (userInput2.length() == passwordLength) {
      if (strcmp(userInput2.c_str(), currentPassword) == 0) {
        userInput2 = "";
        tempDisplay = "";
        display.clearDisplay();
        display.display();
        ConditionMet = true;
      } else {
        userInput2 = "";
        tempDisplay = "";
        display.clearDisplay();
        display.setCursor(20, 25);
        display.print("Wrong password");

        display.display();
        delay(1000);

        display.clearDisplay();
        display.display();
      }
    }
    display.display();  // Thêm dòng này để cập nhật hiển thị trên màn hình
  }
  ConditionMet = false;
  display.setCursor(20, 0);
  display.printf("CHANGE PASSWORD");
  display.setCursor(0, 20);
  display.print("Enter a new Password:");
  display.display();
  while (!ConditionMet) {
    key = keypad.waitForKey();
    if (key == '#') {
      display.clearDisplay();
      return;
    }
    if (key != 'A' && key != 'B' && key != 'C' && key != 'D' && key != '*' && key != '#' && userInput2.length() < 5) {
      userInput2 += key;
      display.setCursor(0, 40);
      display.print(userInput2);
      display.display();
      digitalWrite(buzzer_Pin, HIGH);
      delay(50);
      digitalWrite(buzzer_Pin, LOW);
    }
    currentPassword = userInput2;
    if (currentPassword.length() == 4) {
      display.clearDisplay();
      display.setCursor(0, 0);
      display.print("Do you want save password?\nPress 'C' to save\nPress 'D' to decline");
      display.display();
      if (key == 'C') {
        writePasswordToEEPROM(currentPassword.c_str());
        UDPDataPacket("Master/Change Password");
        display.clearDisplay();
        display.setCursor(0, 25);
        display.print("Change password");
        display.setCursor(0,38);
        display.print("success!");
        display.display();
        delay(2000);
        display.clearDisplay();
        display.display();
        ConditionMet = true;

      } else if (key == 'D') {
        userInput2 = "";
        display.clearDisplay();
        display.setCursor(30, 25);
        display.print("Back");
        display.display();
        delay(1000);
        display.clearDisplay();
        display.display();
        break;
      }
    }
    display.display();  // Thêm dòng này để cập nhật hiển thị trên màn hình
  }
  display.clearDisplay();
  return;
}

//tao mot the master, yeu cau check the master neu them the
//so sanh uid neu trung khop thi khong luu
//neu khong trung khop luu vao the 1, moi the su dung 4 byte.

bool IsUIDStored(byte UID[]) {
  dem = 0;
  byte storedUID[4];
  for (int i = 0; i < 5; i++) {
    EEPROM.get(5 + i * 4, storedUID);

    if (memcmp(UID, storedUID, 4) == 0) {
      addr = i;
      return true;
    } else {
      while (i == 4) {
        return false;
      }
    }
  }
}



void addRFID() {
  byte storedUID[4];
  byte UID[4];

  bool condition = false;
  while (!condition) {
    char key = keypad.getKey();
    if (key == '*') {
      display.clearDisplay();
      return;
    }
    display.setTextSize(1, 2);
    display.clearDisplay();
    display.setCursor(0, 25);
    display.print("Kiem tra the Master..");
    display.display();
    if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
      Serial.print("UID: ");
      for (byte i = 0; i < mfrc522.uid.size; i++) {
        Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
        UID[i] = mfrc522.uid.uidByte[i];
        Serial.print(UID[i]);
      }
      Serial.println("  ");

      mfrc522.PICC_HaltA();
      mfrc522.PCD_StopCrypto1();

      if (memcmp(UID, Master, 4) == 0) {
        condition = true;
      } else {

        tone(buzzer_Pin, 500);
        delay(200);
        noTone(buzzer_Pin);
        display.clearDisplay();
        display.setCursor(10, 30);
        display.print("Sai the Master");
        display.display();
        delay(1500);

        display.clearDisplay();
        display.display();
      }
    }
  }
  condition = false;

  while (!condition) {
    char key = keypad.getKey();
    if (key == '*') {
      display.clearDisplay();
      return;
    }
    display.clearDisplay();
    display.setCursor(0, 25);
    display.print("Read Card to save");
    display.display();
    if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
      Serial.print("UID: ");
      for (byte i = 0; i < mfrc522.uid.size; i++) {
        Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
        UID[i] = mfrc522.uid.uidByte[i];
        Serial.print(UID[i]);
      }
      Serial.println("  ");

      mfrc522.PICC_HaltA();
      mfrc522.PCD_StopCrypto1();
      if (IsUIDStored(UID)) {
        Serial.println("UID đã được lưu trong EEPROM");
        display.clearDisplay();
        display.setCursor(20, 25);
        display.print("UID da duoc luu");
        display.display();
        delay(2000);
        display.clearDisplay();
      } else {

        for (int i = 0; i < 2; i++) {
          digitalWrite(buzzer_Pin, HIGH);
          delay(100);
          digitalWrite(buzzer_Pin, LOW);
          delay(100);
        }
        Serial.println("UID chưa được lưu trong EEPROM");
        for (int i = 0; i < 5; i++) {
          EEPROM.get(5 + i * 4, storedUID);
          if (memcmp(emtyUID, storedUID, 4) == 0) {
            ptrI = &i;
            Serial.print(*ptrI);
            break;
          }
        }
        EEPROM.put(5 + 4 * (*ptrI), UID);
        EEPROM.commit();
        senddingMessage += String(UID[0]) + " " + String(UID[1]) + " " + String(UID[2]) + " " + String(UID[3]);
        senddingMessage += "/Add an UID";
        UDPDataPacket(senddingMessage.c_str());
        display.clearDisplay();
        display.setCursor(10, 25);
        display.print("Luu the thanh cong");
        display.display();
        delay(1000);
        display.clearDisplay();
        senddingMessage = "";
        condition = true;
      }
    }
  }
  return;
}

void removeRFID() {
  bool condition = false;
  byte UID[4];
  while (!condition) {
    char key = keypad.getKey();
    if (key == 'D') {
      display.clearDisplay();
      break;
    }
    display.setTextSize(1, 2);
    display.clearDisplay();
    display.setCursor(0, 25);
    display.print("Read Card for delete");
    display.display();



    if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
      Serial.print("UID: ");
      for (byte i = 0; i < mfrc522.uid.size; i++) {
        Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
        UID[i] = mfrc522.uid.uidByte[i];
        Serial.print(UID[i]);
      }
      Serial.println("  ");

      mfrc522.PICC_HaltA();
      mfrc522.PCD_StopCrypto1();

      if (IsUIDStored(UID)) {
        Serial.printf("Vị trí thẻ: %d", addr);
        Serial.println("");
        EEPROM.put(5 + 4 * addr, emtyUID);
        EEPROM.commit();
        
        senddingMessage += String(UID[0]) + " " + String(UID[1]) + " " + String(UID[2]) + " " + String(UID[3]);
        senddingMessage += "/Delete UID";
        UDPDataPacket(senddingMessage.c_str());
        for (int i = 0; i < 2; i++) {
          digitalWrite(buzzer_Pin, HIGH);
          delay(100);
          digitalWrite(buzzer_Pin, LOW);
          delay(100);
        }
        display.clearDisplay();
        display.setCursor(0, 25);
        display.print("Xoa the thanh cong");
        display.display();
        delay(1000);

        display.clearDisplay();
        senddingMessage = "";
        condition = true;
      } else {
        Serial.println("Card doesn't exist");

        tone(buzzer_Pin, 500);
        delay(200);
        noTone(buzzer_Pin);
        display.clearDisplay();
        display.setCursor(10, 30);
        display.print("Card doesn't exist");
        display.display();
        delay(1500);

        display.clearDisplay();
        display.display();
      }
    }
  }
  return;
}

void checkRFID() {
  bool condition = false;
  byte UID[4];
  while (!condition) {
    char key = keypad.getKey();
    if (key == 'A') {
      display.clearDisplay();
      break;
    }
    display.setTextSize(1, 2);
    display.clearDisplay();
    display.setCursor(20, 30);
    display.println("Read Card...");
    display.display();
    if (mfrc522.PICC_IsNewCardPresent() && mfrc522.PICC_ReadCardSerial()) {
      Serial.print("UID: ");
      for (byte i = 0; i < mfrc522.uid.size; i++) {
        Serial.print(mfrc522.uid.uidByte[i] < 0x10 ? " 0" : " ");
        UID[i] = mfrc522.uid.uidByte[i];
        Serial.print(UID[i]);
      }
      Serial.println("  ");

      mfrc522.PICC_HaltA();
      mfrc522.PCD_StopCrypto1();

      if (IsUIDStored(UID)) {
        condition = true;
        openDoor= true;
        senddingMessage += String(UID[0]) + " " + String(UID[1]) + " " + String(UID[2]) + " " + String(UID[3]);
        senddingMessage += "/Open by RFID";
        UDPDataPacket(senddingMessage.c_str());
        senddingMessage = "";

        
      } else {
        display.clearDisplay();
        display.setCursor(20, 30);
        display.print("Wrong card!!");

        tone(buzzer_Pin, 500);
        delay(200);
        noTone(buzzer_Pin);
        display.display();
        delay(1500);

        display.clearDisplay();
        display.display();
      }
    }
  }
  return;
}

 void UDPDataPacket(const char *sendData){
  uint8_t buffer[50];
  udp.beginPacket(udpAddress, udpPort);
  strcpy((char*)buffer, sendData);
  udp.write(buffer,strlen(sendData));
  udp.endPacket();
  memset(buffer, 0, sizeof(buffer));
}

void usingPassword() {

  bool condition = false;
  while (!condition) {
    display.setTextSize(1, 2);
    display.setCursor(0, 25);
    display.print("Enter Password: ");
    display.display();
    char key = keypad.waitForKey();

    if (key != 'A' && key != 'B' && key != 'C' && key != 'D' && key != '*' && key != '#') {
      userInput += key;
      tempDisplay += "*";
      digitalWrite(buzzer_Pin, HIGH);
      delay(50);
      digitalWrite(buzzer_Pin, LOW);
    }
    if (key == 'B') {
      display.clearDisplay();
      userInput = "";
      tempDisplay = "";
      break;
    }
    DisplayResult();
    char currentPassword[PASSWORD_LENGTH];
    readPasswordFromEEPROM(currentPassword);
    // Serial.print(currentPassword);
    int passwordLength = 0;
    while (currentPassword[passwordLength] != '\0') {
      passwordLength++;
    }

    if (userInput.length() == passwordLength) {
      if (strcmp(userInput.c_str(), currentPassword) == 0) {
        
        userInput = "";
        tempDisplay = "";
        openDoor= true;
        UDPDataPacket("Master/Open by password");
        condition = true;
      } else {
        userInput = "";
        tempDisplay = "";

        tone(buzzer_Pin, 500);
        delay(200);
        noTone(buzzer_Pin);
        display.clearDisplay();
        display.setCursor(20, 25);
        display.print("ACCESS DENIED");
        display.display();
        delay(1500);

        display.clearDisplay();
        display.display();
      }
    }
  }
  return;
}

void loop() {
  if(openDoor){
    display.setTextSize(1, 2);
    digitalWrite(Lock_pin, HIGH);
    for (int i = 0; i < 2; i++) {
      digitalWrite(buzzer_Pin, HIGH);
      delay(100);
      digitalWrite(buzzer_Pin, LOW);
      delay(100);
    }
    display.clearDisplay();
    display.setCursor(20, 25);
    display.print("ACCESS ALLOWED!");
    display.display();
    delay(3000);
    digitalWrite(Lock_pin, LOW);
    display.clearDisplay();
    display.display();
    openDoor= false;
  }
  gio = timeClient.getHours();
  phut = timeClient.getMinutes();
  giay = timeClient.getSeconds();

  if (gio == 23 && phut == 59 && giay == 59) {
    ESP.restart();
  }
  if (gio != oldGio || phut != oldPhut) {
    display.clearDisplay();
    oldGio = gio;
    oldPhut = phut;
  }

  display.setTextSize(1, 2);
  display.setCursor(0, 0);
  display.printf("%s, %s %02d", weekDays[thu], months[thang], ngay);
  display.setCursor(50, 20);
  display.printf("%d", nam);
  display.setTextSize(2);
  display.setCursor(35, 40);
  display.printf("%02d:%02d", gio, phut);
  display.display();
  char userPW = keypad.getKey();
  if (userPW != NO_KEY) {
    // Serial.print(userPW);
    if (userPW != 'A' && userPW != 'B' && userPW != 'C' && userPW != 'D' && userPW != '*' && userPW != '#') {
      display.clearDisplay();
      usingPassword();
    }
    if (userPW == 'A') {
    display.clearDisplay();
    checkRFID();
    }
    if (userPW == 'C'){
      UDPDataPacket("Face");
    }
    if (userPW == 'D') {
      display.clearDisplay();
      removeRFID();
    }

    if (userPW == '*') {
      display.clearDisplay();
      addRFID();
    }

    if (userPW == '#') {
      display.clearDisplay();
      changePassword();
    }
  }
  udp.parsePacket();
  uint8_t ReceiveMessage[50];
  if(udp.available()>0){
    udp.read(ReceiveMessage, sizeof(ReceiveMessage));
    Serial.println((char *)ReceiveMessage);
    if(strcmp((char *)ReceiveMessage, "Open door")== 0){
      openDoor= true;
    }
  }
  
}
