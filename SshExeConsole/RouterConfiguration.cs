using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SshExeConsole
{
    public class RouterConfiguration
    {
        private string _address;
        public string Address
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

        //public bool IsConfigured
        //{
        //    get
        //    {
        //        if (!IsDhcp)
        //        {
        //            return !String.IsNullOrEmpty(IP) && !String.IsNullOrEmpty(Port);
        //        }

        //        return true;
        //    }
        //}
        public override string ToString()
        {
            return $"a:'{Address}' p:'{Port}' s:'{Server}' g:'{Gateway}'";
        }
    }
}
