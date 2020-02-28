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
using System.Windows.Shapes;
using CSManager.Models;

namespace CSManager
{
    /// <summary>
    /// ServerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ServerWindow : Window
    {
        public ServerWindow()
        {
            InitializeComponent();
            List<string> modes = new List<string>
            {
                "xPanel","xServer","xSwitch"
            };
            this.txt_mode.ItemsSource = modes;
            
        }

        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServerModel = new Models.ServerModel();
                ServerModel.IP = txt_ip.Text;
                ServerModel.Mac = txt_mac.Text;
                ServerModel.Mode = (Models.ServerMode)Enum.Parse(typeof(Models.ServerMode), txt_mode.SelectedValue.ToString());
                ServerModel.Password = txt_password.Text;
                ServerModel.ServerName = txt_servername.Text;
                ServerModel.State = Models.ServerState.Off;
                ServerModel.UserName = txt_username.Text;
                this.Close();
            }
            catch (Exception)
            {

            }
        }

        public Models.ServerModel ServerModel { get; set; }
        public bool IsUpdated { get; set; }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        internal void SetServer(ServerModel server)
        {
            
            ServerModel = (Models.ServerModel)server.Clone();
            txt_ip.Text = ServerModel.IP;
            txt_mac.Text = ServerModel.Mac;
            txt_mode.Text = ServerModel.Mode.ToString();
            txt_password.Text = ServerModel.Password;
            txt_servername.Text = ServerModel.ServerName;
            txt_username.Text = ServerModel.UserName;
            IsUpdated = true;
        }
    }
}
