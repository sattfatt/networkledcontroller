using System.Threading;


namespace TwitchBotFramework
{
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
            while (true)
            {
                _client.Send("PING irc.twitch.tv");
                Thread.Sleep(30000);
            }
        }
    }
}

