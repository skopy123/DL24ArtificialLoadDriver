using System;

namespace ExampleConsoleApp_ReadValues
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("DL24p tester app");
            DL24DeviceDriver dL = new DL24DeviceDriver();

            Console.WriteLine("Connecting to COM6");
            dL.Open("COM6");
            Console.WriteLine("Press Q for quit");

            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;
                if (key.Key == ConsoleKey.R) dL.SendCommand((byte)Dl24Commands.ElectricReset); // reset energy counter (Wh)
                if (key.Key == ConsoleKey.O) dL.SendCommand((byte)Dl24Commands.OK);
                if (key.Key == ConsoleKey.T) dL.SendCommand((byte)Dl24Commands.ResetAll);// reset capacity counter (mAh)
                if (key.Key == ConsoleKey.S) dL.SendCommand((byte)Dl24Commands.Setup);
                if (key.Key == ConsoleKey.UpArrow) dL.SendCommand((byte)Dl24Commands.Plus);
                if (key.Key == ConsoleKey.DownArrow) dL.SendCommand((byte)Dl24Commands.Minus);
            }
            dL.Close();
        }
    }
}
