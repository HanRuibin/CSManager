using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSManager.Models
{
    public class ServerModel:ICloneable
    {
        public int Num { get; set; }
        public string ServerName { get; set; }
        public string IP { get; set; }
        public ServerState State { get; set; }
        public string Mac { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public ServerMode Mode { get; set; }

        public static implicit operator ServerMode(ServerModel v)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
