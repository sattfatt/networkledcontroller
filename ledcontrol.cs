using System;
using System.Net.Sockets;
using System.IO;


namespace LedController
{
    public class LedControl {
	
        public static void Main(string[] args) {
    
    	    Console.Write("test");
         
	    }
    }
    
    public class Twitchbot 
    {
    
        private static string _name = "boringprofessorbot";
        private static string _broadcaster = "boringmathprofessor";
        private static string _OAuth = "oauth:qhj1i8177eopunuo6kz8v0xfr0wezz";
    
        public delegate void standardeventdelegate(EventArgs e);
        public event standardeventdelegate TwitchChatMessage;
    
     
        public static void Start() 
        {
             
	    }
    }
    
    public class MessageEventArgs : EventArgs
    {
     
        public readonly string Message;
        public readonly string Sender;
     
        MessageEventArgs(string message, string sender) {
            Message = message;
            Sender = sender; 
        }
    }

    public class ClientIrc
    {
        public string UserName;
        public string ChannelName;
        private TcpClient _tcpClient;
        private StreamReader _streamIn;
        private StreamWriter _streamOut;

        public ClientIrc(string url, int port, string username, string password, string channel)
        {
            try
            {
                UserName = username;
                ChannelName = channel;
                _tcpClient = new TcpClient(url, port);
                _streamIn = new StreamReader(_tcpClient.GetStream());
                _streamOut = new StreamReader(_TcpClient.GetStream());


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
        

    }
}
