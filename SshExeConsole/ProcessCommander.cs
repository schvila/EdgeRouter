using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SshExeConsole
{
    public class ProcessCommander
    {
        public ConnectInfo ConInfo { get; set; } = new ConnectInfo();

        //private ProcessIoManager _procManager;
        private ProcessStartInfo _cmdExeProcessStartInfo = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"))
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false,
        };
        public string Run(List<string> cmdList, int sleep)
        {
            using (var proc = Process.Start(_cmdExeProcessStartInfo))
            {
                var procManager = new ProcessIoManager(proc);
                procManager.StdoutTextRead += OnTextRead;
                procManager.StderrTextRead += OnErroRead;
                //    (txt) =>
                //{
                //    var scReplace = txt.Replace("\r", "");
                //    sb.Append(scReplace);
                //};
                procManager.StartProcessOutputRead();
                try
                {
                    // Router connect
                    procType = ProcesType.Logon;
                    procManager.WriteStdin(ConInfo.Connection);
                    Thread.Sleep(sleep);
                    if (!LogonFailed())
                    {
                        procType = ProcesType.Command;
                        foreach (var cmd in cmdList)
                        {
                            procManager.WriteStdin(cmd);
                            Thread.Sleep(sleep);
                        }

                        procType = ProcesType.Logon;

                        procManager.WriteStdin("exit");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    procManager.StdoutTextRead -= OnTextRead;
                    procManager.StderrTextRead -= OnErroRead;

                    procManager.StopMonitoringProcessOutput(true);
                    Thread.Sleep(500);
                    proc.Close();
                }
            }
            WriteToDevLog();
            return CmdResult;
        }

        private bool LogonFailed()
        {
            if ((_sberror.Length > 0 && _sberror.ToString().Contains("FATAL ERROR")) || !_sblogon.ToString().Contains(ConInfo.User))
                logonError = "Logon failed";

            return !string.IsNullOrEmpty(logonError);
        }
        private Regex _rxEol = new Regex(@"([\u001B]+\[[m])|([\u001B])", RegexOptions.Multiline);
        private string logonError = string.Empty;
        public string CmdResult
        {
            get
            {
                if(!string.IsNullOrEmpty(logonError))
                    return logonError;
                return _rxEol.Replace(_sbcmd.ToString(), "");
            }
        }

        

        private readonly DevLog _cmdLog = new DevLog("Cmd.log", false);
        private readonly DevLog _cmdLogEscaped = new DevLog("CmdEscaped.log", true);
        private readonly DevLog _logonLog = new DevLog("Login.log", false);
        private void WriteToDevLog()
        {
            _cmdLog.Write(_sbcmd.ToString());
            _logonLog.Write(_sblogon.ToString());
            _cmdLogEscaped.Write(CmdResult);
            if (_sberror.Length > 0)
            {
                DevLog errLog = new DevLog("Error.log", false);
                errLog.Write(_sberror.ToString());
            }
        }

        private ProcesType procType = ProcesType.Logon;
        enum ProcesType
        {
            Unknown,
            Logon,
            Command
        }
        private readonly StringBuilder _sbcmd = new StringBuilder(256);
        private readonly StringBuilder _sblogon = new StringBuilder(256);
        private readonly StringBuilder _sberror = new StringBuilder(256);

        private void OnErroRead(string text)
        {
            var scReplace = text.Replace("\r", "");
            _sberror.Append(scReplace);

        }
        private void OnTextRead(string text)
        {
            var scReplace = text.Replace("\r", "");

            switch (procType)
            {
                case ProcesType.Logon:
                    _sblogon.Append(scReplace);
                    break;
                case ProcesType.Command:
                    _sbcmd.Append(scReplace);
                    break;
                default:
                    break;
                    ;
            }
        }
    }

    //"c:\_putytool\plink.exe  schvila@192.168.202.128 -pw SCh*1108"
    public class ConnectInfo
    {
        public string Putty => @"c:\_putytool\plink.exe";
        public string User => "thermofisherscientific";
        public string Host => "192.168.0.30";
        public string Password => "thermofisherscientific";

        public string Connection => $"{Putty} {User}@{Host} -pw {Password}";
    }
}
