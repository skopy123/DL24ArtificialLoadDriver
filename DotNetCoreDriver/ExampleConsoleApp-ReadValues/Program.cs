using System;
using DL24Driver;

namespace ExampleConsoleApp_ReadValues
{
    class Program
    {
        static void Main(string[] args)
        {
            string portName = "";

            //parse command line parameters
            if (args.Length > 0)
            {
                if (args[0] == "-h")
                {
                    PrintHelp();
                    return;
                }
                if (args[0] == "-p")
                {
                    if (args.Length == 1)
                    {
                        Console.WriteLine("Invalid parameters! COM port not specified.");
                        return;
                    }
                    else
                    {
                        portName = args[1];
                    }
                }
            }
            else {
                Console.WriteLine("Please specify com port name.");
                PrintHelp();
                return;
            }

            Console.WriteLine("DL24p tester app");

            //Create driver instance and connect event handler
            DL24DeviceDriver dL = new DL24DeviceDriver();
            dL.DataRecieved += DL_DataRecieved;
            
            //open com port
            Console.WriteLine("Opening port " + portName);
            dL.Open(portName);
            
            PrintControlKeys();
            //infinite loop for scannig key press
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Q) break;
                if (key.Key == ConsoleKey.R)
                {
                    dL.SendCommand((byte)Dl24Commands.ElectricityReset); // reset energy counter (Wh)
                    dL.SendCommand((byte)Dl24Commands.ResetAll);// reset capacity counter (mAh)
                }
                if (key.Key == ConsoleKey.O) dL.SendCommand((byte)Dl24Commands.OK);
                if (key.Key == ConsoleKey.H) PrintHelp();
                if (key.Key == ConsoleKey.S) dL.SendCommand((byte)Dl24Commands.Setup);
                if (key.Key == ConsoleKey.UpArrow) dL.SendCommand((byte)Dl24Commands.Plus);
                if (key.Key == ConsoleKey.DownArrow) dL.SendCommand((byte)Dl24Commands.Minus);
            }

            //close com port before program ends.
            dL.DataRecieved -= DL_DataRecieved;
            dL.Close();
        }

        //event handler which prints value received from device.
        private static void DL_DataRecieved(object sender, CurrentValues values)
        {
            Console.WriteLine(values);
        }

        static void PrintHelp()
        {
            Console.WriteLine(@"
Command line parameters:
    -p COMPORT      Specify COM port number. Example -p COM3
    -h              Print this help.

");
            PrintControlKeys();
        }

        static void PrintControlKeys()
        {
            Console.WriteLine(@"
Keys used for control program:
    Q           Quit program
    H           Print this help
    R           Reset (execute commands ResetAll and ElectricityReset) reset mAh and kWh counters
    O           OK/ON/OFF button press
    S           Setup button press
    UpArrow     Plus button press
    DownArrow   Minus button press
");
        
        }
    }
}
