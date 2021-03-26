using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SshExeConsole
{
    public class PortConfiguration
    {
        public static List<string> GetConfigCommands => new List<string>()
        {
            "configure",
            "show interfaces ethernet eth4",
            "show system gateway-address",
            "show system name-server",
            "exit"
        };

        public string Address { get; set; }
        public string Port { get; set; }
        public bool IsDhcp => !string.IsNullOrEmpty(Address) && Address.Trim().ToLower() == "dhcp";
        public string Server { get; set; }

        public string Gateway { get; set; }

        private string _cmdResult;
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
            Address = RegexVal(_addressRegex, txt);
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
