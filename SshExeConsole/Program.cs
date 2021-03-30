using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SshExeConsole
{
    class Program
    {
        private static RouterCommands _commands = new RouterCommands();
        static void Main(string[] args)
        {
            try
            {
                var orgcfg = _commands.GetConfiguration();
                Console.WriteLine(orgcfg.ToString());
                RouterConfiguration newcfg = new RouterConfiguration()
                {
                    IP = "192.168.1.80",
                    Port = "24",
                    Server = "192.168.1.1",
                    Gateway = "192.168.1.1",
                };
                var res = _commands.WriteConfiguration(orgcfg, newcfg);
                Console.WriteLine(res);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Press any key");
                Console.ReadLine();
            }
        }


        #region obsolette tests
        private static void ShowCmd()
        {
            Console.WriteLine("cmd.exe test");
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
            psi.Arguments = " /C dir"; ;
            psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            using (var prc = Process.Start(psi))
            {
                //StringBuilder sb = new StringBuilder();
                //prc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                //{
                //    sb.AppendLine(e.Data);

                //});
                //prc.StandardInput.WriteLine($"/C dir {Environment.NewLine}");
                //prc.StandardInput.Flush();
                ////prc.BeginOutputReadLine();
                //StringBuilder ob = new StringBuilder();
                //prc.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                //{
                //    sb.AppendLine(e.Data);

                //});
                var t = prc.StandardOutput.ReadToEnd();
                Console.WriteLine(t);
                prc.WaitForExit();

            }
        }

        static void SsHConnect()
        {
            Console.WriteLine("Linux test01");
            ProcessStartInfo psi = new ProcessStartInfo(@"c:\Windows\System32\OpenSSH\ssh.exe");
            psi.Arguments = "schvila@192.168.202.129 -pw SCh*1108";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            //psi.WindowStyle = ProcessWindowStyle.Hidden;
            using (var process = Process.Start(psi))
            {
                Console.WriteLine("Connecting...");

                //process.StandardInput.WriteLine("SSH schvila@192.168.202.129");
                // Read stderr synchronously (on another thread)
                string errorText = null;
                var stderrThread = new Thread(() => { errorText = process.StandardError.ReadToEnd(); });
                stderrThread.Start();
                //process.BeginOutputReadLine();
                //string s1, s2 = String.Empty;

                while (true)
                {
                    //s1 = process.StandardOutput.ReadToEnd();
                    //Console.WriteLine(t);
                    Console.WriteLine("Enter command:");
                    string cmd = Console.ReadLine();
                    if (string.IsNullOrEmpty(cmd))
                        break;
                    process.StandardInput.WriteLine(cmd);
                    process.StandardInput.WriteLine("");
                    //s2 = process.StandardOutput.ReadToEnd();
                }
                //Console.WriteLine($"{s1}-{s2}");
                process.WaitForExit();
            }



        }


        static void PuTTYConnect()
        {
            Console.WriteLine("PuTTY test01");
            int sleep = 1000;
            ProcessStartInfo psi = new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"));
            //psi.Arguments = "schvila@192.168.202.129 -pw SCh*1108";
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = false;
            //psi.WindowStyle = ProcessWindowStyle.Hidden;
            using (var process = Process.Start(psi))
            {
                Console.WriteLine("Connecting...");
                var processIoMgr = new ProcessIO(process);
                string cmdForConnect = @"c:\_putytool\plink.exe  schvila@192.168.202.128 -pw SCh*1108";
                StringBuilder sb = new StringBuilder();
                processIoMgr.StdoutTextRead += (txt) =>
                {
                    var scReplace = txt.Replace("\r", "");
                    sb.Append(scReplace);
                    //Console.WriteLine($"{scReplace}");
                };
                processIoMgr.StartProcessOutputRead();
                processIoMgr.WriteStdin(cmdForConnect);

                Thread.Sleep(sleep);
                while (true)
                {
                    if (sb.Length > 0)
                    {
                        Console.WriteLine(sb.ToString());
                        sb.Clear();
                    }
                    Console.WriteLine($"{Environment.NewLine}Enter command:");
                    string cmd = Console.ReadLine();
                    if (string.IsNullOrEmpty(cmd))
                        break;
                    processIoMgr.WriteStdin(cmd);
                    Thread.Sleep(sleep);
                }
            }
        }
#endregion obsolette
    }
}
