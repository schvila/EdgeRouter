using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using JetBrains.Utility;
using FirewallConfigurator.Service;

namespace FirewallConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private RouterConfiguration OriginalConfiguration { get; set; }
        private RouterConfiguration CurrentConfiguration { get; set; } = new RouterConfiguration(){Address = "dhcp",};
        private static RouterCommands _commands = new RouterCommands();

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        //public override void BeginInit()
        //{
        //    base.BeginInit();
        //    Status = "Connecting";
        //    OriginalConfiguration = _commands.GetConfiguration();
        //    if (OriginalConfiguration == null)
        //    {
        //        CurrentConfiguration = new RouterConfiguration()
        //        {
        //            Address = "dhcp",
        //        };
        //        Status = "Connection failed";
        //        return;
        //    }
        //    else
        //    {
        //        CurrentConfiguration = new RouterConfiguration(OriginalConfiguration);
        //        Status = "Ready";

        //    }
        //}

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            UpdateVisibilityOnConfig();
            Status = "Connecting";
            Task.Run(TryConnect);
            if (OriginalConfiguration == null)
            {
                Status = "Connection failed";
                MessageBox.Show("Connection failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
            else
            {
                SetCurrentConfiguration(OriginalConfiguration);
            }
            Status = "Ready";
            UpdateVisibilityOnConfig();

        }

        private void TryConnect()
        {
            OriginalConfiguration = _commands.GetConfiguration();
        }
        private void SetCurrentConfiguration(RouterConfiguration originalConfiguration)
        {
            CurrentConfiguration = new RouterConfiguration(OriginalConfiguration);
            //WpfDispatcher?
            //RaisePropertyChanged(nameof(IP));
            //RaisePropertyChanged(nameof(Port));
            //RaisePropertyChanged(nameof(Server));
            //RaisePropertyChanged(nameof(Gateway));
        }

        public bool IsDPHC => CurrentConfiguration == null ? false : CurrentConfiguration.IsDhcp;

        public string IP 
        { 
            get => CurrentConfiguration == null ? "" : CurrentConfiguration.IP;
            set
            {
                CurrentConfiguration.IP = value;
                RaisePropertyChanged();
            }
        }

        private string _status;

        public string Status
        {
            get => _status; 
            set
            {
                _status = value;
                RaisePropertyChanged();
            }
        }

        public string Port
        {
            get => CurrentConfiguration.Port;
            set
            {
                CurrentConfiguration.Port = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ConfigChanged));
            }
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            int val;

            e.Handled = !((int.TryParse(e.Text, out val) && val >= 0));
        }
        public string Server 
        {
            get => CurrentConfiguration.Server;
            set
            {
                CurrentConfiguration.Server = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ConfigChanged));
            }
        }
        public string Gateway
        {
            get => CurrentConfiguration.Gateway;
            set
            {
                CurrentConfiguration.Gateway = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ConfigChanged));
            }
        }

        public bool ConfigChanged => CurrentConfiguration.ToString() != OriginalConfiguration.ToString();


        private void typeDhcp_Click(object sender, RoutedEventArgs e)
        {
            //gridStatic.Visibility = Visibility.Hidden;
            CurrentConfiguration.Address = "dhcp";
            UpdateVisibilityOnConfig();

        }

        private void typeStaticIP_Click(object sender, RoutedEventArgs e)
        {
            //gridStatic.Visibility = Visibility.Visible;
            CurrentConfiguration.Address = "";
            UpdateVisibilityOnConfig();

        }
        private void UpdateVisibilityOnConfig()
        {
            if (IsDPHC)
            {
                gridStatic.Visibility = Visibility.Hidden;
                typeDhcp.IsChecked = true;
                typeStaticIP.IsChecked = false;
                //typeStaticIP.Click += typeStaticIP_Click;
            }
            else
            {
                gridStatic.Visibility = Visibility.Visible;
                typeDhcp.IsChecked = false;
                typeStaticIP.IsChecked = true;
            }
            RaisePropertyChanged(nameof(ConfigChanged));
        }

        private async void _btnStore_OnClick(object sender, RoutedEventArgs e)
        {
            Status = "Storing";

            await Task.Run(()=> _commands.WriteConfiguration(OriginalConfiguration, CurrentConfiguration));
            OriginalConfiguration = new RouterConfiguration(CurrentConfiguration);
            RaisePropertyChanged(nameof(ConfigChanged));
            Status = "Ready";
        }
    }
}
