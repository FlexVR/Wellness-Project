from pythonosc.dispatcher import Dispatcher
from pythonosc import udp_client
from pythonosc import osc_server
import ast
import serial

IP = "127.0.0.1" #OSC IP of application
PORT = 2691 #OSC Port
COM_PORT = "/dev/cu.SLAB_USBtoUART" #COM port of EMS ESP32

freq = 90 #STIM Frequency (in Hz)
amplitude = 1 #STIM amplitude in mA (1-25mA)
time = 1 #STIM on time in sec
calib_step = 0 #Calibration step for personal tollerance calibration

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
        calib_step = int(processed_values)
        
        if calib_step != 0:
            amplitude = int(calib_step) * 2.5
        else:
            amplitude = 1
        calib_step += 1

        if calib_step == 10:
            calib_step = 0

        amplitude = float(amplitude)
        if amplitude > 25 or amplitude < 1:
            amplitude = 1
            print('[WARN] AMPL VALUE OUT OF BOUNDS.')


        print("Calib_Step: ", calib_step)

        amplitude_command = f"AMPL 1 {amplitude}\r\n".encode()
        freq_command = f"FREQ 1 {freq}\r\n".encode()
        time_command = f'STIM 1 {time} 1\r\n'.encode()
        com.write(amplitude_command)
        com.write(freq_command)
        com.write(time_command)

    except:
        print('[ERROR] BAD DATA OR COMMUNICATION FAIL.')


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
        amplitude = float(amplitude)
        freq = float(freq)
        time = float(time)

        if amplitude > 25 or amplitude < 1:
            amplitude = 1
            print('[WARN] AMPL VALUE OUT OF BOUNDS.')

        if freq > 100 or freq < 1:
            freq = 100
            print('[WARN] FREQ OUT OF BOUNDS.')

        if time > 7 or time < 0.25:
            time = 1
            print('[WARN] TIME OUT OF BOUNDS.')

        # Printing the values of the variables (DEBUG)
        print("AMPL:", amplitude)
        print("FREQ:", freq)
        print("TIME:", time)
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

com = serial.Serial(port=COM_PORT, baudrate=115200) #init communication with EMS

if __name__ == '__main__':
    server = osc_server.OSCUDPServer((IP, PORT), dispatcher)
    server.serve_forever()