from pythonosc.dispatcher import Dispatcher
from pythonosc import udp_client
from pythonosc import osc_server
import ast
import serial
import time as ti

IP = "127.0.0.1" #OSC IP of application
PORT = 2691 #OSC PortF

f = open('stim_com.txt', 'r+')
com = str(f.read())
f.close()


COM_PORT = str('/dev/cu.SLAB_USBtoUART')#"/dev/cu.SLAB_USBtoUART" #COM port of EMS ESP32  /dev/cu.HEART-ESP32

freq = 75 #STIM Frequency (in Hz)
amplitude = 5 #STIM amplitude in mA (1-25mA)
timec = 1 #STIM on time in sec
calib_step = 0 #Calibration step for personal tollerance calibration

def print_handler(address, *args):
    print(f"{address}: {args}")

def default_handler(address, *args):
    print(f"DEFAULT {address}: {args}")

def stop_stim(address, *args):
    stop = f'EOFF\r\n'.encode()
    com.write(stop)

def end_calib(address, *args):
    global amplitude, freq, timec #i dont like global vars but hackathon lel
    amplitude = amplitude - (amplitude * 0.15)  # limit max to ensure comfort


def calib(address, *args):
    global amplitude, freq, timec, calib_step

   # try:
    input_tuple = ast.literal_eval(str(args))
    calib_step = input_tuple[0]
    # Unpacking the processed values


    if calib_step == 10:
        end_calib(amplitude, amplitude)
    else:
        amplitude = 1
    calib_step += 1

    amplitude = float(amplitude)

    if calib_step == 11:
        calib_step = 0
     #   amplitude = amplitude - (amplitude * 0.15)  # limit max to ensure comfort

    if amplitude > 25 or amplitude < 1:
        print(amplitude)
        amplitude = 2
        print('[WARN] AMPL VALUE OUT OF BOUNDS.')

    amplitude = calib_step * 2.5
    print("Calib_Step: ", calib_step, amplitude)

    enab_command = f'ENAB 2 1\r\n'.encode()
    amplitude_command = f"AMPL 2 {amplitude}\r\n".encode()
    freq_command = f"FREQ 2 {freq}\r\n".encode()
    time_command = f'STIM 2 {timec} 0\r\n'.encode()
    sym_command = f'SYMM 0\r\n'.encode()
    dur_command = f'DURN 400\r\n'.encode()
    denab_command = f'ENAB 2 0\r\n'.encode()

    com.write(enab_command)
    com.write(sym_command)
    com.write(dur_command)
    com.write(amplitude_command)
    com.write(freq_command)
    com.write(time_command)
    ti.sleep(timec)
    com.write(denab_command)


  #  except:
    #    print('[ERROR] BAD DATA OR COMMUNICATION FAIL.')


def surge(address, *args):
    global amplitude, freq, timec
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
        amplitudes, freq, timec = processed_values
        amplitude = float(amplitude)
        freq = float(freq)
        timec = float(timec)

        if amplitude > 25 or amplitude < 1:
            amplitude = 1
            print('[WARN] AMPL VALUE OUT OF BOUNDS.')

        if freq > 100 or freq < 1:
            freq = 100
            print('[WARN] FREQ OUT OF BOUNDS.')

        if timec > 7 or timec < 0.25:
            timec = 0.5
            print('[WARN] TIME OUT OF BOUNDS.')

        # Printing the values of the variables (DEBUG)
        print("AMPL:", amplitude)
        print("FREQ:", freq)
        print("TIME:", timec)

        enab_command = f'ENAB 2 1\r\n'.encode()
        amplitude_command = f"AMPL 2 {amplitude}\r\n".encode()
        freq_command = f"FREQ 2 {freq}\r\n".encode()
        time_command = f'STIM 2 {timec} 0\r\n'.encode()
        sym_command = f'SYMM 0\r\n'.encode()
        dur_command = f'DURN 500\r\n'.encode()
        denab_command = f'ENAB 2 0\r\n'.encode()

        com.write(enab_command)
        com.write(sym_command)
        com.write(dur_command)
        com.write(amplitude_command)
        com.write(freq_command)
        com.write(time_command)
        ti.sleep(timec)
        com.write(denab_command)


    except (SyntaxError, ValueError):
        print('[ERROR] BAD DATA OR COMMUNICATION FAIL.')


dispatcher = Dispatcher()
dispatcher.map("/EMS_TRIGGER/*", surge)
dispatcher.set_default_handler(surge)
dispatcher.map("/EMS_CALIB/*", calib)
dispatcher.set_default_handler(calib)
dispatcher.map("/END_CALIB/*", calib)
dispatcher.set_default_handler(end_calib)
dispatcher.map("/STOP/*", calib)
dispatcher.set_default_handler(stop_stim)

com = serial.Serial(port=COM_PORT, baudrate=115200) #init communication with EMS

if __name__ == '__main__':
    server = osc_server.OSCUDPServer((IP, PORT), dispatcher)
    server.serve_forever()