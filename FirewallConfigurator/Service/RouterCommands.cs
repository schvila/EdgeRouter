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
        public RouterConfiguration GetConfiguration()
        {
            ProcessManager processManager = new ProcessManager();

            var res = processManager.Run(PortConfigurator.ConfigReadCommands, 500);
            // get address
            var pcfg = new PortConfigurator(res);
            return pcfg.CurrentRouterConfiguration;
        }

        public string WriteConfiguration(RouterConfiguration orgcfg, RouterConfiguration newcfg)
        {
            PortConfigurator portConfigurator = new PortConfigurator(newcfg);
            var cmdList = portConfigurator.PrepareConfigWriteCommands(orgcfg);
            
            if (cmdList.Count == 0)
                return "Configuration not changed or invalid";
            ProcessManager processManager = new ProcessManager();

            return processManager.Run(cmdList, 1000);
        }

    }
}
