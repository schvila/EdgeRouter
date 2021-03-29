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

namespace FirewallConfigurator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;

            // get configuration
            Status = "Connecting";
            IsDPHC = false;
            UpdateUIOnConfig();

        }

        public bool IsDPHC { get; set; }
        public string IpText { get; set; }

        private string _status;

        public string Status
        {
            get => _status; 
            set
            {
                _status = value;
                RaisePropertyChanged(nameof(Status));
            }
        }
        public string Port { get; set; }
        public string Server { get; set; }
        public string Gateway { get; set; }

        private void typeDhcp_Click(object sender, RoutedEventArgs e)
        {
            //gridStatic.Visibility = Visibility.Hidden;
            IsDPHC = true;
            UpdateUIOnConfig();

        }

        private void typeStaticIP_Click(object sender, RoutedEventArgs e)
        {
            //gridStatic.Visibility = Visibility.Visible;
            IsDPHC = false;
            UpdateUIOnConfig();

        }
        private void UpdateUIOnConfig()
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
        }
    }
}
