using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSManager
{
    /// <summary>
    /// executen cmd script
    /// </summary>
    public class CmdHelper
    {
        public static string ExecuteCmd (string args)
        {
            try
            {
                Process cmdProcess = new Process();
                cmdProcess.StartInfo.FileName = "cmd.exe";
                cmdProcess.StartInfo.CreateNoWindow = true;
                cmdProcess.StartInfo.RedirectStandardError = true;
                cmdProcess.StartInfo.RedirectStandardInput = true;
                cmdProcess.StartInfo.RedirectStandardOutput = true;
                cmdProcess.StartInfo.UseShellExecute = false;
                cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                //cmdProcess.StartInfo.Arguments = args;
                cmdProcess.Start();
                cmdProcess.StandardInput.WriteLine(args);
                cmdProcess.StandardInput.WriteLine("exit");

                string data = string.Empty;

                using (StreamReader reader = cmdProcess.StandardOutput)
                {
                    data = reader.ReadToEnd();

                    while(reader.Peek()>-1)
                    {
                        data += reader.ReadLine();
                    }
                }

                cmdProcess.WaitForExit();
                cmdProcess.Close();
                cmdProcess.Dispose();

                return data;

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
