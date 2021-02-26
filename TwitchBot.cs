using System;

namespace TwitchBotFramework
{
    public class Twitchbot
    {

        private static string _name = "boringprofessorbot";
        private static string _broadcaster = "boringmathprofessor";
        private static string _OAuth = "oauth:qd1ifa53rj37pma9sr19247jvvsuit";

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

