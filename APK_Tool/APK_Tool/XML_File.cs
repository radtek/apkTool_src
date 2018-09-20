using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APK_Tool
{
    /// <summary>
    /// 此类用于实现对xml文件中的数据、节点进行添加修改删除操作
    /// </summary>
    public class XML_File
    {
        // 基础数据
        string filePath = "";
        public List<xmlNode> list;

        // xml内容节点信息
        xmlNode contentNode;

        /// <summary>
        /// 从指定的xml文件构建XML_File对象
        /// </summary>
        public XML_File(string filePath)
        {
            this.filePath = filePath;
            string xml = FileProcess.fileToString(filePath);
            list = xmlNode.Parse(xml);

            if(list.Count > 1) contentNode = list[1];
        }

        /// <summary>
        /// 保存到文件
        /// </summary>
        public void save()
        {
            string xml = xmlNode.ToString(list);
            FileProcess.SaveProcess(xml, filePath);
        }

        /// <summary>
        /// 执行cmd参数命令，对当前xml文件进行修改
        /// </summary>
        /// <param name="cmd"></param>
        public void runCMD(string cmd)
        {
            contentNode.runCMD(cmd);
            save();
        }

        public void runCMD(List<string> cmds)
        {
            contentNode.runCMD(cmds);
            save();
        }

        public void runCMD(string[] cmds)
        {
            contentNode.runCMD(cmds);
            save();
        }

        /// <summary>
        /// 执行cmd修改命令，修改xmlPath对应的xml文件
        /// </summary>
        public static void modify(string xmlPath, string cmd, Cmd.Callback call=null)
        {
            if (System.IO.File.Exists(xmlPath))
            {
                XML_File xml = new XML_File(xmlPath);
                xml.runCMD(cmd);

                if (call != null) call("【I3】 " + "对文件" + xmlPath + "，执行修改逻辑" + cmd);
            }
        }
    }
}
