using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSManager
{
    /// <summary>
    /// schtasks for start/shutdown/restart system/xserver/xpanel
    /// </summary>
    public class CSSchtasks
    {
        public CSSchtasks()
        {

        }
        public void CreateTask(Models.ServerModel server, Models.SchtaskModel schtask)
        {
            string time = DateTime.Now.AddMinutes(-10).ToString("HH:mm");
            var isLocal = Commons.NetHelper.IsLocalIPAddress(server.IP);

            string cmd;

            if(isLocal)
                cmd = string.Format("schtasks /create /tn {0} /tr {1} /sc once /st {2}", schtask.TaskName, schtask.TaskPath, time);
            else
                cmd = string.Format("schtasks /create /s {0} /u {1} /p {2} /tn \"{3}\" /tr {4} /sc once /st {5}", server.IP, server.UserName, server.Password, schtask.TaskName, schtask.TaskPath, time);
            var result = CmdHelper.ExecuteCmd(cmd);
            if(result.Contains("成功")||result.ToLower().Contains("success"))
            {
                schtask.TaskStatus = Models.SchtaskStatus.Normal;
            }
            else
            {
                schtask.TaskStatus = Models.SchtaskStatus.Failed;
            }
        }
        public void Query(Models.ServerModel server,Models.SchtaskModel schtask)
        {
            var isLocal = Commons.NetHelper.IsLocalIPAddress(server.IP);
            string cmd;

            if (isLocal)
                cmd = string.Format("schtasks /query /tn {0}", schtask.TaskName);
            else
                cmd = string.Format("schtasks /query /s {0} /u {1} /p {2} /tn \"{3}\"", server.IP, server.UserName, server.Password, schtask.TaskName);

            var result = CmdHelper.ExecuteCmd(cmd).ToLower();
            if(result.Contains("任务名"))
            {
                schtask.TaskStatus = Models.SchtaskStatus.Exist;
            }
            else
            {
                schtask.TaskStatus = Models.SchtaskStatus.NotExist;
            }
        }
        public void Run(Models.ServerModel server,Models.SchtaskModel schtask)
        {
            var isLocal = Commons.NetHelper.IsLocalIPAddress(server.IP);
            string cmd;

            if(isLocal)
                cmd = string.Format("schtasks /run /tn {0}", schtask.TaskName);
            else
                cmd = string.Format("schtasks /run /s {0} /u {1} /p {2} /tn \"{}\"", server.IP, server.UserName, server.Password, schtask.TaskName);

            var result = CmdHelper.ExecuteCmd(cmd).ToLower();

            if (result.Contains("成功") || result.Contains("success"))
            {
                schtask.TaskStatus = Models.SchtaskStatus.Executed;
            }
            else
                schtask.TaskStatus = Models.SchtaskStatus.Failed;
        }
    }
}
