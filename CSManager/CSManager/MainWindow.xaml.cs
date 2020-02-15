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
            this.serverlist.ItemsSource = Servers;
            Models.ServerModel server = new Models.ServerModel
            {
                Num = 1,
                IP = "127.0.0.1",
                State = Models.ServerState.Off,
                Mac = "A0B0C0D0E0F0"
            };
            Models.ServerModel server2 = new Models.ServerModel
            {
                Num = 1,
                IP = "127.0.0.1",
                State = Models.ServerState.Off,
                Mac = "A1B1C1D1E1F1"
            };
            Servers.Add(server2);
            Servers.Add(server);

            ServerService = new ServerStartClass();
        }

        private void btn_startall_Click(object sender, RoutedEventArgs e)
        {
            foreach(var s in Servers)
            {
                ServerService.WakeServer(s.IP, s.Mac);
            }
        }

        private void btn_shutdownall_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_addserver_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_subserver_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_startSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var value = serverlist.SelectedValue;
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
        public ObservableCollection<Models.ServerModel> Servers { get; set; } = new ObservableCollection<Models.ServerModel>();
        public ServerStartClass ServerService { get; set; }
        #endregion

        private void btn_shutdown_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
