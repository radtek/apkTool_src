using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace APK_Tool
{
    /// <summary>
    /// 此类用于实现，对XML的操作（包括XML文件中节点和属性的读取、修改，以及同名XML文件相同节点属性的混合）
    /// </summary>
    class XML
    {
        /// <summary>
        /// 载入XML文件
        /// </summary>
        public static void Load(string file)
        {
            // 载入xml文件
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);

            // 获取所有子节点
            XmlNodeList xmlNodes = xmlDoc.ChildNodes;

            // 第一个子节点为XML文件的属性声明 <?xml version="1.0" encoding="utf-8" standalone="no"?>
            XmlDeclaration xmlDeclaration = xmlNodes[0] as XmlDeclaration; 
            // OuterXml = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"no\"?>"  Name = "xml" Value = "version=\"1.0\" encoding=\"utf-8\" standalone=\"no\""
            

            // 解析其余子节点信息
            for (int i = 1; i < xmlNodes.Count; i++)
            {
                XmlNodeAnalyse(xmlNodes[i]);
            }
        }

        /// <summary>
        /// XmlNode的所有节点名和属性值解析
        /// </summary>
        public static void XmlNodeAnalyse(XmlNode xmlNode)
        {
            string NodeName = xmlNode.Name;         // 节点名称
            string NodeValue = xmlNode.Value;       // 节点值
            string innerText = xmlNode.InnerText;   // 节点内容（为节点的属性串）

            // 所有节点的属性
            //XmlAttributeCollection attrs = xmlNode.Attributes;
            foreach(XmlAttribute attr in xmlNode.Attributes)
            {
                string name = attr.Name;
                string value = attr.Value;
            }

            // 获取所有子节点
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                XmlNodeAnalyse(child);
            }
        }

        /// <summary>
        /// xml文件合并，功能测试
        /// </summary>
        public static void test(string fileSource, string fileTarget)
        {
            // 载入xml文件
            XmlDocument Source = new XmlDocument();
            XmlDocument Target = new XmlDocument();
            Source.Load(fileSource);
            Target.Load(fileTarget);

            Combine(Source, Target);
            Target.Save(fileTarget);
        }

        /// <summary>
        /// 实现两个XML文件的内容混合，
        /// 将xmlSource的节点属性混合到xmlTarget中
        /// 若xmlTarget没有该节点，则添加
        /// 已有，则替换或混合或忽略
        /// </summary>
        public static void Combine(XmlDocument Source, XmlDocument Target)
        {
            XmlNode_ChildListCombine(Source, Target);
        }

        /// <summary>
        /// 节点合并
        /// 
        /// 若为相同节点，
        /// 则将xmlNodeSource合并到xmlNodeSource中
        /// </summary>
        public static void XmlNodeCombine(XmlNode Source, XmlNode Target)
        {
            // 若为相同的节点，则合并
            if (isSameNode(Source, Target))
            {
                XmlNodeAttributeCombine(Source, Target);  // 添加source中的节点属性到Target中
                if (!ValueEquals(Source, Target))         // 节点值不相同
                {
                    // 当存在子节点时，继续进行相同子节点的合并
                    if (Source.ChildNodes.Count > 0 && Target.ChildNodes.Count > 0)
                    {
                        XmlNode_ChildListCombine(Source, Target);
                    }
                    // 否则修改Target节点的值
                    else
                    {
                        Target.Value = Source.Value;
                        // 输出： 修改节点值
                    }
                }
            }
        }

        /// <summary>
        /// 节点集合的合并
        /// 
        /// </summary>
        public static void XmlNode_ChildListCombine(XmlNode Source, XmlNode Target)
        {
            if (Source != null)
            {
                foreach (XmlNode node in Source.ChildNodes)
                {
                    XmlNode sameNode = getSameNode(Target.ChildNodes, node);   // 获取Target中与node相同的节点
                    if (sameNode != null)           // 找到相同的节点，则合并
                    {
                        XmlNodeCombine(node, sameNode);
                        // 输出： 合并子节点
                    }
                    else// 未找到，则添加到Target中
                    {
                        AddChild(Target, node);
                        //XmlElement element = Target.OwnerDocument.CreateElement(node.Name);
                        //element.InnerXml = node.InnerXml;
                        //Target.AppendChild(element);        
                        // 输出： 添加子节点
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个child副本，并添加到parent中
        /// </summary>
        public static void AddChild(XmlNode parent, XmlNode child)
        {
            XmlElement element = parent.OwnerDocument.CreateElement(child.Name);
            element.InnerXml = child.InnerXml;
            element.SetAttribute(child.Name, child.Value);

            parent.AppendChild(element);       
        }

        /// <summary>
        /// 节点属性合并
        /// 
        /// 将节点xmlNodeSource中的属性添加到xmlNodeTarget中
        /// </summary>
        public static void XmlNodeAttributeCombine(XmlNode Source, XmlNode Target)
        {
            if (Source is XmlDeclaration && Target is XmlDeclaration) Target.InnerText = Source.InnerText;
            else if (Source is XmlDeclaration || Target is XmlDeclaration) ;
            else
            {
                if (Source.Name.Equals(Target.Name))                // 节点名称相同，合并属性
                {
                    if (Source.Attributes != null)
                    {
                        // 遍历Node1的所有属性
                        foreach (XmlAttribute attrSource in Source.Attributes)
                        {
                            string value = getAttribute(Target, attrSource.Name);
                            if (value == null)                          // 节点xmlNode2中没有有该属性时，添加属性值
                            {
                                XmlAttribute attr = Target.OwnerDocument.CreateAttribute(attrSource.Name);
                                attr.InnerXml = attrSource.InnerXml;
                                Target.Attributes.Append(attr);
                                // 输出： 节点添加属性
                            }
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 判定两个节点是否具有相同的名称和属性值，
        /// 是则可以视为相同节点
        /// </summary>
        public static bool isSameNode(XmlNode node1, XmlNode node2)
        {
            if (!node1.Name.Equals(node2.Name)) return false;           // 节点名称不相同

            // 判断是否为XmlDeclaration
            if (node1 is XmlDeclaration && node2 is XmlDeclaration) return node1.InnerText.Equals(node2.InnerText);
            else if (node1 is XmlDeclaration || node2 is XmlDeclaration) return false;

            if (node1.InnerXml.Equals(node2.InnerXml)) return true;   // 节点属性值相同

            if (node1.Attributes != null)
            {
                // 遍历Node1的所有属性
                foreach (XmlAttribute attr1 in node1.Attributes)
                {
                    string value2 = getAttribute(node2, attr1.Name);
                    if (value2 != null && !value2.Equals(attr1.Value)) return false;      // 节点属性值不相同
                }
            }

            XmlDeclaration Elemnet = node2 as XmlDeclaration;

            return true;
        }

        /// <summary>
        /// 获取节点node的name属性值
        /// </summary>
        public static string getAttribute(XmlNode node, string name)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.Equals(name)) return attr.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// 在节点数组list寻找与中node,
        /// 名称和属性值相同的节点
        /// </summary>
        public static XmlNode getSameNode(XmlNodeList list, XmlNode node)
        {
            foreach (XmlNode iteam in list)
                if (isSameNode(iteam, node)) return iteam;

            return null;
        }

        /// <summary>
        /// 判断两个节点的值是否相同
        /// </summary>
        public static bool ValueEquals(XmlNode xmlNode1, XmlNode xmlNode2)
        {
            // 存在节点值，比较节点值
            if (xmlNode1.Value != null && xmlNode2.Value != null)
                return xmlNode1.Value.Equals(xmlNode2.Value);
            // 只存在子节点，比较子节点
            else if (xmlNode1.Value == null && xmlNode2.Value == null)
            {
                // 均存在子节点
                if (xmlNode1.ChildNodes != null && xmlNode2.ChildNodes != null)
                {
                    // 子节点数目不同，则不相同
                    if (xmlNode1.ChildNodes.Count != xmlNode2.ChildNodes.Count) return false;
                    else
                    {
                        // 比较所有子节点
                        foreach (XmlNode node in xmlNode1.ChildNodes)
                        {
                            // 获取相同节点
                            XmlNode sameNode = getSameNode(xmlNode2.ChildNodes, node);
                            if (sameNode == null) return false; // 未获取到，则不相同
                            else
                            {
                                // 获取到，但子节点的值不同，则不相同
                                if (!ValueEquals(sameNode, node)) return false;
                            }
                        }

                        // 子节点均相同，则相同
                        return true;
                    }
                }
                // 均不存在子节点
                else if (xmlNode1.ChildNodes == null && xmlNode2.ChildNodes == null) return true;
                else return false;
            }
            else return false;
        }




        //=======================================================  
        //XML文件的，数据读、写  
        //=======================================================  

        /// <summary>  
        /// 读取XML  
        /// </summary>  
        public void ReadXML(string file)
        {   //using System.Xml;  

            //载入XML文档  
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(file);


            XmlNode root = xmlDoc.SelectSingleNode("Map");//查找<bookstore>  
            XmlElement Node = (XmlElement)root;
            string str = Node.GetAttribute("CellHeight"); //CellWidth  

            MessageBox.Show("宽度值：" + str + "", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //root.SelectSingleNode()  
            XmlNode Node2 = root.SelectSingleNode("PhysicalLayer");        //查找子节点  
            XmlElement Elem = (XmlElement)Node2;
            MessageBox.Show("获取数据：" + Elem.InnerText, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        /// <summary>  
        /// 写入XML  
        /// </summary>  
        public static void writeXML(string file)
        {
            //载入XML文档  
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "no");
            //xmlDoc.Load(file);

            //修改XML  
            XmlNode root = xmlDoc.SelectSingleNode("bookstore");//查找<bookstore>  
            //XmlNode root = xmlDoc.CreateNode("bookstore");//查找<bookstore>  
            {
                XmlElement xe1 = xmlDoc.CreateElement("book");  //创建一个<book>节点  
                xe1.SetAttribute("genre", "李赞红");            //设置该节点genre属性  
                xe1.SetAttribute("ISBN", "2-3631-4");           //设置该节点ISBN属性  
                {
                    XmlElement xesub1 = xmlDoc.CreateElement("title");
                    xesub1.InnerText = "CS从入门到精通";        //设置文本节点  
                    xe1.AppendChild(xesub1);                    //添加到<book>节点中  

                    XmlElement xesub2 = xmlDoc.CreateElement("author");
                    xesub2.InnerText = "候捷";
                    xe1.AppendChild(xesub2);

                    XmlElement xesub3 = xmlDoc.CreateElement("price");
                    xesub3.InnerText = "58.3";
                    xe1.AppendChild(xesub3);
                }
                root.AppendChild(xe1);                          //添加到<bookstore>节点中  
            }

            //保存修改到XML  
            xmlDoc.Save(file);
            MessageBox.Show("写入XML完成", "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        /// <summary>  
        /// 获取XML文件，节点属性值,用法：  
        /// ReadXML_Attrribute("D:\\data.XML", "Map", "CellHeight");     根节点  
        /// ReadXML_Attrribute("D:\\data.XML", "Map/BuildItem", "name"); 子节点  
        /// </summary>  
        public string ReadXML_NodeAttrribute(string file, string nodePath, string attr)
        {   //using System.Xml;  

            string str = "";
            try
            {
                //载入XML文档  
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                XmlNode Node = xmlDoc.SelectSingleNode(nodePath);   //查找节点  
                XmlNode Node2 = xmlDoc.SelectNodes(nodePath)[2];
                Node = Node2;
                XmlElement Elemnet = (XmlElement)Node;              //转化为XML元素  

                str = Elemnet.GetAttribute(attr);                   //获取节点属性值  
            }
            catch (Exception e)
            {
                string tmp = "获取XML文件，节点属性值出错！ \n1、请确保文件\n“" + file + "”存在，且其格式无误\n2、请确保节点“" + nodePath + "”和节点属性“" + attr + "”在XML文件中确实存在";
                MessageBox.Show(tmp, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return str;
        }

        /// <summary>  
        /// 读取所有节点nodePath,的所有attr属性值  
        /// </summary>  
        public string[][] ReadXML_NodesAttrribute(string file, string nodePath, string[] attr)
        {   //using System.Xml;  

            string[][] str = null;
            try
            {
                //载入XML文档  
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                XmlNodeList Nodes = xmlDoc.SelectNodes(nodePath);       //获取所有nodePath的节点  
                str = new string[Nodes.Count][];

                for (int i = 0; i < Nodes.Count; i++)
                {
                    XmlElement Elemnet = (XmlElement)Nodes[i];          //转化为XML元素  
                    str[i] = new string[attr.Length];

                    for (int j = 0; j < attr.Length; j++)
                        str[i][j] = Elemnet.GetAttribute(attr[j]);      //获取节点属性值  
                }
            }
            catch (Exception e)
            {
                string tmp = "获取XML文件，节点属性值出错！ \n1、请确保文件\n“" + file + "”存在，且其格式无误\n2、请确保节点“" + nodePath + "”和节点属性“" + attr + "”在XML文件中确实存在";
                MessageBox.Show(tmp, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return str;
        }

        /// <summary>  
        /// 读取所有节点nodePath,的所有attr属性值  
        /// </summary>  
        public string[][] ReadXML_NodesAttrribute2(string fileText, string nodePath, string[] attr)
        {   //using System.Xml;  

            string[][] str = null;
            try
            {
                //载入XML文档  
                XmlDocument xmlDoc = new XmlDocument();
                //xmlDoc.Load(file);  
                xmlDoc.LoadXml(fileText);

                XmlNodeList Nodes = xmlDoc.SelectNodes(nodePath);       //获取所有nodePath的节点  
                str = new string[Nodes.Count][];

                for (int i = 0; i < Nodes.Count; i++)
                {
                    XmlElement Elemnet = (XmlElement)Nodes[i];          //转化为XML元素  
                    str[i] = new string[attr.Length];

                    for (int j = 0; j < attr.Length; j++)
                        str[i][j] = Elemnet.GetAttribute(attr[j]);      //获取节点属性值  
                }
            }
            catch (Exception e)
            {
                string tmp = "获取XML文件，节点属性值出错！ \n1、请确保XML文件\n" + "存在，且其格式无误\n2、请确保节点“" + nodePath + "”和节点属性“" + attr + "”在XML文件中确实存在";
                MessageBox.Show(tmp, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return str;
        }

        /// <summary>  
        ///获取XML文件，节点数据,用法  
        ///ReadXML_NodeText("D:\\data.XML", "Map");                  根节点  
        ///ReadXML_NodeText("D:\\data.XML", "Map/PhysicalLayer");    子节点  
        /// </summary>  
        public string ReadXML_NodeText(string file, string nodePath)
        {
            string str = "";
            try
            {
                //载入XML文档  
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);

                XmlNode Node = xmlDoc.SelectSingleNode(nodePath);   //查找节点  
                XmlElement Elemnet = (XmlElement)Node;              //转化为XML元素  

                str = Elemnet.InnerText;                     //获取节点数据  
            }
            catch (Exception e)
            {
                string tmp = "获取XML文件，节点数据出错！ \n1、请确保文件\n“" + file + "”存在，且其格式无误\n2、请确保节点“" + nodePath + "”在XML文件中确实存在";
                MessageBox.Show(tmp, "消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return str;
        }  
    }
}
