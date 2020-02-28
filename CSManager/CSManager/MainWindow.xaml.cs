using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Threading;

namespace CSManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            ServerService = new ServerStartClass();
            Schtask = new CSManager.CSSchtasks();


            DataContext = this;
            ConfigPath = string.Format(@"{0}\Server.Config", Environment.CurrentDirectory);
            var servers = GetConfig();
            Task initializeServer = new Task(InitialServers);
            initializeServer.Start();


        }

        private void InitialServers()
        {
            var servers = GetConfig();
            foreach(var s in servers)
            {
                ViewModel.ServerViewModel serverVm = new ViewModel.ServerViewModel() { Server = s };
                Servers.Add(serverVm);
            }
        }

        private void btn_startall_Click(object sender, RoutedEventArgs e)
        {
            foreach(var s in Servers)
            {
                var pingResult = Commons.NetHelper.Ping(s.IP);
                if(!pingResult)
                    ServerService.WakeServer(s.IP, s.Server.Mac);
            }
        }

        private void btn_shutdownall_Click(object sender, RoutedEventArgs e)
        {
            foreach(var s in Servers)
            {
                var pingResult = Commons.NetHelper.Ping(s.IP);
                if (!pingResult)
                {
                    s.State = Models.ServerState.Off;
                    continue;
                }
                Models.SchtaskModel shutdown = new Models.SchtaskModel()
                {
                    TaskName = Commons.Constants.ShutDown,
                    TaskPath = Commons.Constants.ShutDownBat
                };
                Schtask.Query(s.Server, shutdown);
                if (shutdown.TaskStatus == Models.SchtaskStatus.NotExist)
                {
                    Schtask.CreateTask(s.Server, shutdown);
                }
                if (shutdown.TaskStatus == Models.SchtaskStatus.Exist)
                    Schtask.Run(s.Server, shutdown);
                else
                    s.State = Models.ServerState.Warn;
            }
        }

        private void btn_startSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var value = ServerList.SelectedValue;
                if (value == null)
                    return;
                var model = value as Models.ServerModel;
                var mac = model.Mac;
                var ip = model.IP;
                ServerService.WakeServer(ip,mac);
            }
            catch (Exception)
            {

            }
        }
        #region 属性
        public ObservableCollection<ViewModel.ServerViewModel> Servers { get; set; } = new ObservableCollection<ViewModel.ServerViewModel>();
        public ServerStartClass ServerService { get; set; }
        public CSSchtasks Schtask { get; set; }
        public string ConfigPath { get; set; }

        #endregion

        private void btn_shutdown_Click(object sender, RoutedEventArgs e)
        {

            var value = ServerList.SelectedValue;
            if (value == null)
                return;

            var model = value as Models.ServerModel;
            var pingResult = Commons.NetHelper.Ping(model.IP);
            if (!pingResult)
            {
                model.State = Models.ServerState.Off;
                return;
            }
            Models.SchtaskModel shutdown = new Models.SchtaskModel
            {
                TaskName = Commons.Constants.ShutDown,
                TaskPath = Commons.Constants.ShutDownBat
            };
            Schtask.Query(model, shutdown);
            if (shutdown.TaskStatus == Models.SchtaskStatus.NotExist)
            {
                Schtask.CreateTask(model, shutdown);
            }
            if (shutdown.TaskStatus == Models.SchtaskStatus.Exist)
                Schtask.Run(model, shutdown);
            else
                model.State = Models.ServerState.Warn;
        }

        private void restart_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var model = value as Models.ServerModel;
            var pingResult = Commons.NetHelper.Ping(model.IP);
            if (!pingResult)
            {
                model.State = Models.ServerState.Off;
                return;
            }
            Models.SchtaskModel xServerStartModel = new Models.SchtaskModel
            {
                TaskName = Commons.Constants.ReStart,
                TaskPath = Commons.Constants.ReStartBat
            };
            Schtask.Query(model, xServerStartModel);
            if (xServerStartModel.TaskStatus == Models.SchtaskStatus.NotExist)
            {
                Schtask.CreateTask(model, xServerStartModel);
            }
            if (xServerStartModel.TaskStatus == Models.SchtaskStatus.Exist)
                Schtask.Run(model, xServerStartModel);
            else
                model.State = Models.ServerState.Warn;
        }

        private void runprocess_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var server = (value as ViewModel.ServerViewModel);
            var pingResult = Commons.NetHelper.Ping(server.IP);
            if (!pingResult)
                return;

            Models.SchtaskModel StartModel = new Models.SchtaskModel();
            switch (server.Mode)
            {
                case Models.ServerMode.xPanel:
                    {
                        StartModel.TaskName = Commons.Constants.xPanel;
                        StartModel.TaskPath = Commons.Constants.xPanelBat; break;
                    };
                case Models.ServerMode.xSwitch:
                    {
                        StartModel.TaskName = Commons.Constants.xSwitch;
                        StartModel.TaskPath = Commons.Constants.xSwitchBat; break;
                    };
                case Models.ServerMode.xServer:
                    {
                        StartModel.TaskName = Commons.Constants.xServer;
                        StartModel.TaskPath = Commons.Constants.xServerBat; break;
                    };
            }

            Schtask.Query(server.Server, StartModel);
            if (StartModel.TaskStatus == Models.SchtaskStatus.NotExist)
            {
                Schtask.CreateTask(server.Server, StartModel);
            }
            if (StartModel.TaskStatus == Models.SchtaskStatus.Exist)
                Schtask.Run(server.Server, StartModel);
            else
                server.State = Models.ServerState.Warn;
        }
        private void addserver_Click(object sender, RoutedEventArgs e)
        {
            ServerWindow serverWindow = new CSManager.ServerWindow();
            serverWindow.ShowDialog();
            var s = (Models.ServerModel)serverWindow.ServerModel.Clone();
            Servers.Add(new ViewModel.ServerViewModel() { Server = s });
            SetConfig();

        }

        private void deleteserver_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var server = (value as ViewModel.ServerViewModel);
            Servers.Remove(server);
            SetConfig();
        }

        private void editserver_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var server = (value as ViewModel.ServerViewModel);
            var sl = Servers.First((s) => s.IP == server.IP);

            ServerWindow serverWindow = new CSManager.ServerWindow();
            serverWindow.SetServer(server.Server);
            serverWindow.ShowDialog();
            if(serverWindow.IsUpdated)
            {
                Servers.Remove(sl);
                Servers.Add(new ViewModel.ServerViewModel { Server = serverWindow.ServerModel });
                SetConfig();
            }
        }
        private List<Models.ServerModel> GetServers()
        {
            List<Models.ServerModel> servers = new List<Models.ServerModel>();
            foreach(var s in Servers)
            {
                servers.Add(s.Server);
            }
            return servers;
        }
        private void SetConfig()
        {
            if (Servers != null)
            {

                Commons.XmlHelper<List<Models.ServerModel>>.SerialToXml(ConfigPath, GetServers());
            }
        }
        private List<Models.ServerModel> GetConfig()
        {
            var servers = Commons.XmlHelper<List<Models.ServerModel>>.Deserialize(ConfigPath);
            return servers;
        }
    }
}
