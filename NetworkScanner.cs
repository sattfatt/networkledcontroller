using System;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Net.Sockets;


namespace NetworkScanner
{
    public static class NetworkScanner
    {
        static CountdownEvent countdown;
        static int upCount = 0;
        static object lockObj = new object();
        public delegate void espip(string ip);
        public static event espip Espipfound;
        public static bool espFound { get; private set; }
        private static bool _alreadyFound;
        const bool resolveNames = true;
        public static bool isScanning { get; private set; }

        public static void Scan()
        {
            countdown = new CountdownEvent(1);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string ipBase = "192.168.86.";
            isScanning = true;
            espFound = false;
            _alreadyFound = false;
            for (int i = 0; i < 255; i++)
            {
                string ip = ipBase + i.ToString();
                Ping p = new Ping();

                p.PingCompleted += new PingCompletedEventHandler(p_PingCompleted);

                countdown.AddCount();

                p.SendAsync(ip, 100, ip);
            }

            countdown.Signal();
            countdown.Wait();
            sw.Stop();
            TimeSpan span = new TimeSpan(sw.ElapsedTicks);
            Console.WriteLine("Took {0} milliseconds. {1} hosts active", sw.ElapsedMilliseconds, upCount);
            isScanning = false;
            //Console.ReadLine();
        }

        static void p_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            string ip = (string)e.UserState;

            if (e.Reply != null && e.Reply.Status == IPStatus.Success)
            {
                if (resolveNames)
                {
                    string name;
                    try
                    {
                        IPHostEntry hostEntry = Dns.GetHostEntry(ip);
                        name = hostEntry.HostName;
                        if (name.Contains("esp"))
                        {
                            espFound = true;
                        }
                    }
                    catch (SocketException ex)
                    {
                        name = "?";
                    }
                    Console.WriteLine("{0} ({1}) is up: ({2} ms)", ip, name, e.Reply.RoundtripTime);
                }

                else
                {
                    Console.WriteLine("{0} is up: ({1} ms)", ip, e.Reply.RoundtripTime);
                }

                lock (lockObj)
                {
                    upCount++;
                    if (espFound && !_alreadyFound)
                    {
                        Espipfound?.Invoke(ip);
                        _alreadyFound = true;
                    }
                }
            }
            else if (e.Reply == null)
            {
                Console.WriteLine("Pinging {0} failed. Null reply object or something", ip);
            }
            countdown.Signal();

        }

    }

}
