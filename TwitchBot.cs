using System;

namespace TwitchBotFramework
{
    public class Twitchbot
    {

        private static string _name = "ENTER YOUR OWN";
        private static string _broadcaster = "ENTER YOUR OWN";
        private static string _OAuth = "ENTER YOUR OWN";

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
            while (true)
            {
                client.ReadMessage();
            }
        }
    }

    
}

