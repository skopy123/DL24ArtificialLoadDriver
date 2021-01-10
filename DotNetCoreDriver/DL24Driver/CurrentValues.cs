using System;
using System.Collections.Generic;
using System.Text;

namespace DL24Driver
{
    public class CurrentValues
    {
		public double Voltage; //V
		public double Current; //A
		public double Power; //W
		public double ElectricityKwh;
		public double InternalTemperature; //℃
		public double BatteryCapacity; //Ah

        public override string ToString()
        {
			return string.Format("{0}V;{1}A;{2}W;{3}Ah;{4}C", Voltage, Current, Power, BatteryCapacity, InternalTemperature);
        }

    }
}
