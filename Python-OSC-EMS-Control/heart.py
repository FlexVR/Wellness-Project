import serial
from pythonosc.udp_client import SimpleUDPClient

f = open('OSC_IP.txt', 'r+') #this is cursed but im lazy ;p
IP = str(f.read())
f.close()

#IP = "127.0.0.1"
PORT = 2691

client = SimpleUDPClient(IP, PORT)  # Create client

f = open('heart_com.txt', 'r+')
com = str(f.read())
f.close()
# /dev/cu.SLAB_USBtoUART
ser = serial.Serial()
ser.baudrate = 115200
ser.port = com
ser.open()

while True:
  try:
    data_raw = ser.readline()
    data_raw = data_raw.decode('utf-8').strip()

    # Extract integers from the string
    data = int(''.join(filter(str.isdigit, data_raw)))
    client.send_message("/HEART", data)  # Send message with int, float and string
    print("Heart Rate AVG: ", data_raw)
  except:
    print('[ERROR] Hardware loss, retrying...')
