# This is a sample Python script.
from pythonosc.dispatcher import Dispatcher
from pythonosc import udp_client
from pythonosc import osc_server
from pythonosc import dispatcher
import ast

def print_handler(address, *args):
    print(f"{address}: {args}")


def default_handler(address, *args):
    print(f"DEFAULT {address}: {args}")


def surge(address, *args):
    print(args)
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

        # Printing the values of the variables
        print("var1:", amplitude)
        print("var2:", freq)
        print("var3:", time)

    except (SyntaxError, ValueError):
        pass
    amplitude_command = f"AMPL 1 {amplitude}\r\n".encode()
    freq_command = f"FREQ 1 {freq}\r\n".encode()
    time_command = f'STIM 1 {time} 1\r\n'.encode()
    pump.write(amplitude_command)
    pump.write(freq_command)
    pump.write(time_command)


dispatcher = Dispatcher()
dispatcher.map("/EMS_TRIGGER/*", surge)
dispatcher.set_default_handler(surge)

ip = "127.0.0.1"
port = 2691


# Press ⌃R to execute it or replace it with your code.
# Press Double ⇧ to search everywhere for classes, files, tool windows, actions, and settings.

import serial
pump = serial.Serial(port='/dev/cu.SLAB_USBtoUART', baudrate=115200)


if __name__ == '__main__':
    server = osc_server.OSCUDPServer((ip, port), dispatcher)
    server.serve_forever()  # Blocks forever
    print('eee')


# See PyCharm help at https://www.jetbrains.com/help/pycharm/
