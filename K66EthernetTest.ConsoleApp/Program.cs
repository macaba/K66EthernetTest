using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace K66EthernetTest.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConcurrentDictionary<UInt64, DateTime> sentDictionary = new ConcurrentDictionary<ulong, DateTime>();
            ConcurrentDictionary<UInt64, TimeSpan> roundTripTimes = new ConcurrentDictionary<ulong, TimeSpan>();

            Console.WriteLine("Starting receive task");
            Receive receive = new Receive();
            receive.Start(sentDictionary, roundTripTimes);
            Console.WriteLine("Starting transmit task");
            Transmit transmit = new Transmit();
            transmit.Start(sentDictionary);
            Console.WriteLine("Running...");


            while (true)
            {
                Console.Clear();
                //Console.WriteLine(roundTripTimes.Count.ToString());
                Console.WriteLine(sentDictionary.Count.ToString());
                Console.WriteLine(transmit.transmitCount.ToString());
                Console.WriteLine(receive.receiveCount.ToString());
                Thread.Sleep(100);
            }
        }
    }
}
