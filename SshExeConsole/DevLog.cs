using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SshExeConsole
{
    public class DevLog
    {
        private string LogDir
        {
            get
            {
                var dir = Path.Combine(Assembly.GetExecutingAssembly().Location,"DevLog");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }
        private string FileName { get; set; }
        public string LogPathName => Path.Combine(LogDir, FileName);

        private DevLog(){}

        public DevLog(string fname)
        {
            FileName = fname;
        }

        public void Write(string text)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true))
            {
                file.WriteLine(text);
            }
        }
    }
}
