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
            else if (e.Message.Contains("!hue"))
            {
                if (chatsw.ElapsedMilliseconds > chatCommandInterval * 1000f)
                {
                    chatsw.Restart();

                    Console.Write(e.Sender + ": ");
                    Console.WriteLine(e.Message);

                    string[] args = e.Message.Split(' ');

                    float h1, h2, h3, h4;
                    double s, v;
                    h1 = 0f; h2 = 0f; h3 = 0f; h4 = 0f;
                    s = 1d; v = 1d;

                    int argcount = args.Length;

                    if (argcount == 5)
                    {
                        float.TryParse(args[1], out h1);
                        float.TryParse(args[2], out h2);
                        float.TryParse(args[3], out h3);
                        float.TryParse(args[4], out h4);
                    }

                    Color c1 = new Color();
                    c1.SetHSV(h1, s, v);
                    Color c2 = new Color();
                    c2.SetHSV(h2, s, v);
                    Color c3 = new Color();
                    c3.SetHSV(h3, s, v);
                    Color c4 = new Color();
                    c4.SetHSV(h4, s, v);

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
               // here we can do stuff but we dont really care about what the server sends back for now.
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
            UpdateHex();
        }

        public Color()
        {
            r = 255;
            g = 255;
            b = 255;
            UpdateHex();
        }

        public void SetHSV(double h, double S, double V)
        {
            // hsv should be 0 to 1
            // rgb out is 0 to 255

            double H = h * 360;

            while (H<0) { H += 360; };
            while (H>=360) { H -= 360; };
            
            double R, G, B;

            if (V <= 0)
            { 
                R = G = B = 0; 
            }
            else if (S <= 0)
            {
                R = G = B = V;
            }
            else
            {
                double hf = H / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = V * (1 - S);
                double qv = V * (1 - S * f);
                double tv = V * (1 - S * (1 - f));
                switch(i)
                {
                    case 0:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case 1:
                        R = qv;
                        G = V;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = V;
                        B = tv;
                        break;
                    case 3:
                        R = pv;
                        G = qv;
                        B = V;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = V;
                        break;
                    case 5:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    case 6:
                        R = V;
                        G = tv;
                        B = pv;
                        break;
                    case -1:
                        R = V;
                        G = pv;
                        B = qv;
                        break;
                    default:
                        R = G = B = V;
                        break;
                }

                this.r = Clamp((int)(R * 255.0));
                this.g = Clamp((int)(G * 255.0));
                this.b = Clamp((int)(B * 255.0));
                UpdateHex();
            }
        }

        private int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

        private void UpdateHex()
        {
            this.hex = r.ToString("X2") + g.ToString("X2") + b.ToString("X2");
        }
    }
}

