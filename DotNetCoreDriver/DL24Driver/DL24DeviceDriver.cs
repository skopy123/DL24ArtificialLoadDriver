using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace DL24Driver
{
	public class DL24DeviceDriver
	{
		public event Dl24DataReceivedEventHandler DataRecieved;

		public delegate void Dl24DataReceivedEventHandler(object sender, CurrentValues values);

		protected SerialPort serialPort;

		public CurrentValues Values { get; protected set; }

		public DL24DeviceDriver() {
			Values = new CurrentValues();
			serialPort = new SerialPort();
		}

		public void Open(string portName)
		{
			serialPort = new SerialPort(portName, 9600);
            serialPort.DataReceived += Sp_DataReceived;
			serialPort.Open();
		}

		public void Close() {
			serialPort.Close();
			serialPort.DataReceived -= Sp_DataReceived;
		}

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			Thread.Sleep(150);
			byte[] rxBuffer = new byte[checked(serialPort.BytesToRead - 1 + 1)];
			serialPort.Read(rxBuffer, 0, serialPort.BytesToRead);
			if (rxBuffer.Length > 3)
			{
				if (rxBuffer[3] == 2)
				{
					DecodeRecievedMessage(rxBuffer);
					DataRecieved?.Invoke(this, Values);
				}
				else
				{
					Console.WriteLine("unsupported device type or invalid message format");
				}
			}
			serialPort.DiscardInBuffer();
			serialPort.DiscardOutBuffer();
		}

		private void DecodeRecievedMessage(byte[] buffer)
		{
			Values.Voltage = (double)(unchecked((int)buffer[4]) * 256 * 256 + unchecked((int)buffer[5]) * 256 + unchecked((int)buffer[6])) / 10.0; //V
			Values.Current = (double)(unchecked((int)buffer[7]) * 256 * 256 + unchecked((int)buffer[8]) * 256 + unchecked((int)buffer[9])) / 1000.0; //A
			Values.Power = Values.Voltage * Values.Current;
			Values.ElectricityKwh = (double)(unchecked((int)buffer[13]) * 256 * 256 * 256) / 100.0 + (double)(unchecked((int)buffer[14]) * 256 * 256) / 100.0 + (double)(unchecked((int)buffer[15]) * 256) / 100.0 + (double)unchecked((int)buffer[16]) / 100.0; //Kwh
			Values.InternalTemperature = unchecked((int)buffer[24]) * 256 + unchecked((int)buffer[25]); //℃
			Values.BatteryCapacity = (double)(unchecked((int)buffer[10]) * 256 * 256 + unchecked((int)buffer[11]) * 256 + unchecked((int)buffer[12])) / 100.0; //Ah
		}

		public void SendCommand(byte command)
		{
			if (!serialPort.IsOpen)
			{
				throw new Exception("Serial port is not open");
			}
			byte[] txBuffer = new byte[10];
			txBuffer[0] = byte.MaxValue;
			txBuffer[1] = 85;
			txBuffer[2] = 17;

				txBuffer[3] = (byte)2; //DeviceType;
				txBuffer[4] = command;
				txBuffer[5] = 0;
				txBuffer[6] = 0;
				txBuffer[7] = 0;
				txBuffer[8] = 0;
			checked
			{
				txBuffer[9] = (byte)((byte)unchecked((uint)(checked((byte)unchecked((uint)(txBuffer[2] + txBuffer[3]))) + txBuffer[4])) ^ 0x44);
			}
			serialPort.DiscardOutBuffer();
			serialPort.DiscardInBuffer();
			serialPort.Write(txBuffer, 0, 10);
			Thread.Sleep(500);
		}

	}

	public enum Dl24Commands 
	{ 
		OK = 50,
		Plus = 51,
		Minus = 52,
		Setup = 49,
		ElectricReset = 1,
		ResetAll = 5,
	}
}
