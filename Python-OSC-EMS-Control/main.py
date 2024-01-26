from pythonosc.dispatcher import Dispatcher
from pythonosc import udp_client
from pythonosc import osc_server
import ast
import serial

IP = "127.0.0.1"
PORT = 2691

freq = 20
amplitude = 1
time = 1
calib_step = 0

def print_handler(address, *args):
    print(f"{address}: {args}")

def default_handler(address, *args):
    print(f"DEFAULT {address}: {args}")

def calib(address, *args):
    try:
        input_tuple = ast.literal_eval(str(args))
        # Additional processing to handle different types
        processed_values = []
        for value in input_tuple:
            if isinstance(value, (int, float, str)):
                processed_values.append(value)
            else:
                # Handle other types as needed
                processed_values.append(str(value))

        # Unpacking the processed values
        calib_step = processed_values
        if calib_step != '0':
            amplitude = int(calib_step) * 2.5
        else:
            amplitude = 1

        # Printing the values of the variables (DEBUG)
        print("Calib_Step: ", calib_step)

        amplitude_command = f"AMPL 1 {amplitude}\r\n".encode()
        time_command = f'STIM 1 {time} 1\r\n'.encode()
        com.write(amplitude_command)
    except:
        print('[ERROR]')


def surge(address, *args):
    try:
        input_tuple = ast.literal_eval(str(args))
        # Additional processing to handle different types
        processed_values = []
        for value in input_tuple:
            if isinstance(value, (int, float, str)):
                processed_values.append(value)
            else:
                # Handle other types as needed
                processed_values.append(str(value))

        # Unpacking the processed values
        amplitude, freq, time = processed_values

        # Printing the values of the variables (DEBUG)
        print("var1:", amplitude)
        print("var2:", freq)
        print("var3:", time)
        amplitude_command = f"AMPL 1 {amplitude}\r\n".encode()
        freq_command = f"FREQ 1 {freq}\r\n".encode()
        time_command = f'STIM 1 {time} 1\r\n'.encode()
        com.write(amplitude_command)
        com.write(freq_command)
        com.write(time_command)

    except (SyntaxError, ValueError):
        print('[ERROR] BAD DATA OR COMMUNICATION FAIL.')


dispatcher = Dispatcher()
dispatcher.map("/EMS_TRIGGER/*", surge)
dispatcher.set_default_handler(surge)
dispatcher.map("/EMS_CALIB/*", calib)
dispatcher.set_default_handler(calib)

com = serial.Serial(port='/dev/cu.SLAB_USBtoUART', baudrate=115200)


if __name__ == '__main__':
    server = osc_server.OSCUDPServer((IP, PORT), dispatcher)
    server.serve_forever()