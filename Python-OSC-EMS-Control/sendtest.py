from pythonosc.udp_client import SimpleUDPClient

ip = "127.0.0.1"
port = 2691
calib = 0
client = SimpleUDPClient(ip, port)  # Create client

client.send_message("/EMS_TRIGGER/*", [2, 100, 0.8])

while True:  #debug code
    print(f"Step: {calib}")
    send = input('send next step?: ')
    client.send_message("/EMS_CALIB/*", [calib])  # Send message with int, float and string
    calib += 1
    if calib == 10:
        print('max')
        break