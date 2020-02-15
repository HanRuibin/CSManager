using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSManager.Models
{
    public class ServerModel
    {
        public int Num { get; set; }
        public string IP { get; set; }
        public ServerState State { get; set; }
        public string Mac { get; set; }
    }
}
