using System.Net.NetworkInformation;

namespace LauncherMU.Methods
{
    class PingStatusServer
    {
        public static bool CheckServerPing(string serverAddress)
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(serverAddress);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false; 
            }
        }
    }
}
