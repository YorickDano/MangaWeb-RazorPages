using System.Net.NetworkInformation;

namespace MangaWeb.Managers
{
    public static class TestConnectionManager
    {
        private const string Host = "google.com";
        private const int Timeout = 3000;

        public static string GetDataBaseConnectionString()
        {
            var ping = new Ping();
            try
            {
                PingReply reply = ping.Send(Host, Timeout);

                if (reply.Status == IPStatus.Success)
                {
                    return "MangaWebContext";
                }
            }
            catch { return "MangaWebContextLocal"; }

            return "MangaWebContextLocal";
        }

        public static string GetLocalDataBaseConnectionString() =>
            "DefaultConnection";
    }
}
