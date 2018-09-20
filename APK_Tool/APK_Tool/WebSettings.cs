using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace APK_Tool
{
    // 示例：scimence( Name1(6JSO-F2CM-4LQJ-JN8P) )scimence
    // string url = "https://git.oschina.net/scimence/easyIcon/wikis/OnlineSerial";
    // 
    // string data = getWebData(url);
    // string str1 = getNodeData(data, "scimence", false);
    // string str2 = getNodeData(str1, "Name1", true);

    /// <summary>
    /// 此类用于获取，在网络文件中的配置信息
    /// </summary>
    class WebSettings
    {
        #region 网络数据的读取

        //从给定的网址中获取数据
        public static string getWebData(string url)
        {
            string data = "";
            Encoding encoding = System.Text.Encoding.Default;
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = encoding;
                data = client.DownloadString(url);
            }
            catch (Exception) { }

            if (data.Equals("")) data = GetUrltoHtml(url, encoding);

            return data;
        }

        // 使用WebRequest获取，参考：http://www.cnblogs.com/sufei/archive/2011/05/22/2053642.html
        public static string GetUrltoHtml(string Url, Encoding encoding)
        {
            try
            {
                System.Net.WebRequest wReq = System.Net.WebRequest.Create(Url);
                // Get the response instance.
                System.Net.WebResponse wResp = wReq.GetResponse();
                System.IO.Stream respStream = wResp.GetResponseStream();
                // Dim reader As StreamReader = New StreamReader(respStream)
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, encoding))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
            }

            return "";
        }

        #endregion

        
        // 从自定义格式的数据data中，获取nodeName对应的节点数据
        //p>scimence(&#x000A;NeedToRegister(false)NeedToRegister&#x000A;RegisterPrice(1)RegisterPrice&#x000A;)scimence</p>&#x000A;</div>
        // NeedToRegister(false)&#x000A;RegisterPrice(1)   finalNode的数据格式
        public static string getNodeData(string data, string nodeName, bool finalNode)
        {
            try
            {
                string S = nodeName + "(", E = ")" + (finalNode ? "" : nodeName);
                int indexS = data.IndexOf(S) + S.Length;
                int indexE = data.IndexOf(E, indexS);

                return data.Substring(indexS, indexE - indexS);
            }
            catch (Exception) { return data; }
        }
    }
}
