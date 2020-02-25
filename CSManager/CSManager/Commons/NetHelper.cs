using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;

namespace CSManager.Commons
{
    public class NetHelper
    {
        public static bool Ping(string ip)
        {
            Ping ping = new Ping();
            PingReply repply = ping.Send(ip);
            if (repply.Status == IPStatus.Success)
                return true;
            else
                return false;
        }
        public static bool IsLocalIPAddress(string ip)
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in nics)
            {
                foreach (var uni in adapter.GetIPProperties().UnicastAddresses)
               {
                    if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (ip == uni.Address.ToString())
                            return true;
                    }
                }
            }
            return false;
        }
    }
}
