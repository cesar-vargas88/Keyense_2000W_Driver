using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Keyense_2000W_Driver
{
    class Program
    {
        static void Main(string[] args)
        {
            Keyence2000W Scanner = new Keyence2000W("192.168.0.1", 9004);

            while(true)
            {
                Console.WriteLine(Scanner.Trigger("LON\r", "LOFF\r", 200) + "\n\n");
                Thread.Sleep(2000);
            }
        }
    }
}
