using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APK_Tool
{
    /// <summary>
    /// 乐堂计费配置文件 assets\ltsdk_res\config.txt
    /// 实现对config.txt文件的载入和修改逻辑
    /// </summary>
    class LT_config
    {
        // 配置信息
        public Dictionary<string, string> dic = new Dictionary<string, string>();
        public string configPath = "";

        /// <summary>
        /// path为config.txt文件的完整路径
        /// </summary>
        public void load(string path)
        {
            if (dic.Count > 0) dic.Clear();
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            configPath = path + "\\config.txt";
            if(File.Exists(configPath))
            {
                string data = FileProcess.fileToString(configPath);
                AddValues(data);
            }
        }

        /// <summary>
        /// path为config.txt文件的完整路径
        /// </summary>
        public void load_ConfigFile(string filePath)
        {
            if (dic.Count > 0) dic.Clear();
            configPath = filePath;

            if (!File.Exists(filePath)) return;
            else
            {
                string data = FileProcess.fileToString(configPath);
                AddValues(data);
            }
        }

        /// <summary>
        /// config.txt文件合并逻辑,将SourceConfig中的所有配置合并到TargetConfig中
        /// </summary>
        public static void Config_Combine(String SourceConfig, String TargetConfig, Cmd.Callback call = null, bool useDirMode = true)
        {
            if (useDirMode)
            {
                SourceConfig = SourceConfig + "\\assets\\ltsdk_res\\config.txt";
                TargetConfig = TargetConfig + "\\assets\\ltsdk_res\\config.txt";
            }

            LT_config config = new LT_config();
            config.load_ConfigFile(TargetConfig);   // 载入目标config配置信息

            string data = FileProcess.fileToString(SourceConfig);
            if (!data.Equals(""))                   // 载入修改的配置信息
            {
                config.AddValues(data);
            }

            // 确保目标路径存在
            String parentDir = TargetConfig.Substring(0, TargetConfig.LastIndexOf("\\config.txt"));
            ToolSetting.confirmDir(parentDir);

            config.save();                          // 保存配置信息

            if (call != null) call("【I】混合" + SourceConfig + "到\r\n" + TargetConfig + "中");
        }


        /// <summary>
        /// 将配置文件中的信息存入Dic中
        /// </summary>
        private void AddValues(string data)
        {
            data = data.Replace("\r\n", "\n");
            string[] lines = data.Split('\n');
            foreach (string line in lines)
            {
                string L = line.Trim();
                if (L.StartsWith("#") || L.Equals("")) continue;
                else
                {
                    if (L.Contains("="))
                    {
                        int index = L.IndexOf("=");
                        string key = L.Substring(0, index), value = L.Substring(index + 1);
                        SetValue(key, value);
                    }
                    else SetValue(L, "");
                }
            }
        }

        /// <summary>
        /// 添加或修改key，vlaue值
        /// </summary>
        public void SetValue(string key, string value)
        {
            if (!dic.Keys.Contains(key)) dic.Add(key, value);
            else dic[key] = value;
        }

        /// <summary>
        /// 添加配置信息到config.txt,
        /// 形如：ltsdk_debug=true
        /// </summary>
        public void AddValues(List<string> configData)
        {
            foreach (string data in configData)
            {
                AddValues(data);
            }
        }

        /// <summary>
        /// 保存dic中当前的所有配置信息到config.txt
        /// </summary>
        public void save()
        {
            string data = "";
            List<string> keys = dic.Keys.ToList<string>();
            foreach(string key in keys)
            {
                data += key + "=" + dic[key] + "\r\n";
            }

            FileProcess.SaveProcess(data, configPath);
        }

    }
}
