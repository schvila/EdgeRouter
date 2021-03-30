using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace FirewallConfigurator.Service
{
    public class PortConfigurator
    {
        public static List<string> ConfigReadCommands => new List<string>()
        {
            "configure",
            "show interfaces ethernet eth4",
            "show system gateway-address",
            "show system name-server",
            "exit"
        };

        private RouterConfiguration _curRouterConfiguration = new RouterConfiguration();

        public RouterConfiguration CurrentRouterConfiguration
        {
            get => _curRouterConfiguration;
            private set => _curRouterConfiguration = value;
        }

        private string Address
        {
            get => CurrentRouterConfiguration.Address;
            set => CurrentRouterConfiguration.Address = value;
        }

        private string IP
        {
            get => CurrentRouterConfiguration.IP;
            set => CurrentRouterConfiguration.IP = value;
        }
        private string Port 
        { 
            get => CurrentRouterConfiguration.Port; 
            set => CurrentRouterConfiguration.Port = value;
        }

        private bool IsDhcp => CurrentRouterConfiguration.IsDhcp;

        private string Server
        {
            get => CurrentRouterConfiguration.Server; 
            set => CurrentRouterConfiguration.Server = value; 
        }

        public string Gateway 
        {
            get => CurrentRouterConfiguration.Gateway; 
            set => CurrentRouterConfiguration.Gateway = value; 
        }


    public List<string> PrepareConfigWriteCommands(RouterConfiguration org)
        {
            List<string> sl = new List<string>();
            
            if ((org.ToString() != CurrentRouterConfiguration.ToString()) && IsValid())
            {
                sl.Add("configure");
                sl.Add("delete interfaces ethernet eth4 address");
                sl.Add($"set interfaces ethernet eth4 address {Address}");
                sl.Add("delete system name-server");
                sl.Add("delete system gateway-address");
                if (!IsDhcp)
                {
                    if (!string.IsNullOrEmpty(Server))
                    {
                        sl.Add($"set system name-server {Server}");
                    }

                    if (!string.IsNullOrEmpty(Gateway))
                    {
                        sl.Add($"set system gateway-address {Gateway}");
                    }
                }

                sl.Add("commit ; save");
                sl.Add("exit");
            }

            return sl;
        }
        private List<string> _validErrors = new List<string>();

        public IList<string> ValidErrors => _validErrors.AsReadOnly();
        public bool IsValid()
        {
            _validErrors.Clear();
            if (!IsDhcp)
            {
                if (string.IsNullOrEmpty(Address))
                {
                    _validErrors.Add("Address is empty");
                    return false;
                }

                if (string.IsNullOrEmpty(Port))
                {
                    _validErrors.Add("Port is empty");
                    return false;
                }
            }

            return true;
        }


        public PortConfigurator()
        {
        }
        public PortConfigurator(RouterConfiguration cfg)
        {
            CurrentRouterConfiguration = cfg;
        }
        public PortConfigurator(string cmdResult)
        {
            Parse(cmdResult);
        }

        // name-server, gateway-address
        private readonly Regex _addressRegex = new Regex(@"(?<==\saddress\s).*(?=\n)", RegexOptions.Multiline);
        //private readonly Regex _serverRegex = new Regex(@"(?<=name-server\s)((?!is empty).)*(?=\n)", RegexOptions.Multiline);
        private readonly Regex _serverRegex = new Regex(@"(?<==\sname-server\s).*(?=\n)", RegexOptions.Multiline);
        //private readonly Regex _gatewayRegex = new Regex(@"(?<=gateway-address\s)((?!is empty).)*(?=\n)", RegexOptions.Multiline);
        private readonly Regex _gatewayRegex = new Regex(@"(?<==\sgateway-address\s).*(?=\n)", RegexOptions.Multiline);
        public void Parse(string txt)
        {
            var address = RegexVal(_addressRegex, txt);
            if (!string.IsNullOrEmpty(address))
            {
                if (address.Trim().ToLower() == "dhcp")
                    Address = address;
                else
                {
                    var m = Regex.Match(address, "(.*?)/(.*)");
                    IP = m.Groups[1].Value;
                    Port = m.Groups[2].Value;
                }
            }

            Server = RegexVal(_serverRegex, txt);
            Gateway = RegexVal(_gatewayRegex, txt);
        }

        private string RegexVal(Regex regex, string txt)
        {
            return regex.Matches(txt).Cast<Match>().FirstOrDefault()?.Captures.Cast<Capture>().FirstOrDefault()?.Value;
        }

    }
}
