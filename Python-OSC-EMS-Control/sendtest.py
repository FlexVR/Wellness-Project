from pythonosc.udp_client import SimpleUDPClient

ip = "127.0.0.1"
port = 2691

client = SimpleUDPClient(ip, port)  # Create client

client.send_message("/EMS_TRIGGER", [1, 100, 55])  # Send message with int, float and string