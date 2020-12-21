using System;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TwitchBotFramework 
{
    public class TwitchBotFramework 
    {
        public static void Main(string[] args) 
        {
    	    Console.Write("test");
            Twitchbot t = new Twitchbot();

            t.Start();
	    }
    }
    
    public class Twitchbot 
    {
    
        private static string _name = "boringprofessorbot";
        private static string _broadcaster = "boringmathprofessor";
        private static string _OAuth = "oauth:qhj1i8177eopunuo6kz8v0xfr0wezz";
        
        private TwitchClientIrc client;
        private Pinger pinger;

        public Twitchbot()
        {
            client = new TwitchClientIrc("irc.twitch.tv", 6667, _name, _OAuth, _broadcaster);
            pinger = new Pinger(client);
        }  

        public void Start() 
        {
            pinger.Start();
            while(true)
            {
                client.ReadMessage();
            }      
	    }
    }
    
    public class MessageEventArgs : EventArgs
    {
     
        public string Message;
        public string Sender;
     
        public MessageEventArgs(string message, string sender) {
            Message = message;
            Sender = sender; 
        }
    }

    public class TwitchClientIrc
    {
        public string UserName;
        public string ChannelName;
        private TcpClient _tcpClient;
        private StreamReader _streamIn;
        private StreamWriter _streamOut;
        private string _sendString; 
        
        public delegate void standardeventdelegate(EventArgs e);
        public static event standardeventdelegate ChatMessage;
        
        public TwitchClientIrc(string url, int port, string username, string password, string channel)
        {
            try
            {
                UserName = username;
                ChannelName = channel;
                _tcpClient = new TcpClient(url, port);
                _streamIn = new StreamReader(_tcpClient.GetStream());
                _streamOut = new StreamWriter(_tcpClient.GetStream());
                _sendString = ":" + UserName + "!" + UserName + "@" + UserName + ".tmi.twitch.tv PRIVMSG #" + ChannelName + " :";

                _streamOut.WriteLine("PASS " + password);
                _streamOut.WriteLine("NICK " + username);
                _streamOut.WriteLine("USER " + username+" 8 * :" + username);
                _streamOut.WriteLine("JOIN #" + channel);
               
                _streamOut.Flush();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        public void Send(string m)
        {
            try
            {
                _streamOut.WriteLine(m);
                _streamOut.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SendChatMessage(string m)
        {
            Send(_sendString + m);
        }

        private string Read() 
        {
            try
            {
                string m = _streamIn.ReadLine();
                return m; 
            }
            catch(Exception e)
            {
                Console.WriteLine("Error reading input stream" + e);
                return "";
            }
        }

        public void ReadMessage()
        {
            try
            {
                string m = Read();

                Console.WriteLine(m);

                if (m.Contains("PRIVMSG"))
                {
                    int delimiterIndex = m.IndexOf('!');

                    string senderUserName = m.Substring(1, delimiterIndex - 1);
                    delimiterIndex = m.IndexOf(" :");
                    string senderMessage = m.Substring(1, delimiterIndex + 2);

                    ChatMessage?.Invoke(new MessageEventArgs(senderMessage, senderUserName));
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }


    public class Pinger
    {
        private TwitchClientIrc _client;
        private Thread _pingThread;


        public Pinger(TwitchClientIrc client)
        {
            _client = client;
            _pingThread = new Thread(new ThreadStart(this.pingEveryFiveMinutes));
        } 

        public void Start()
        {
            _pingThread.IsBackground = true;
            _pingThread.Start();
        }

        public void pingEveryFiveMinutes()
        {
            while(true)
            {
                _client.Send("PING irc.twitch.tv");
                Thread.Sleep(30000);
            }
        }
    }
}

