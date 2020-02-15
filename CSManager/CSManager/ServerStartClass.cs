using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace CSManager
{
    public class ServerStartClass
    {
        public bool WakeServer(string ip,string macString)
        {
            try
            {
                var mac = StringToBytes(macString);
                UdpClient client = new UdpClient();
                client.Connect(ip, 7070);

                byte[] magicPacket = new byte[17*6];
                for (int i = 0; i < 6; i++)
                    magicPacket[i] = 0xFF;

                for (int i = 1; i <= 16; i++)
                    for (int j = 0; j < 6; j++)
                        magicPacket[i * 6 + j] = mac[j];

                int result = client.Send(magicPacket, magicPacket.Length);
                client.Close();
                return true;
            }
            catch (Exception e)
            {

                return false;
            }
        }

        public bool ShutDown()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool StartxServer()
        {
            throw new NotImplementedException();
        }
        private byte[] StringToBytes(string str)
        {
            byte[] hexBytes = new byte[6];
            for(int i=0;i<str.Length/2;i++)
            {
                var b = str.Substring(i * 2, 2);

                var hex = Convert.ToByte(b,16);
                hexBytes[i] = hex;
            }
            return hexBytes;
        }
    }
}
