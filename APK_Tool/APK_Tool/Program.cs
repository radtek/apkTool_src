using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            //args = new String[] { "GAMEID=1007", "CHANNELID=0000001,0001381", @"APK=D:\sci\网游打包工具2\APK_Base\游戏裸包\1001\v2.0.1\ltsdk_Demo_1.apk", "SIGN=letang", @"OUTDIR=C:\Users\wangzhongyuan\Desktop\test\out", @"ICONDIR=C:\Users\wangzhongyuan\Desktop\test\Icon,C:\Users\wangzhongyuan\Desktop\test\drawable"};
            StartParam.getParams(args);         // 设置启动参数信息

            ApplicationException.Run(call);     // 调用异常信息捕获类，进行异常信息的捕获
        }

        // 应用程序，入口逻辑
        public static void call()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (Update.Updated())
            {
                DependentFiles.checksAll();     // 检测工具运行依赖文件
                ToolSetting.Instance();         // 载入工具的配置信息

                //Application.Run(new Form2());
                Application.Run(new Form4());
            }
        }

        // 执行延时操作
        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }

        ///// <summary>
        ///// 获取与当前进程TaskManager同名的其它进程
        ///// </summary>
        //public static Process RunningInstance()
        //{
        //    Process current = Process.GetCurrentProcess();
        //    MessageBox.Show(current.ProcessName);
        //    Process[] processes1 = Process.GetProcessesByName(current.ProcessName); // "TaskManager.vshost"
        //    Process[] processes2 = Process.GetProcessesByName(current.ProcessName.Replace(".vshost", ""));
        //    //Process[] processes2 = Process.GetProcessesByName(current.ProcessName);

        //    List<Process> processes = new List<Process>();
        //    foreach (Process process in processes1) processes.Add(process);
        //    foreach (Process process in processes2) processes.Add(process);


        //    foreach (Process process in processes)
        //    {
        //        if (process.Id != current.Id)
        //        {
        //            string processName = System.IO.Path.GetFileName(process.MainModule.FileName).Replace(".vshost", "");
        //            string currentName = System.IO.Path.GetFileName(current.MainModule.FileName).Replace(".vshost", "");

        //            if (processName.Equals(currentName))
        //            {
        //                return process;
        //            }
        //        }
        //    }

        //    return null;
        //}

    }
}
