using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace APK_Tool
{
    /// <summary>
    /// 属性, 形如： encoding="utf-8"
    /// </summary>
    public class attribute
    {
        public string name = "属性名";
        public string value= "属性值";

        public attribute(){}

        /// <summary>
        /// 创建属性
        /// </summary>
        public attribute(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// 获取属性,的字符串形式
        /// </summary>
        public string ToString()
        {
            return name + "=" + "\"" + value + "\"";
        }

        /// <summary>
        /// 从属性串创建属性， 形如：encoding="utf-8" 或 encoding=utf-8
        /// </summary>
        public static attribute Parse(string attrStr)
        {
            attrStr = attrStr.Trim();
            if (attrStr.Contains("="))
            {
                attribute iteam = new attribute();
                //string[] A = attrStr.Split('=');          // 按=分割属性名称和属性值
                string[] A = xmlNode.split(attrStr, "=");   // 仅分割为两个字符串
                iteam.name = A[0].Trim();
                iteam.value = A[1].Trim().Trim('"');

                return iteam;
            }
            return null;
        }
    }

    /// <summary>
    /// 属性List, 形如： version="1.0" encoding="utf-8" standalone="no" ...
    /// 包含若干个属性
    /// </summary>
    public class attributeList
    {
        public List<string> list = new List<string>();                             // 属性列表，字符串形式
        public Dictionary<string, string> dic = new Dictionary<string, string>();  // 属性字典，名称、值

        public attributeList() { }


        /// <summary>
        /// 向属性list中添加新的属性
        /// </summary>
        public void Add(attribute attr)
        {
            if (!dic.Keys.Contains(attr.name))
            {
                dic.Add(attr.name, attr.value);
                list.Add(attr.ToString());
            }
            else if (xmlNode.call != null)
            {
                string containsValue = dic[attr.name];  // 当前已有节点属性值
                if (!containsValue.Equals(attr.value)) xmlNode.call("【W】当前节点属性" + attr.name + "=" + containsValue + ",渠道中" + attr.name + "=" + attr.value);
            }
        }

        /// <summary>
        /// 向属性list中添加新的属性
        /// </summary>
        public void Add(string name, string value)
        {
            Add(new attribute(name, value));
        }

        /// <summary>
        /// 从属性list中移除指定的属性
        /// </summary>
        public void Remove(attribute attr)
        {
            if (dic.Keys.Contains(attr.name))
            {
                dic.Remove(attr.name);
                list.Remove(attr.ToString());
            }
        }

        /// <summary>
        /// 从属性list中移除指定的属性,name为属性名
        /// </summary>
        public void Remove(string name)
        {
            if (dic.Keys.Contains(name))
                Remove(new attribute(name, dic[name]));
        }

        /// <summary>
        /// 清空属性list
        /// </summary>
        public void Clear()
        {
            list.Clear();
            dic.Clear();
        }

        /// <summary>
        /// 判断属性list中中是否含有属性attr
        /// </summary>
        public bool Contains(attribute attr)
        {
            return list.Contains(attr.ToString());
        }

        /// <summary>
        /// 判断属性list中是否含有属性, name为属性名
        /// </summary>
        public bool Contains(string name)
        {
            return dic.Keys.Contains(name);
        }

        /// <summary>
        /// 判断属性list中是否含有属性值, value为属性值
        /// </summary>
        public bool ContainsValue(string value)
        {
            return dic.Values.Contains(value);

            //foreach (string V in dic.Values)
            //{
            //    if (V.Contains("=\"" + value + "\""))
            //        return true;
            //}
            //return false;
        }

        ///<summary>
        /// 获取属性list的第index个属性
        ///</summary>
        public attribute Get(int index)
        {
            if (index < list.Count) 
                return attribute.Parse(list[index]);
            return null;
        }

        ///<summary>
        /// 获取属性list中，属性名为name的属性值
        ///</summary>
        public string Get(string name)
        {
            if (dic.Keys.Contains(name))
                return dic[name];
            return null;
        }

        /// <summary>
        /// 重新设置属性list中，属性名为name的属性值
        /// 当value为ATTR_NULL时，仅清除节点属性
        /// </summary>
        public void Set(string name, string value)
        {
            Remove(name);           // 移除原有的属性

            if(!value.Equals("ATTR_NULL")) 
                Add(name, value);   // 重新添加新的属性
        }

        /// <summary>
        /// 重新设置属性list中，属性attr
        /// </summary>
        public void Set(attribute attr)
        {
            Remove(attr.name);            // 移除原有的属性
            Add(attr.name, attr.value);   // 重新添加新的属性
        }

        /// <summary>
        /// 重新设置属性List对应的所有属性
        /// </summary>
        public void Set(attributeList List)
        {
            foreach (String name in List.dic.Keys)
            {
                Set(name, List.dic[name]);
            }
        }


        /// <summary>
        /// 获取属性list中的元素数目
        /// </summary>
        public int Count()
        {
            return list.Count;
        }

        /// <summary>
        /// 获取属性list的字符串形式,
        /// 形如：version="1.0" encoding="utf-8" standalone="no"
        /// </summary>
        public string ToString()
        {
            string str = "";

            foreach (string attr in list)
                str += (str.Equals("") ? "" : " ") + attr;

            return str;
        }

        /// <summary>
        /// 获取属性list经排序后的 字符串形式
        /// 形如：version="1.0" encoding="utf-8" standalone="no"
        /// </summary>
        public string SortString()
        {
            attributeList attr = this.Clone();
            attr.list.Sort();       // 元素排序

            return attr.ToString();
        }

        /// <summary>
        /// 从属性串创建属性list， 
        /// 形如：version="1.0" encoding="utf-8" standalone="no"
        /// 或：version=1.0 encoding=utf-8 standalone=no
        /// </summary>
        public static attributeList Parse(string Str)
        {
            if (Str.Contains("="))
            {
                attributeList L = new attributeList();

                //Str = Str.Trim();
                //string[] A = Str.Split('\"');
                //for (int i = 0; i < A.Length-1; i += 2)
                //    L.Add(new attribute(A[i].Trim().Trim('='), A[i + 1]));

                Str = Str.Trim().Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");

                //while (Str.Contains("  ")) Str = Str.Replace("  ", " ");
                //string[] A = Str.Split(' ');
                string[] A = SplitAttrs(Str);

                for (int i = 0; i < A.Length; i ++)
                    L.Add(attribute.Parse(A[i]));

                return L;
            }
            else return new attributeList();
        }

        // 分割属性字符串，如：android:exported="false" android:label="Omlet Network Service" android:name="mobisocial.omlib.service.OmlibService"
        private static string[] SplitAttrs(String attrStr)
        {
            int S = 0, E = 0;
            List<String> list = new List<string>();

            String reminder = attrStr;

            while (reminder.Contains("\""))
            {
                S = reminder.IndexOf("\"");
                E = reminder.IndexOf("\"", S + 1);

                if(E > S)
                {
                    list.Add(reminder.Substring(0, E));
                    reminder = reminder.Substring(E+1).Trim();
                }
            }

            // 非标准形式的属性串，如：package=com.joym.limitkart.baidu 的解析
            reminder = reminder.Trim();
            if (!reminder.Equals(""))
            {
                while (reminder.Contains("  ")) reminder = reminder.Replace("  ", " ");
                string[] A = reminder.Split(' ');
                foreach (String str in A) list.Add(str);
            }

            return list.ToArray();
        }

        /// <summary>
        /// 复制当前attributeList
        /// </summary>
        public attributeList Clone()
        {
            attributeList L = new attributeList();
            foreach (string attr in list)
                L.Add(attribute.Parse(attr));
            return L;
        }

        /// <summary>
        /// 判断两个属性list,是否具有相同的属性；
        /// 是否完全相同根据ToString()后串进行判定
        /// </summary>
        public bool Equals(attributeList attributes)
        {
            if (this != null && attributes != null)
            {
                bool isMin1 = this.Count() <= attributes.Count();
                attributeList min = isMin1 ? this : attributes;
                attributeList max = isMin1 ? attributes : this;

                foreach (string attribute in min.list)
                    if (!max.list.Contains(attribute)) return false;    // 存在不相同的属性
            }

            return true; // 现有属性均相同
        }

        /// <summary>
        /// 创建一个attributeList合集，
        /// 先添加attributes1中的所有属性，再添加仅存在attributes2中的属性
        /// </summary>
        public static attributeList operator +(attributeList attributes1, attributeList attributes2)
        {
            attributeList L = attributes1.Clone();
            foreach (string attr in attributes2.list)
                L.Add(attribute.Parse(attr));
            return L;
        }


        /// <summary>
        /// 判断两个attributeList合集是否相同
        /// </summary>
        public static bool Equals(attributeList attributes1, attributeList attributes2)
        {
            string str1 = attributes1.SortString();
            string str2 = attributes2.SortString();
            return str1.Equals(str2);
        }
    }

    /// <summary>
    /// xml节点
    /// </summary>
    public class xmlNode
    {
        public string name = "节点名";
        public attributeList attributes = new attributeList();  // 节点属性
        public string value = "节点值";
        public List<xmlNode> childs = new List<xmlNode>();      // 子节点
        public bool isDeclarNode = false;                       // 是否为xml的信息声明节点 <?xml version="1.0" encoding="utf-8" standalone="no"?>

        public xmlNode parent;                                  // 父节点
        public Dictionary<string, xmlNode> dic = new Dictionary<string, xmlNode>();    // 子节点特征串, 到子节点的映射表
        public List<string> childNames = new List<string>();    // 存储子节点的节点名列表

        public string note = "";    // 节点的注释说明信息

        public static Cmd.Callback call = null;                  // 用于输出显示xml修改时的相关提示信息
        public static bool ParseCombine = false;                 // 控制载入xml文件时，是否进行同名节点的合并，默认不合并

        /// <summary>
        /// 创建xml节点
        /// </summary>
        public xmlNode() { }

        /// <summary>
        /// 创建xml节点
        /// </summary>
        public xmlNode(string name, attributeList attributes = null, string value = "", List<xmlNode> childs=null) 
        {
            this.name = name;
            if(attributes != null) this.attributes = attributes.Clone();
            this.value = value;
            this.Add(childs);
        }


        /// <summary>
        /// 定义节点的特征串，为名称和节点属性值组合
        /// </summary>
        public string Key()
        {
            string attrs = attributes.ToString();
            return name + (attrs.Equals("") ? "" : " ") + attrs;
        }

        /// <summary>
        /// 定义节点的特征串，为名称和节点属性值组合
        /// 如：<activity android:name="com.lt.sdk" android:screenOrientation="portrait"> 特征串为 activity:com.lt.sdk
        /// </summary>
        public string ShortKey()
        {
            string attrs = "";
            if(attributes.Contains("android:name")) attrs = attributes.Get("android:name");
            else if (attributes.Contains("package")) attrs = attributes.Get("package");
            else if (attributes.Count() > 0) attrs = attributes.Get(0).value;
            return name + (attrs.Equals("") ? "" : ":") + attrs;
        }
        

        /// <summary>
        /// 定义节点的特征串，为名称和节点属性值组合
        /// </summary>
        public string SortKey()
        {
            string attrs = attributes.SortString();
            return name + (attrs.Equals("") ? "" : " ") + attrs;
        }

        /// <summary>
        /// 获取当前节点在父节点中的路径信息
        /// </summary>
        public string Path()
        {
            if (parent != null) return parent.Path() + "/" + this.name;
            else return this.name;
        }

        /// <summary>
        /// 添加新的子节点到当前节点, 
        /// 当directAdd = true时，直接添加child为子节点，不进行同名节点的混合
        /// </summary>
        public void Add(xmlNode child, bool directAdd=false)
        {
            xmlNode node = null;
            if(!directAdd) node = child.getSameNode(this); // 在当前节点的子节点中查找与child相似的节点

            if (node == null)
            {
                //添加子节点
                childs.Add(child);
                child.parent = this;

                // 子节点相关信息记录
                if (!dic.Keys.Contains(child.SortKey()))
                    dic.Add(child.SortKey(), child);
                if (!childNames.Contains(child.name))
                    childNames.Add(child.name);
            }
            else
            {
                if (call != null)
                {
                    if (!child.name.Equals("xml") && !child.name.Equals("uses-permission") && !child.name.Equals("resources"))  // 这3种同名节点冲突不提示
                        call("【E】渠道包中存在与游戏包同名冲突的节点 <" + child.SortKey() + ">");
                }

                String tmp2 = "";
                if(child.SortKey().Contains("Platform.AppCompat.Light")) 
                    tmp2 = child.SortKey();

                // 将child节点的属性和其子节点，合并到查找到的相似的子节点中
                node.Combine(child);
            }
        }

        /// <summary>
        /// 添加list的所有节点到当前节点的子节点集合中
        /// 当directAdd = true时，直接添加child为子节点，不进行同名节点的混合
        /// </summary>
        public void Add(List<xmlNode> list, bool directAdd = false, bool combineMode = true)
        {
            if (list != null)
            {
                // 原有子节点的节点名列表
                xmlNode tmp = null;

                foreach (xmlNode child in list)
                {
                    if (combineMode)
                    {
                        if(tmp == null) tmp = this.Clone();                 // 创建一个当前节点的副本
                        directAdd = !tmp.childNames.Contains(child.name);   // 若原有子点中不含有当前添加的节点类型，则直接添加当前节点为子节点
                    }
                    this.Add(child.Clone(), directAdd);
                }
            }
        }

        /// <summary>
        /// 将节点node的属性和子节点合并到当前节点中
        /// </summary>
        public void Combine(xmlNode node)
        {
            // 添加node中的属性到当前节点中
            if (!attributeList.Equals(this.attributes, node.attributes))      // 添加的节点中的属性和当前节点属性不相同
            {
                if (call != null) if (!this.name.Equals("xml") && !this.name.Equals("uses-permission") && !this.name.Equals("resources"))  // 这3种同名节点冲突不提示
                        call("【W】节点属性合并：<" + this.ShortKey() + ">");// 显示当前节点特征串提示信息

                this.attributes = this.attributes + node.attributes;         // 进行节点属性合并
            }

            // 添加node的所有子节点到当前节点中
            this.Add(node.childs);
        }

        /// <summary>
        /// 将两List xmlNode 合并为一个list, 将listSource添加到listTarget上
        /// </summary>
        public static List<xmlNode> Combine(List<xmlNode> listTarget, List<xmlNode> listSource)
        {
            xmlNode N = new xmlNode();
            N.Add(listTarget);
            N.Add(listSource);

            // 清除childs的父节点信息
            foreach (xmlNode child in N.childs)
                child.parent = null;

            return N.childs;
        }


        /// <summary>
        /// 将两个Manifest.xml合并，将listSource添加到listTarget上
        /// 合并"manifest", "application"节点，保留所有activity等待后续操作
        /// </summary>
        public static List<xmlNode> CombineManifest(List<xmlNode> listTarget, List<xmlNode> listSource)
        {
            xmlNode N = new xmlNode();
            N.Add(listTarget);
            N.Add(listSource);

            string[] combinePath = { "manifest", "application"/*, "activity"*/ };
            xmlNode T = N;
            foreach (string name in combinePath)
            {
                // 合并所有manifest到第一个中
                List<xmlNode> list = T.GetChilds(name);
                if (name.Equals("activity"))            
                    list = getLancherActivity(list);    // 获取入口Activity

                for (int i = 1; i < list.Count; i++)
                {
                    if (name.Equals("activity"))        // 移除入口Activity中原有的intent-filter节点
                        list[0].Remove(list[0].Get("intent-filter"));
                        
                    list[0].Combine(list[i]);
                    T.Remove(list[i]);
                }

                T = list[0];
            }

            // 清除childs的父节点信息
            foreach (xmlNode child in N.childs)
                child.parent = null;

            return N.childs;
        }

        /// <summary>
        /// 获取list中，所有节点名为NodeName的节点
        /// 
        /// 如：activity 或 activity:com.game.main
        /// </summary>
        public static List<xmlNode> getNodes(List<xmlNode> list, String NodeName)
        {
            xmlNode N = new xmlNode();
            N.Add(list);
            List<xmlNode> list2 = N.GetChilds(NodeName);

            return list2;
        }

        /// <summary>
        /// 获取所有list中所有入口Activity
        /// </summary>
        public static List<xmlNode> getLancherActivity(List<xmlNode> list)
        {
            List<xmlNode> L = new List<xmlNode>();
            foreach (xmlNode N in list)
                if (N.isLauncherActivity()) L.Add(N);
            return L;
        }

        /// <summary>
        /// 判断当前节点是否是入口Activity
        /// </summary>
        public bool isLauncherActivity()
        {
            if (!this.name.Equals("activity") || !childNames.Contains("intent-filter")) return false;
            else
            {
                xmlNode main = this.Get("intent-filter");
                if (main != null && main.Contains("category android:name=\"android.intent.category.LAUNCHER\""))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 从当前节点中移除子节点
        /// </summary>
        public void Remove(xmlNode child)
        {
            if (childs == null) childs = new List<xmlNode>();
            if (Contains(child))
            {
                childs.Remove(child);
                child.parent = null;

                dic.Remove(child.SortKey());
            }
        }

        /// <summary>
        /// 判断当前节点中, 是否含有子节点child
        /// </summary>
        public bool Contains(xmlNode child)
        {
            return dic.Keys.Contains(child.SortKey());
        }

        /// <summary>
        /// 判断当前节点中, 是否含有特征串为key的子节点
        /// </summary>
        public bool Contains(string key)
        {
            // 解析Key()串
            string[] A = Parse_Key(key);            
            xmlNode Node = new xmlNode(A[0], attributeList.Parse(A[1]));

            return dic.Keys.Contains(Node.SortKey());
        }

        /// <summary>
        /// 根据子节点的特征串，获取对应的子节点
        /// </summary>
        public xmlNode Get(string key)
        {
            // 解析Key()串
            string[] A = Parse_Key(key);
            xmlNode Node = new xmlNode(A[0], attributeList.Parse(A[1]));
            key = Node.SortKey();

            if (Contains(key)) return dic[key];
            else return null;
        }

        /// <summary>
        /// 根据子节点名，获取节点名对应的所有子节点;
        /// 若节点名为空则获取到的为当前节点
        /// 如：activity 或 activity:com.game.main
        /// </summary>
        public List<xmlNode> GetChilds(string name)
        {
            string[] A = name.Split(':');
            List<xmlNode> L = new List<xmlNode>();
            if (name.Equals("") || name.Equals("THIS")) L.Add(this);
            else
            {
                if (childNames.Contains(A[0]))
                {
                    foreach (xmlNode child in childs)
                        if (child.name.Equals(A[0])) L.Add(child);
                }

                // 记录所有含有属性值：X的节点
                if (A.Length > 1 && !A[1].Equals(""))
                {
                    List<xmlNode> L2 = new List<xmlNode>();
                    foreach (xmlNode N in L)
                    {
                        if (N.attributes.ContainsValue(A[1]))
                        {
                            L2.Add(N);
                        }
                    }
                    L = L2;
                }
            }

            return L;
        }

        /// <summary>
        /// 根据子节点路径信息获取指定的子节点
        /// 如：获取指定activity下的指定intent-filter节点
        /// manifest/application/activity:com.game.main/intent-filter:lenovoid.MAIN
        /// 
        /// 或者创建 NEWNODE=<action android:name="android.intent.action.MAIN"/>形式的节点
        /// THIS或空串返回当前节点
        /// </summary>
        public xmlNode GetChild_ByPath(string path)
        {
            if (path.StartsWith("NEWNODE") && path.Contains("="))
            {
                String New = path.Substring(path.IndexOf("=") + 1).Trim().Trim('"');
                List<xmlNode> list = xmlNode.Parse(New);
                if (list.Count > 0) return list[0];
            }
            else if (path.Equals("THIS")) return this;

            xmlNode N = this;
            string[] A = path.Split('/');
            foreach (string P in A)
            {
                List<xmlNode> L = N.GetChilds(P);
                if (L.Count > 0) N = L[0];
                else return null;
            }

            return N;
        }

        /// <summary>
        /// 合并节点, sourcePath到targetPath的子节点；
        /// 添加source节点的属性和子节点到target节点，若target已有某条属性则不变，没有则添加
        /// 
        /// 或添加节点sourcePath到targetPath， targetPath需以"/CHILDS"结尾
        /// </summary>
        public void Add(string sourcePath, string targetPath)
        {
            xmlNode source = this.GetChild_ByPath(sourcePath);
            bool addchils = targetPath.EndsWith("/CHILDS");
            if(addchils)
            {
                targetPath = targetPath.Substring(0, targetPath.LastIndexOf("/CHILDS"));
            }

            xmlNode target = this.GetChild_ByPath(targetPath);
            if (source == null || target == null) return;

            // 添加source节点的属性和子节点到target节点
            if (addchils) target.childs.Add(source);
            else target.Combine(source);
        }

        /// <summary>
        /// 替换节点,
        /// 添加source节点的属性和子节点到target节点，若target已有某条属性则不变，没有则添加
        /// </summary>
        public void Replace(string targetPath, string sourcePath)
        {
            xmlNode source = this.GetChild_ByPath(sourcePath);
            xmlNode target = this.GetChild_ByPath(targetPath);
            if (source == null || target == null) return;

            // 替换节点
            xmlNode parent = target.parent;
            if (parent != null)
            {
                parent.Remove(target);
                parent.Add(source.Clone());
            }
        }

        /// <summary>
        /// 设置path对应节点的属性值为attributeStr或attributeListStr
        /// 形如：version="1.0" encoding="utf-8" standalone="no"
        /// 或：version=1.0 encoding=utf-8 standalone=no
        /// </summary>
        public void SetAttribute(string path, string attributeStr)
        {
            xmlNode N = this.GetChild_ByPath(path);
            attributeList attrs = attributeList.Parse(attributeStr);
            if (N != null && attrs != null) N.attributes.Set(attrs);
        }

        /// <summary>
        /// 设置path对应节点的节点值为value
        /// </summary>
        public void SetValue(string path, string value)
        {
            xmlNode N = this.GetChild_ByPath(path);
            if (N != null)
            {
                N.value = value;            // 设置节点值

                if (N.childs.Count > 0)
                {
                    N.childs.Clear();       // 清除子节点信息
                    N.dic.Clear();          // 子节点特征串, 到子节点的映射表
                    N.childNames.Clear();   // 存储子节点的节点名列表
                }
            }
        }

        /// <summary>
        /// 删除节点， nodePath对应的节点
        /// 或者删除nodePath的所有子节点，nodePath需以/CHILDS结尾
        /// </summary>
        public void Remove(string nodePath)
        {
            bool removeChils = nodePath.EndsWith("/CHILDS");
            if (nodePath.EndsWith("/CHILDS")) 
                nodePath = nodePath.Substring(0, nodePath.LastIndexOf("/CHILDS"));

            xmlNode node = this.GetChild_ByPath(nodePath);
            if (node != null)
            {
                if (removeChils) node.childs.Clear();
                else
                {
                    if (node.parent != null) node.parent.Remove(node);
                }
            }
        }


        /// <summary>
        /// 判断当前节点和node，是否为具有相同名称 和 属性list
        /// </summary>
        public bool Equals(xmlNode node)
        {
            if (this == null || node == null) return false;
            if (!this.name.Equals(node.name)) return false;       // 名称不相同

            // 节点主键名称
            String[] keyAttrs = { "name", "android:name", "android:id", "android:color" };
            foreach (string keyAttr in keyAttrs)
            {
                // 名称相同，则视作同一个节点
                if (this.attributes.Contains(keyAttr) && node.attributes.Contains(keyAttr))
                {
                    String ThisName = this.attributes.Get(keyAttr);
                    String NodeName = node.attributes.Get(keyAttr);

                    if (ThisName.Equals(NodeName)) return true;
                }
            }

            if (!this.attributes.Equals(node.attributes)) return false; // 属性不相同
            return true;
        }

        /// <summary>
        /// 获取root的子节点中,与当前节点具有相同名称和属性的节点
        /// 重合的节点属性均相同，但Key()可能不同
        /// </summary>
        public xmlNode getSameNode(xmlNode root)
        {
            if (root != null)
            {
                if (this.attributes.Count() == 0 && this.childs.Count == 0 && !this.value.Equals("")) 
                    return null;    // 没有属性，没有子节点，仅有value值时，视作没有相同节点
                else
                {
                    if (root.dic.Keys.Contains(SortKey()))
                        return root.dic[SortKey()];

                    foreach (xmlNode child in root.childs)
                        if (child.Equals(this)) return child;
                }
            }

            return null;
        }

        /// <summary>
        /// 从当前节点复制出一个新的节点
        /// </summary>
        public xmlNode Clone()
        {
            //xmlNode N = new xmlNode();
            //N.name = this.name;
            //N.value = this.value;
            //N.attributes = this.attributes.Clone();
            //N.Add(this.childs, true);               // 复制节点时，直接进行子节点的添加，不执行同名节点混合逻辑
            //N.isDeclarNode = this.isDeclarNode;

            //N.note = this.note;

            //return N;

            string Str = this.ToString();
            return xmlNode.Parse(Str)[0];
        }

        /// <summary>
        /// 定义xmlNode合并逻辑，从node1、和node2创建一个新的节点
        /// </summary>
        public static xmlNode operator +(xmlNode node1, xmlNode node2)
        {
            xmlNode N = node1.Clone();
            N.Combine(node2);

            return N;
        }

        /// <summary>
        /// 获取当前xmlNode的层次信息，检索parent直到为null的次数
        /// </summary>
        public int Layer()
        {
            int i = 0;
            xmlNode N = this;
            while (N.parent != null)
            {
                N = N.parent;
                i++;
            }
            return i;
        }

        /// <summary>
        /// 获取当前节点的输出为字符串时，需要缩进的空格数
        /// </summary>
        private string Space()
        {
            int layer = Layer();
            string tabs = "";
            while (layer-- > 0) tabs += "    ";  // tabs += "\t";

            return tabs;
        }

        /// <summary>
        /// 定义xmlNode的字符串形式
        /// </summary>
        public string ToString()
        {
            // 是否为xml声明信息
            if (isDeclarNode) return "<?" + Key() + "?>";

            string space = Space();
            string str = space + "<" + Key();

            // 有子节点时的显示样式
            if (childs.Count > 0)
            {
                str += ">";
                foreach (xmlNode child in childs)
                    str += "\r\n" + child.ToString();
                str += "\r\n" + space + "</" + name + ">";
            }
            // 有值时的显示样式
            else if (!value.Equals(""))
            {
                str += ">" + value + "</" + name + ">";
            }
            // 其余显示样式
            else str += "/>";

            // 添加当前节点的注释信息
            if (!note.Equals("")) str = space + note + "\r\n" + str;

            return str;
        }

        /// <summary>
        /// 转化list为字符串形式
        /// </summary>
        public static string ToString(List<xmlNode> list)
        {
            string Str = "";

            bool haveDeclar = false;
            foreach (xmlNode node in list)
            {
                Str += (Str.Equals("") ? "" : "\r\n") + node.ToString();
                if (node.isDeclarNode == true) haveDeclar = true;
            }

            string declar = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n";
            Str = (!haveDeclar ? declar : "") + Str;

            return Str;
        }

        /// <summary>
        /// 从字符串形式的xml节点，构建xmlNode
        /// </summary>
        public static xmlNode ParseOne(string Str)
        {
            List<xmlNode> L = Parse(Str);
            if (L.Count > 0) return L[0];
            else return null;
        }

        /// <summary>
        /// 从字符串形式的xml节点，构建xmlNode
        /// </summary>
        public static List<xmlNode> Parse(string Str)
        {
            List<xmlNode> list = new List<xmlNode>();
            string reminds = "";

            string noteInfo = "";
            while (!(Str = Str.Trim()).Equals(""))
            {
                try
                {
                    int S = Str.IndexOf('<') + 1, E = Str.IndexOf('>', S);
                    String key = Str.Substring(S, E - S);
                    reminds = Str.Substring(E + 1);         // 记录剩余未解析的串

                    // 注释信息不解析
                    if (key.StartsWith("!--"))
                    {
                        E = Str.IndexOf("-->", S);
                        S = S - 1;

                        // 获取节点注释信息
                        noteInfo = Str.Substring(S, E - S + "-->".Length);

                        // 剔除注释信息
                        Str = Str.Substring(E + "-->".Length);
                        continue;
                    }

                    bool isDeclear = key.StartsWith("?");   // 判定是否为xml声明节点
                    if (isDeclear) key = key.Trim('?');

                    string[] A = Parse_Key(key);            // 解析Key()串

                    // 创建xmlNode对象
                    xmlNode Node = new xmlNode(A[0], attributeList.Parse(A[1]));
                    Node.isDeclarNode = isDeclear;

                    bool haveChildValue = (!isDeclear && !key.EndsWith("/"));  // 判断是否有子节点或值
                    if (haveChildValue)
                    {
                        //int E2 = Str.IndexOf("</" + A[0], E);
                        int E2 = getMatchIndex(Str, A[0]);      // 获取与A[0]匹配的/A[0]在Str中的索引位置
                        string ValueStr = Str.Substring(E + 1, E2- E - 1);

                        E2 = Str.IndexOf(">", E2 + 1);
                        reminds = Str.Substring(E2 + 1);        // 记录剩余未解析的串

                        // 含有子节点，进行子节点的解析
                        //if (ValueStr.StartsWith("\"") && ValueStr.EndsWith("\"")) Node.value = ValueStr;    // 以引号开头和结尾的为节点值
                        //else
                        {
                            String ValueStrTmp = ValueStr.Trim();
                            if (ValueStrTmp.StartsWith("<") && ValueStrTmp.EndsWith(">") && ValueStrTmp.Contains("/"))
                            {
                                List<xmlNode> childs = Parse(ValueStr);
                                Node.Add(childs, !ParseCombine, ParseCombine);     // ParseCombine=false时，直接添加子节点，不进行节点的合并。ParseCombine=true时，会进行同名子节点的合并
                            }
                            // 有节点值 
                            else Node.value = ValueStr;
                        }
                    }

                    // 为当前节点添加注释信息
                    if (!noteInfo.Equals(""))
                    {
                        Node.note = noteInfo;
                        noteInfo = "";
                    }

                    list.Add(Node);

                    Str = reminds;
                }
                catch (Exception ex) 
                {
                    DialogResult result = MessageBox.Show("解析" + Str + "时异常！", "是否查看？", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        String path = AppDomain.CurrentDomain.BaseDirectory + "xmlNode解析异常.txt";
                        if (File.Exists(path)) File.Delete(path);
                        FileProcess.SaveProcess(Str, path);
                        System.Diagnostics.Process.Start(path);
                    }
                    
                    break;
                }
            }

            return list;
        }

        //<LinearLayout>
        //    <RelativeLayout>ssss3</RelativeLayout>
        //    <LinearLayout >ssss1</LinearLayout>
        //    <LinearLayout>ssss2</LinearLayout>
        //    <LinearLayout><TextView/></LinearLayout>
        //</LinearLayout>

        // 获取与首个<key >匹配的</key >索引位置
        private static int getMatchIndex(string data, string key)
        {
            int index = -1;
            int pos = 0;
            string keyStr = "";
            int count = 0;

            do
            {
                keyStr = getKeyStr(data, key, pos);     // <key > 或 </key >对应串
                index = data.IndexOf(keyStr, pos);      // 获取索引位置
                if (index == -1) index = data.IndexOf('<' + key, pos);

                pos = index + keyStr.Length;            // 记录检索位置

                if (keyStr.StartsWith("</")) count--;   
                else if (keyStr.StartsWith("<") && !keyStr.EndsWith("/>")) count++; // 累计匹配值

                if (keyStr.Equals("")) return -1;

            }while (count!=0);

            string reminds = data.Substring(index, data.Length - index);

            return index;
        }

        // 从坐标S0处开始，获取<key > 或 </key >标识的串
        private static string getKeyStr(string data, string key, int S0)
        {
            string str = getNodeString(data, key, S0);          // 获取<key >节点串

            string str2 = getNodeString(data, "/" + key, S0);   // 获取</key >节点串
            if (!str2.Equals(""))
            {
                if (str.Equals("")) str = str2;
                else
                {
                    int index1 = data.IndexOf(str, S0);
                    if (index1 == -1) index1 = data.IndexOf('<' + key, S0);

                    int index2 = data.IndexOf(str2, S0);

                    if (index2 < index1) str = str2;            // 取<key >或</key >靠前的那个
                    if (index1 == -1) str = str2;
                }
            }
            return str;
        }

        // 从data中获取第一个以<key ***>标识的串
        private static string getNodeString(string data, string key, int S0)
        {
            if (data.Contains('<' + key) && data.Contains('>'))
            {
                int S = data.IndexOf('<' + key, S0);
                if (S != -1)
                {
                    int E = data.IndexOf('>', S + key.Length + 1);
                    if (E > S) return data.Substring(S, E - S + 1);
                }
            }
            return "";
        }

        // ssss1</LinearLayout><LinearLayout>ssss2</LinearLayout><LinearLayout>ssss3
        // 将形如这样的串转化为ssss1和<LinearLayout>ssss2</LinearLayout><LinearLayout>ssss3</LinearLayout>
        private static string[] getValueReminds(string data, string Key)
        {
            string[] tmp = {"", ""};
            if (data.Contains("</" + Key) && data.Contains(">"))
            {
                int index1 = data.IndexOf("</" + Key);
                int index2 = data.IndexOf(">", index1);
                string value = data.Substring(0, index1);       // 获取ssss1值部分

                string reminds = data.Substring(index2 + 1);
                if (reminds.Contains("<" + Key) && reminds.Contains(">"))
                {
                    tmp[0] = value;
                    tmp[1] = reminds + " </" + Key + ">";
                }
            }
            else tmp[0] = data;

            return tmp;
        }

        /// <summary>
        /// 解析Key()串, 形如：xml encoding="utf-8"
        /// </summary>
        private static string[] Parse_Key(string key)
        {
            string key0 = key.Trim().Trim('/');
            key = key0.Replace("\r", " ").Replace("\t", " ").Replace("\n", " ");
            int S = key.IndexOf(" ");
            if (S == -1) return new string[] { key0, "" };
            else
            {
                string name = key.Substring(0, S);
                string attrs = key.Substring(S + 1).Trim();

                return new string[] { name, attrs };
            }
        }

        /// <summary>
        /// 判断node1和node2，是否完全相同
        /// 具有相同的节点名、节点属性、节点属性、子节点
        /// </summary>
        //public static bool Equals(xmlNode node1, xmlNode node2)
        //{
        //    if (node1 != node2) return false;

        //}


        /// <summary>
        /// 将两个xml文件，按节点属性名自动合并为同一个xml
        /// </summary>
        public static List<xmlNode> Combine(string xmlSource, string xmlTarget, bool isManifest, Cmd.Callback call = null)
        {
            List<xmlNode> listTarget = xmlNode.Parse(xmlTarget);
            List<xmlNode> listSource = xmlNode.Parse(xmlSource);

            xmlNode.call = call;    // 在进行xml合并时，输出显示相关提示信息
            List<xmlNode> list = !isManifest ? xmlNode.Combine(listTarget, listSource) : xmlNode.CombineManifest(listTarget, listSource);
            xmlNode.call = null;

            return list;

            //string xml = xmlNode.ToString(list);
            //return xml;
        }


        /// <summary>
        /// 获取manifests中的包名信息, manifest_File为 AndroidManifest.xml文件的完整路径
        /// </summary>
        public static string getPackageName(string manifest_File)
        {
            string xmlTarget = FileProcess.fileToString(manifest_File);
            if (xmlTarget.Equals("")) return "";

            List<xmlNode> listTarget = xmlNode.Parse(xmlTarget);
            List<xmlNode> gameManifests = getNodes(listTarget, "manifest");     // 获取manifest节点
            String packageName = getPackageName(gameManifests);                 // 获取游戏包名信息
            return packageName;
        }

        // 获取manifests中的包名信息
        private static string getPackageName(List<xmlNode> manifests)
        {
            if(manifests.Count > 0 )
            {
                xmlNode manifest = manifests[0];
                attributeList attrs = manifest.attributes;
                if(attrs != null && attrs.Contains("package"))
                    return attrs.Get("package");
            }
            return "";
        }

        /// <summary>
        /// XML文件解析测试
        /// </summary>
        public static void ParseTest(string FileSource, string outFile)
        {
            string xmlSource = FileProcess.fileToString(FileSource);
            List<xmlNode> listSource = xmlNode.Parse(xmlSource);

            string xml = xmlNode.ToString(listSource);
            FileProcess.SaveProcess(xml, outFile);                          // 保存文件
        }

        /// <summary>
        /// 获取首个节点，非<?xml></xml>节点
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<xmlNode>  getFirstNode(List<xmlNode> list)
        {
            List<xmlNode> tmp = new List<xmlNode>();

            bool finish = false;
            foreach (xmlNode N in list)
            {
                if (finish) break;

                if (!N.isDeclarNode) finish = true;
                tmp.Add(N);
            }

            return tmp;
        }

        /// <summary>
        /// 从文件中载入xml文件, 先复制FileTarget，在添加FileSource，输出到outFile
        /// </summary>
        public static void Combine(string FileSource, string FileTarget, string outFile, bool isManifest, List<string> cmds, Cmd.Callback call=null)
        {
            string xmlTarget = FileProcess.fileToString(FileTarget);
            string xmlSource = FileProcess.fileToString(FileSource);

            //string xml = xmlNode.Combine(xml1, xml2, isManifest);
            //FileProcess.SaveProcess(xml, outFile);

            List<xmlNode> list = xmlNode.Combine(xmlSource, xmlTarget, isManifest, call);   // 执行Manifest简单混合
            //string xml = xmlNode.ToString(list);
            string xml = xmlNode.ToString(getFirstNode(list));

            FileProcess.SaveProcess(xml, outFile);                          // 保存文件

            //Manifest manifest = new Manifest(list, outFile);                // 创建Manifest对象

            ////FileProcess.SaveProcess(xmlNode.ToString(list), outFile.Replace(".xml", "_缓存.xml"));

            //// 执行manifest修改逻辑
            ////string[] cmd = { "remove GAME_MAIN_ACTIVITY/intent-filter", "add THIS_MAIN_ACTIVITY to GAME_MAIN_ACTIVITY" };   // 执行操作移除游戏入口Activity中的intent-filter，混合附加入口Activity
            ////string[] cmd = { "remove GAME_MAIN_ACTIVITY/intent-filter", "add activity:com.ltsdk_entryDemo.LogoActivity to GAME_MAIN_ACTIVITY", "remove activity:com.ltsdk_entryDemo.LogoActivity" };
            //if (isManifest)
            //{
            //    // 执行manifest处理逻辑
            //    manifest.runCMD(cmds);

            //    // 获取游戏图标名称，如： @drawable/icon
            //    Settings.gameIcon = manifest.icon;
            //}

            //// 保存manifest
            //manifest.save();
        }




        //// 自定义cmd处理命令逻辑

        /// <summary>
        /// 执行多条操作
        /// </summary>
        public void runCMD(List<string> cmds)
        {
            if (cmds != null && cmds.Count > 0) runCMD(cmds.ToArray());
        }

        /// <summary>
        /// 执行多条操作
        /// </summary>
        public void runCMD(string[] cmds)
        {
            foreach (string cmd in cmds)
            {
                try
                {
                    runCMD(cmd);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Path() + ".runCMD()报错：" + cmd + "\r\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 自定Manifest处理逻辑
        /// path (形如 manifest/application/activity:com.shjc.jsbc.Main)，也可定义path为创建新的节点，NEWNODE=<action android:name="android.intent.action.MAIN"/>
        /// add path1 to path2      （添加path1对应的节点信息到path2）
        /// replace path1 by path2  （）
        /// remove path             （删除path处的节点）
        /// get path set attr       （获取指定的节点，并修改其属性值attr）
        /// GAME_MAIN_ACTIVITY      （指代游戏入口Activity Path）
        /// THIS_MAIN_ACTIVITY      （指代当前配置计费包apk的Activity Path）
        /// </summary>
        public void runCMD(string cmd)
        {
            // 移除指定的节点
            if (cmd.StartsWith("remove "))
            {
                string path = cmd.Substring("remove ".Length);
                Remove(path);
            }
            else if (cmd.StartsWith("get "))
            {
                cmd = cmd.Substring("get ".Length).Trim();

                if (cmd.Contains(" set "))
                {
                    string[] A = split(cmd, " set ");
                    if (A.Length > 1) SetAttribute(A[0], A[1]);
                }
                else if (cmd.Contains(" setValue "))
                {
                    string[] A = split(cmd, " setValue ");
                    if (A.Length > 1) SetValue(A[0], A[1]);
                }
            }
            else if (cmd.StartsWith("add "))
            {
                cmd = cmd.Substring("add ".Length).Trim();

                string[] A = split(cmd, " to ");
                if (A.Length > 1) Add(A[0], A[1]);
            }
            else if (cmd.StartsWith("replace "))
            {
                cmd = cmd.Substring("replace ".Length).Trim();

                string[] A = split(cmd, " by ");
                if (A.Length > 1) Replace(A[0], A[1]);
            }
        }

        /// <summary>
        /// 将data按seprator分割为两个子串
        /// </summary>
        public static string[] split(string data, string seprator)
        {
            if (data.Contains(seprator))
            {
                int S = data.IndexOf(seprator), E = S + seprator.Length;
                string[] A = new string[2];
                A[0] = data.Substring(0, S).Trim();
                A[1] = data.Substring(E).Trim();

                return A;
            }
            else return new string[] { data };
        }
    }

    /// <summary>
    /// 定义Manifest用于修改Manifest.xml
    /// </summary>
    public class Manifest
    {
        public static Cmd.Callback call;

        // 基础数据
        string filePath = "";
        public List<xmlNode> list;

        // 拓展信息
        xmlNode manifest;
        xmlNode application;
        xmlNode LauncherActivity;       // 启动Activity
        public string applicationName;  // 应用包名
        public string icon;             // icon图标属性值    android:icon="@drawable/icon"
        public string label;            // label图标属性值   android:label="@string/app_name"

        List<xmlNode> permissions;
        List<xmlNode> activitys;
        List<xmlNode> services;
        List<xmlNode> meta_datas;
        List<xmlNode> receivers;

        string GAME_MAIN_ACTIVITY;  // 指代Manifest.xml第一个MAIN_ACTIVITY路径
        string THIS_MAIN_ACTIVITY;  // 指代Manifest.xml第二个MAIN_ACTIVITY路径
        string MANIFEST = " ";      // 指代manifest节点


        Dictionary<string, xmlNode> ActivityDic = new Dictionary<string, xmlNode>();

        public Manifest(string filePath)
        {
            this.filePath = filePath;
            string xml = FileProcess.fileToString(filePath);
            list = xmlNode.Parse(xml);

            InitLoad();
        }

        public Manifest(string xml, string filePath)
        {
            this.filePath = filePath;
            list = xmlNode.Parse(xml);

            InitLoad();
        }


        public Manifest(List<xmlNode> list, string filePath)
        {
            this.filePath = filePath;
            this.list = list;
            InitLoad();
        }

        /// <summary>
        /// 初始载入，预处理
        /// </summary>
        private void InitLoad()
        {
            if (!(list.Count > 1 || !list[1].name.Equals("manifest"))) return;
            manifest = list[1];

            List<xmlNode> L = manifest.GetChilds("application");
            if (L.Count == 0) return;
            application = L[0];

            permissions = manifest.GetChilds("uses-permission");

            if (application == null) return;
            activitys = application.GetChilds("activity");
            services = application.GetChilds("service");
            meta_datas = application.GetChilds("meta-data");
            receivers = application.GetChilds("receiver");

            if (application.attributes != null)
            {
                applicationName = application.attributes.Get("android:name");
                icon = application.attributes.Get("android:icon");
                label = application.attributes.Get("android:label");
            }


            // 获取activitys中所有入口Activity
            L = xmlNode.getLancherActivity(activitys);
            if (L.Count == 0) return;
            LauncherActivity = L[0];

            // 获取第一个入口Activity路径
            string name1 = L[0].attributes.Get("android:name");
            GAME_MAIN_ACTIVITY = "activity:" + name1;

            if (L.Count > 1)
            {
                // 获取第一个入口Activity路径
                string name2 = L[1].attributes.Get("android:name");
                THIS_MAIN_ACTIVITY = "activity:" + name2;
            }
            else THIS_MAIN_ACTIVITY = "";
        }

        /// <summary>
        /// 已Activity的名称为键值，构建映射表
        /// </summary>
        private void createDic(List<xmlNode> activitys)
        {
            //<activity android:name="com.shjc.jsbc.view2d.selectmap.SelectMap" android:screenOrientation="landscape"/>
            foreach (xmlNode N in activitys)
            {
                if (N.attributes != null)
                {
                    string activityName = N.attributes.Get("android:name");
                    if(!ActivityDic.Keys.Contains(activityName))
                        ActivityDic.Add(activityName, N);
                }
            }
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
        /// 设置应用包名
        /// </summary>
        public void setApplicationName(string name)
        {
            if (application != null || application.attributes != null)
            {
                application.attributes.Set("android:name", name);
            }
        }

        /// <summary>
        /// 获取对应名称的Activity节点, 如：com.shjc.jsbc.Main
        /// </summary>
        public xmlNode getActivity(string name)
        {
            if (ActivityDic.Keys.Contains(name))
                return ActivityDic[name];
            else return null;
        }

        /// <summary>
        /// 根节点manifest
        /// 获取path对应的节点
        /// 
        /// 如： 
        /// uses-permission:android.permission.SEND_SMS
        /// 
        /// application/activity:com.shjc.jsbc.Main 或
        /// activity:com.shjc.jsbc.Main
        /// service:com.lenovo.AppCheckService
        /// meta-data:lenovo.gamesdk.new
        /// receiver:com.lenovo.lsf.push.receiver.PushReceiver
        /// </summary>
        private string FullPath(string path)
        {
            if (path.Contains("GAME_MAIN_ACTIVITY")) path = path.Replace("GAME_MAIN_ACTIVITY", GAME_MAIN_ACTIVITY);    // 替换字段为游戏入口Activity路径
            if (path.Contains("THIS_MAIN_ACTIVITY")) path = path.Replace("THIS_MAIN_ACTIVITY", THIS_MAIN_ACTIVITY);    // 计费包入口Activity路径
            if (path.Contains("MANIFEST")) path = path.Replace("MANIFEST", MANIFEST);
            
            path = path.Trim();
            if (path.StartsWith("activity:") || path.StartsWith("service:") || path.StartsWith("meta-data:") || path.StartsWith("receiver:"))
                path = "application/" + path;

            return path.Trim();
        }

        /// <summary>
        /// 执行多条操作
        /// </summary>
        public void runCMD(List<string> cmds)
        {
            if(cmds != null && cmds.Count > 0) runCMD(cmds.ToArray());
        }

        /// <summary>
        /// 执行多条操作
        /// </summary>
        public void runCMD(string[] cmds)
        {
            //string cmd2 = "";
            foreach (string cmd in cmds)
            {
                try
                {
                    //if (cmd.Contains("get MANIFEST/application set android:label="))
                    //    cmd2 = cmd;
                    runCMD(cmd);
                }
                catch (Exception ex) 
                {
                    if (call != null && !cmd.Equals("")) call("【E】" + "Manifest.runCMD()报错：" + cmd + "\r\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// 自定Manifest处理逻辑
        /// path (形如 manifest/application/activity:com.shjc.jsbc.Main)，也可定义path为创建新的节点，NEWNODE=<action android:name="android.intent.action.MAIN"/>
        /// add path1 to path2      （添加path1对应的节点信息到path2）
        /// replace path1 by path2  （）
        /// remove path             （删除path处的节点）
        /// get path set attr       （获取指定的节点，并修改其属性值attr）
        /// GAME_MAIN_ACTIVITY      （指代游戏入口Activity Path）
        /// THIS_MAIN_ACTIVITY      （指代当前配置计费包apk的Activity Path）
        /// </summary>
        public void runCMD(string cmd)
        {
            if (call != null && !cmd.Equals("")) call("【I2】" + cmd);
            if (cmd.Contains("THIS_MAIN_ACTIVITY") && THIS_MAIN_ACTIVITY.Equals("")) return;
            if (THIS_MAIN_ACTIVITY != null && THIS_MAIN_ACTIVITY.Equals("") && cmd.Equals("remove GAME_MAIN_ACTIVITY/intent-filter")) return;     // 当渠道入口为空时，不移除游戏入口中的intent-filter

            // 移除指定的节点
            if(cmd.StartsWith("remove "))
            {
                string path = FullPath(cmd.Substring("remove ".Length));
                manifest.Remove(path);
            }
            else if(cmd.StartsWith("get "))
            {
                cmd = cmd.Substring("get ".Length).Trim();

                if (cmd.Contains(" set "))
                {
                    string[] A = split(cmd, " set ");
                    if (A.Length > 1) manifest.SetAttribute(FullPath(A[0]), A[1]);
                }
                else if (cmd.Contains(" setValue "))
                {
                    string[] A = split(cmd, " setValue ");
                    if (A.Length > 1) manifest.SetValue(FullPath(A[0]), A[1]);
                }
            }
            else if (cmd.StartsWith("add "))
            {
                cmd = cmd.Substring("add ".Length).Trim();
                
                string[] A = split(cmd, " to ");
                if (A.Length > 1) manifest.Add(FullPath(A[0]), FullPath(A[1]));
            }
            else if (cmd.StartsWith("replace "))
            {
                cmd = cmd.Substring("replace ".Length).Trim();

                string[] A = split(cmd, " by ");
                if(A.Length > 1) manifest.Replace(FullPath(A[0]), FullPath(A[1]));
            }
        }

        /// <summary>
        /// 将data按seprator分割为两个子串
        /// </summary>
        public static string[] split(string data, string seprator)
        {
            if (data.Contains(seprator))
            {
                int S = data.IndexOf(seprator), E = S + seprator.Length;
                string[] A = new string[2];
                A[0] = data.Substring(0, S).Trim();
                A[1] = data.Substring(E).Trim();

                return A;
            }
            else return new string[] { data };
        }
    }
}
