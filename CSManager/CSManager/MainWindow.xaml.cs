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
using System.IO;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace CSManager
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
            ServerService = new ServerStartClass();
            Schtask = new CSManager.CSSchtasks();

            DataContext = this;
            ConfigPath = string.Format(@"{0}\Server.Config", Environment.CurrentDirectory);
            if(File.Exists(ConfigPath))
            {
                InitialServers();
            }
            this.Closing += MainWindow_Closing;

            TokenSource = new CancellationTokenSource();
            Task updateStateTask = new Task(Servers_CollectionChanged);
            updateStateTask.Start();
        }

        private async void Servers_CollectionChanged()
        {
            try
            {
                await Task.Delay(1000 * 5);

                while (!TokenSource.IsCancellationRequested)
                {
                    try
                    {
                        if (Servers != null)
                            await Task.Delay(1000 * 5);

                        foreach (var s in Servers)
                        {
                            var state = Commons.NetHelper.Ping(s.IP) ? Models.ServerState.Online : Models.ServerState.Off;
                            Application.Current.Dispatcher.Invoke(() => { s.State = state; });
                        }
                    }
                    catch (Exception ex)
                    {

                    }


                }
            }
            catch (Exception e)
            {

            }
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "退出",
                NegativeButtonText = "取消"
            };
            mySettings.ColorScheme = MetroDialogColorScheme.Accented;
            MessageDialogResult result = await this.ShowMessageAsync("...", "是否退出 ? ", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                TokenSource.Cancel();
                Environment.Exit(0);
            }
        }

        private async void InitialServers()
        {
            await Task.Delay(500);
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

        private async void btn_shutdownall_Click(object sender, RoutedEventArgs e)
        {
            //confirm if shutdown
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消"
            };
            mySettings.ColorScheme = MetroDialogColorScheme.Accented;
            MessageDialogResult result = await this.ShowMessageAsync("...", "是否关闭所有设备 ? ", MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                foreach (var s in Servers)
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
        }

        private void btn_startSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var value = ServerList.SelectedValue;
                if (value == null)
                    return;
                var model = value as ViewModel.ServerViewModel;
                var mac = model.Server.Mac;
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

        private CancellationTokenSource TokenSource;
        private object locker = new object();
        #endregion

        private async void btn_shutdown_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;

            var model = value as ViewModel.ServerViewModel;
            var pingResult = Commons.NetHelper.Ping(model.IP);
            if (!pingResult)
            {
                model.State = Models.ServerState.Off;
                return;
            }
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消"
            };
            mySettings.ColorScheme = MetroDialogColorScheme.Accented;
            var strWarnning = string.Format("是否关闭 {0} ?", model.IP);
            MessageDialogResult result = await this.ShowMessageAsync("...", strWarnning, MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                Models.SchtaskModel shutdown = new Models.SchtaskModel
                {
                    TaskName = Commons.Constants.ShutDown,
                    TaskPath = Commons.Constants.ShutDownBat
                };
                Schtask.Query(model.Server, shutdown);
                if (shutdown.TaskStatus == Models.SchtaskStatus.NotExist)
                {
                    Schtask.CreateTask(model.Server, shutdown);
                }
                if (shutdown.TaskStatus == Models.SchtaskStatus.Exist)
                    Schtask.Run(model.Server, shutdown);
                else
                    model.State = Models.ServerState.Warn;
            }
        }

        private async void restart_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var model = value as ViewModel.ServerViewModel;
            var pingResult = Commons.NetHelper.Ping(model.IP);
            if (!pingResult)
            {
                model.State = Models.ServerState.Off;
                return;
            }
            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消"
            };
            mySettings.ColorScheme = MetroDialogColorScheme.Accented;
            var strWarnning = string.Format("是否关闭 {0}", model.IP);
            MessageDialogResult result = await this.ShowMessageAsync("...", strWarnning, MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                Models.SchtaskModel xServerStartModel = new Models.SchtaskModel
                {
                    TaskName = Commons.Constants.ReStart,
                    TaskPath = Commons.Constants.ReStartBat
                };
                Schtask.Query(model.Server, xServerStartModel);
                if (xServerStartModel.TaskStatus == Models.SchtaskStatus.NotExist)
                {
                    Schtask.CreateTask(model.Server, xServerStartModel);
                }
                if (xServerStartModel.TaskStatus == Models.SchtaskStatus.Exist)
                    Schtask.Run(model.Server, xServerStartModel);
                else
                    model.State = Models.ServerState.Warn;
            }
                
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
            else
            {
                server.State = Models.ServerState.Online;
            }

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
            var s = (Models.ServerModel)serverWindow.ServerModel;
            if (s == null)
                return;
            var server = (Models.ServerModel)s.Clone();
            var newserver = new ViewModel.ServerViewModel() { Server = server };
            Servers.Add(newserver);
            SetConfig();
        }

        private async void deleteserver_Click(object sender, RoutedEventArgs e)
        {
            var value = ServerList.SelectedValue;
            if (value == null)
                return;
            var server = (value as ViewModel.ServerViewModel);

            var mySettings = new MetroDialogSettings
            {
                AffirmativeButtonText = "确定",
                NegativeButtonText = "取消"
            };
            mySettings.ColorScheme = MetroDialogColorScheme.Accented;
            var strWarnning = string.Format("是否删除服务器 {0} ?", server.IP);
            MessageDialogResult result = await this.ShowMessageAsync("...", strWarnning, MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if (result == MessageDialogResult.Affirmative)
            {
                Servers.Remove(server);
                SetConfig();
            }
              
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
                //Servers.Remove(sl);
                //Servers.Add(new ViewModel.ServerViewModel { Server = serverWindow.ServerModel });
                var editServer = serverWindow.ServerModel;
                sl.IP = editServer.IP;
                sl.ServerName = editServer.ServerName;
                sl.Mode = editServer.Mode;
                sl.Server = editServer;
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
