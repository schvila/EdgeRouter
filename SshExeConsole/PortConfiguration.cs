using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SshExeConsole
{
    public class PortConfigException : Exception
    {

    }
    public class PortConfiguration
    {
        public static List<string> ConfigReadCommands => new List<string>()
        {
            "configure",
            "show interfaces ethernet eth4",
            "show system gateway-address",
            "show system name-server",
            "exit"
        };

        private string _address;

        /// <summary>
        /// IP/Port or dhcp
        /// </summary>
        private string Address
        {
            get
            {
                if (_address == "dhcp")
                {
                    return _address;
                }

                return $"{IP}/{Port}";
            }
            set => _address = value;

        }

        public string IP { get; set; }
        public string Port { get; set; }
        public bool IsDhcp => !string.IsNullOrEmpty(Address) && Address.Trim().ToLower() == "dhcp";
        public string Server { get; set; }

        public string Gateway { get; set; }

        private string _cmdResult;

        public List<string> PrepareConfigWriteCommands(PortConfiguration org)
        {
            List<string> sl = new List<string>();
            
            if (!Equals(org) && IsValid())
            {
                sl.Add("configure");
                sl.Add("delete interfaces ethernet eth4 address");
                sl.Add($"set interfaces ethernet eth4 address {Address}");
                if (!string.IsNullOrEmpty(Server))
                {
                    sl.Add($"set system name-server {Server}");
                }
                if (!string.IsNullOrEmpty(Gateway))
                {
                    sl.Add($"set system gateway-address {Gateway}");
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
            if (string.IsNullOrEmpty(Address))
            {
                _validErrors.Add("Address is empty");
                return false;
            }
            if (string.IsNullOrEmpty(Port) && !IsDhcp)
            {
                _validErrors.Add("Port is empty");
                return false;
            }
            return true;
        }

        public bool Equals(PortConfiguration cfg)
        {
            return Address == cfg.Address &&
                   Port == cfg.Port &&
                   Server == cfg.Server &&
                   Gateway == cfg.Gateway;
        }
        public PortConfiguration(){}
        public PortConfiguration(string cmdResult)
        {
            _cmdResult = cmdResult;
            Parse(cmdResult);
        }

        private readonly Regex _addressRegex = new Regex(@"(?<==\saddress\s).*(?=\n)", RegexOptions.Multiline);
        private readonly Regex _serverRegex = new Regex(@"(?<=name-server\s)((?!is empty).)*(?=\n)", RegexOptions.Multiline);
        private readonly Regex _gatewayRegex = new Regex(@"(?<=gateway-address\s)((?!is empty).)*(?=\n)", RegexOptions.Multiline);
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

        public override string ToString()
        {
            return $"a:'{Address}' p:'{Port}' s:'{Server}' g:'{Gateway}'";
        }
    }
}
