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
    public class Receive
    {
        private CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
        Task taskReceive;
        public UInt64 receiveCount = 0;


        public void Start(ConcurrentDictionary<UInt64, DateTime> sentDictionary, ConcurrentDictionary<UInt64, TimeSpan> roundTripTimes)
        {
            taskReceive = new Task(() => TaskReceive(cancelTokenSource.Token, 666, sentDictionary, roundTripTimes), TaskCreationOptions.LongRunning);
            taskReceive.Start();
        }

        public void Stop()
        {
            cancelTokenSource.Cancel();
        }

        private void TaskReceive(CancellationToken cancelToken, int port, ConcurrentDictionary<UInt64, DateTime> sentDictionary, ConcurrentDictionary<UInt64, TimeSpan> roundTripTimes)
        {
            using (UdpClient udpServer = new UdpClient(port))
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                    byte[] data = udpServer.Receive(ref remoteEP);
                    receiveCount++;
                    DateTime receivedTimestamp = DateTime.Now;
                    UInt64 sequence = BitConverter.ToUInt64(data, 0);
                    if (sentDictionary.Keys.Contains(sequence))
                    {
                        DateTime sentTimestamp;
                        if (sentDictionary.TryRemove(sequence, out sentTimestamp))
                        {
                            TimeSpan roundTripTime = receivedTimestamp - sentTimestamp;
                            roundTripTimes.TryAdd(sequence, roundTripTime);
                        }
                        else
                        {
                            Console.WriteLine("Error removing from sentDictionary");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error: Sequence ID not found");
                    }
                }
            }
        }
    }
}