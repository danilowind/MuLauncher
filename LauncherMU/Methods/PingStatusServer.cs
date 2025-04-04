using System.Net.NetworkInformation;
using System.Net.Sockets;

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
        public static bool CheckServerPing(string ip, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var result = client.BeginConnect(ip, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000));
                    return success && client.Connected;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
