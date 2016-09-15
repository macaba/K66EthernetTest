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
    public sealed class Transmit
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        Task taskTransmit;
        public UInt64 transmitCount = 0;

        public void Start(ConcurrentDictionary<UInt64, DateTime> sentDictionary)
        {
            taskTransmit = new Task(() => TaskTransmit(cancelTokenSource.Token, "192.168.1.222", 666, sentDictionary), TaskCreationOptions.LongRunning);
            taskTransmit.Start();
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
        }

        private void TaskTransmit(CancellationToken cancelToken, string ip, int port, ConcurrentDictionary<UInt64, DateTime> sentDictionary)
        {
            var client = new UdpClient();
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);
            client.Connect(ep);
            UInt64 sequence = 0;

            while (!cancelToken.IsCancellationRequested)
            {
                byte[] sequenceBytes = BitConverter.GetBytes(sequence++);
                DateTime transmitTimestamp = DateTime.Now;
                client.Send(sequenceBytes, 8);
                transmitCount++;
                if (!sentDictionary.TryAdd(sequence, transmitTimestamp))
                {
                    Console.WriteLine("Error adding to sentDictionary");
                }
                Thread.Sleep(1);
            }
        }
    }
}
