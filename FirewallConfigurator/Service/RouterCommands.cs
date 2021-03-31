using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirewallConfigurator.Service
{
    public class RouterCommands
    {
        public RouterCommands()
        {
        }
        public string LastError { get; private set; }= String.Empty;
        public RouterConfiguration GetConfiguration()
        {
            LastError = String.Empty;
            
            ProcessManager processManager = new ProcessManager();

            var res = processManager.Run(PortConfigurator.ConfigReadCommands, 500, 5000);
            if (processManager.ConnectionFailed ||string.IsNullOrEmpty(res))
            {
                LastError = "Connection failed";
                return null;
            }

            // get configuration
            var pcfg = new PortConfigurator(res);
            return pcfg.CurrentRouterConfiguration;
        }

        public string WriteConfiguration(RouterConfiguration orgcfg, RouterConfiguration newcfg)
        {
            LastError = String.Empty;
            PortConfigurator portConfigurator = new PortConfigurator(newcfg);
            var cmdList = portConfigurator.PrepareConfigWriteCommands(orgcfg);
            
            if (cmdList.Count == 0)
                return "Configuration not changed or invalid";
            ProcessManager processManager = new ProcessManager();

            return processManager.Run(cmdList, 1000);
        }

    }
}
