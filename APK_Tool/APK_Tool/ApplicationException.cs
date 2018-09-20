using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace APK_Tool
{
/// <summary>
/// 此类用于捕获Application异常信息
/// </summary>
class ApplicationException
{
    /// <summary>
    /// 定义委托接口处理函数，调用此类中的Main函数为应用添加异常信息捕获
    /// </summary>
    public delegate void ExceptionCall();

    public static void Run(ExceptionCall exCall)
    {
        try
        {
            // 设置当前线程为最高优先级执行
            //Thread.CurrentThread.Priority = ThreadPriority.Highest;

            //设置应用程序处理异常方式：ThreadException处理
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            if (exCall != null) exCall();
        }
        catch (Exception ex)
        {
            string str = GetExceptionMsg(ex, string.Empty);
            MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    ///////--------------------------------------------------------------------------

    ///// <summary>
    ///// 应用程序的主入口点。
    ///// </summary>
    //[STAThread]
    //static void Main()
    //{
    //    try
    //    {
    //        //设置应用程序处理异常方式：ThreadException处理
    //        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
    //        //处理UI线程异常
    //        Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
    //        //处理非UI线程异常
    //        AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

    //        #region 应用程序的主入口点
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);
    //        Application.Run(new Form1());
    //        #endregion
    //    }
    //    catch (Exception ex)
    //    {
    //        string str = GetExceptionMsg(ex, string.Empty);
    //        MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //    }
    //}


    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        string str = GetExceptionMsg(e.Exception, e.ToString());
        MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //bool ok = (MessageBox.Show(str, "系统错误，提交bug信息？", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK);
        //if (ok) sendBugToAuthor(str);

        Update.Updated();      // 捕获运行异常后，检测是否有版本更新
        //LogManager.WriteLog(str);
    }

    static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string str = GetExceptionMsg(e.ExceptionObject as Exception, e.ToString());
        MessageBox.Show(str, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

        //bool ok = (MessageBox.Show(str, "系统错误，提交bug信息？", MessageBoxButtons.OKCancel, MessageBoxIcon.Error) == DialogResult.OK);
        //if (ok) sendBugToAuthor(str);

        Update.Updated();      // 捕获运行异常后，检测是否有版本更新
        //LogManager.WriteLog(str);
    }

    /// <summary>
    /// 生成自定义异常消息
    /// </summary>
    /// <param name="ex">异常对象</param>
    /// <param name="backStr">备用异常消息：当ex为null时有效</param>
    /// <returns>异常字符串文本</returns>
    static string GetExceptionMsg(Exception ex, string backStr)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("****************************异常文本****************************");
        sb.AppendLine("【出现时间】：" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
        if (ex != null)
        {
            sb.AppendLine("【异常类型】：" + ex.GetType().Name);
            sb.AppendLine("【异常信息】：" + ex.Message);
            sb.AppendLine("【堆栈调用】：" + ex.StackTrace);
            sb.AppendLine("【异常方法】：" + ex.TargetSite);
        }
        else
        {
            sb.AppendLine("【未处理异常】：" + backStr);
        }
        sb.AppendLine("***************************************************************");


        Update.Updated();      // 捕获运行异常后，检测是否有版本更新

        return sb.ToString();
    }
}
}
