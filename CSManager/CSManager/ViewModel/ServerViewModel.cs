using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSManager.ViewModel
{
    public class ServerViewModel:ViewModelBase
    {
        public ServerViewModel()
        {
            Server = new Models.ServerModel();
        }
        public Models.ServerModel Server { get; set; }


        public string ServerName
        {
            get { return Server.ServerName; }
            set { Server.ServerName = value; RaisePropertyChanged("ServerName"); }
        }
        public string IP
        {
            get { return Server.IP; }
            set { Server.IP = value; RaisePropertyChanged("IP"); }
        }
        public Models.ServerState State
        {
            get { return Server.State; }
            set { Server.State = value; RaisePropertyChanged("State"); }
        }
        public Models.ServerMode Mode
        {
            get { return Server.Mode; }
            set { Server.Mode = value; RaisePropertyChanged("Mode"); }
        }
    }
}
