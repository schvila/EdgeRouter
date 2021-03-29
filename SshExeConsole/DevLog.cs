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
                var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"DevLog");
                Directory.CreateDirectory(dir);
                return dir;
            }
        }
        private string FileName { get; set; }
        public string LogPathName => Path.Combine(LogDir, FileName);

        private DevLog(){}

        public DevLog(string fname, bool clear)
        {
            FileName = fname;
            if (clear && File.Exists(LogPathName))
            {
                File.Delete(LogPathName);
            }
        }

        public void Write(string text)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(LogPathName, true))
            {
                file.WriteLine(text);
            }
        }
    }
}
