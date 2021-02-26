using System;
using System.Net.Sockets;
using System.IO;


namespace TwitchBotFramework
{
    public class TwitchClientIrc
    {
        public string UserName;
        public string ChannelName;
        private TcpClient _tcpClient;
        private StreamReader _streamIn;
        private StreamWriter _streamOut;
        private string _sendString;

        public delegate void standardeventdelegate(MessageEventArgs e);
        public static event standardeventdelegate ChatMessage;

        public TwitchClientIrc(string url, int port, string username, string password, string channel)
        {
            try
            {
                UserName = username;
                ChannelName = channel;
                Console.WriteLine("Connecting to tcp client");
                _tcpClient = new TcpClient(url, port);
                _streamIn = new StreamReader(_tcpClient.GetStream());
                _streamOut = new StreamWriter(_tcpClient.GetStream());
                _sendString = ":" + UserName + "!" + UserName + "@" + UserName + ".tmi.twitch.tv PRIVMSG #" + ChannelName + " :";

                _streamOut.WriteLine("PASS " + password);
                _streamOut.WriteLine("NICK " + username);
                _streamOut.WriteLine("USER " + username + " 8 * :" + username);
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
            catch (System.Net.Sockets.SocketException e)
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

                //Console.WriteLine(m);

                if (m.Contains("PRIVMSG"))
                {
                    int delimiterIndex = m.IndexOf('!');

                    string senderUserName = m.Substring(1, delimiterIndex - 1);
                    delimiterIndex = m.IndexOf(" :");
                    string senderMessage = m.Substring(delimiterIndex + 2);

                    ChatMessage?.Invoke(new MessageEventArgs(senderMessage, senderUserName));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    public class MessageEventArgs : EventArgs
    {

        public string Message;
        public string Sender;

        public MessageEventArgs(string message, string sender)
        {
            Message = message;
            Sender = sender;
        }
    }
}

