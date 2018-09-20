using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APK_Tool
{
    /// <summary>
    /// 自动打包参数设置
    /// </summary>
    class StartParam
    {
        public static Boolean AutoRun = false; // 根据传入的参数自动打包

        public static String GAMEID = "";      // 游戏id
        public static String CHANNELID = "";   // 渠道id
        public static String APK = "";         // 游戏裸包
        public static String SIGN = "";        // 签名名称
        public static String OUTDIR = "";      // apk包输出目录 
        public static String ICONDIR = "";     // ICON目录，存放GAME_ICON.png、GAME_LOGO.jpg、CHANNEL_ICON.png

        /// <summary>
        /// 解析启动参数信息
        /// </summary>
        public static void getParams(String[] args)
        {
            
            if (args != null && args.Length > 0)
            {
                foreach (String arg0 in args)
                {
                    String arg = arg0.Trim();
                    if (arg.StartsWith("GAMEID=")) GAMEID = arg.Substring("GAMEID=".Length);
                    else if (arg.StartsWith("CHANNELID=")) CHANNELID = arg.Substring("CHANNELID=".Length);
                    else if (arg.StartsWith("APK=")) APK = arg.Substring("APK=".Length);
                    else if (arg.StartsWith("SIGN=")) SIGN = arg.Substring("SIGN=".Length);
                    else if (arg.StartsWith("OUTDIR=")) OUTDIR = arg.Substring("OUTDIR=".Length);
                    else if (arg.StartsWith("ICONDIR=")) ICONDIR = arg.Substring("ICONDIR=".Length);
                }
            }

            AutoRun = (!GAMEID.Equals("") && !CHANNELID.Equals("") && !APK.Equals(""));
        }
    }
}
