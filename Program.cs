using System;
using TwitchBotFramework;
using System.Net;
using System.Threading;
using System.Diagnostics;

namespace LedController
{

    using NetworkScanner;
    

    public class Program
    {
        private static string _espIP;

        public static Stopwatch chatsw;

        public static int chatCommandInterval = 5;

        public static void Main(string[] args)
        {
            
            Twitchbot t = new Twitchbot();
            NetworkScanner.Espipfound += OnEspFound;
            chatsw = new Stopwatch();
            chatsw.Start();
            // scan for the the esp and keep scanning untill found.
            while (!NetworkScanner.espFound && !NetworkScanner.isScanning)
            {
                NetworkScanner.Scan();
                Thread.Sleep(500);
            }

            // sub to the bot message event

            TwitchClientIrc.ChatMessage += OnTwitchChat;

            // start the twitch bot
            
            t.Start();
        }


        public static void OnEspFound(string ip)
        {
            Console.WriteLine("ESP was found!! " + ip);
            _espIP = ip;
        }

        public static void OnTwitchChat(MessageEventArgs e)
        {

            Console.WriteLine(chatsw.ElapsedMilliseconds);

            if(e.Message == "!randomcolor")
            {
                if(chatsw.ElapsedMilliseconds > chatCommandInterval * 1000f)
                {
                    chatsw.Restart();

                    Console.Write(e.Sender + ": ");
                    Console.WriteLine(e.Message);
                    Random rnd = new Random();


                    Color c1 = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    Color c2 = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    Color c3 = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));
                    Color c4 = new Color(rnd.Next(0, 255), rnd.Next(0, 255), rnd.Next(0, 255));

                    PostToESP(_espIP, c1, c2, c3, c4);
                }
            }
        }

        public static void PostToESP(string ip, Color c1, Color c2, Color c3, Color c4)
        {
            WebClient wc = new WebClient();
            var URI = new Uri("http://" + ip + "/colorinput/");

            wc.Headers["Content-Type"] = "application/x-www-form-urlencoded";

            wc.UploadStringAsync(URI, "POST", "InputColor0=%23" + c1.hex + "&InputColor1=%23" + c2.hex + "&InputColor2=%23" + c3.hex + "&InputColor3=%23" + c4.hex);

        }

        void wc_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            try
            {
               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    public class Color
    {
        public int r { private set; get; }
        public int g { private set; get; }
        public int b { private set; get; }
        public string hex { private set; get; }
        public Color(int r, int g, int b)
        {
            
            if (r < 0) this.r = 0; else if (r > 255) this.r = 255;
            if (g < 0) this.g = 0; else if (g > 255) this.g = 255;
            if (b < 0) this.b = 0; else if (b > 255) this.b = 255;


            this.hex = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }

    }
}

