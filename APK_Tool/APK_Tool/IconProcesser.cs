using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace APK_Tool
{
    /// <summary>
    /// 此类用于实现对Icon图标的操作。
    /// 1、自动为icon添加设置的角标图像
    /// 2、从一张指定的icon创建多种适配尺寸
    /// </summary>
    class IconProcesser
    {
        # region 文件检索功能函数

        /// <summary>
        /// 在目录dir和其子目录中寻找文件名为fileName的所有文件
        /// </summary>
        public static List<String> getPath(String dir, String fileName)
        {
            return Directory.Exists(dir) ? getPath(new DirectoryInfo(dir), fileName) : new List<string>();
        }

        /// <summary>
        /// 在目录dir和其子目录中寻找文件名为fileName的所有文件
        /// </summary>
        public static List<String> getPath(DirectoryInfo directoryInfo, String fileName)
        {
            List<String> Paths = new List<string>();

            //目录下所有文件
            FileInfo[] files = directoryInfo.GetFiles();

            //判定文件名称是否为fileName
            foreach (FileInfo file in files)
            {
                String Name = Path.GetFileNameWithoutExtension(file.Name);
                if (Name.Equals(fileName)) Paths.Add(file.FullName);    // 记录搜索到的文件路径
            }

            //最后复制目录
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                List<String> subPaths = getPath(dir, fileName);
                foreach (string subPath in subPaths)
                    Paths.Add(subPath);
            }

            return Paths;
        }

        // 选取list中的文件，优先选择.png文件
        private static String preferPngFile(List<String> list)
        {
            if (list.Count > 1)
            {
                foreach (String path in list)
                {
                    if (path.ToLower().EndsWith(".png")) return path;
                }
            }

            if (list.Count > 0) return list[0];
            else return "";
        }

        // 在resDir目录下，获取以drawableDirName对应的目录完整路径，如： drawable-hdpi 对应 drawable-hdpi-v4或drawable-hdpi-v7所在目录
        // 若无匹配的目录，则返回drawableDirName， drawable不进行拓展匹配
        private static List<String> getDrawablePath(String resDir, String drawableDirName)
        {
            List<String> list = new List<string>();

            if(Directory.Exists(resDir))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(resDir);
                foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
                {
                    if (dir.Name.Equals(drawableDirName) || (!drawableDirName.Equals("drawable") && dir.Name.StartsWith(drawableDirName + "-")))
                        list.Add(dir.FullName);
                }
            }
            if (list.Count == 0)
            {
                String path = resDir + "\\" + drawableDirName;
                list.Add(path);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            }

            return list;
        }

        # endregion

        # region 图像处理功能函数

        /// <summary>
        /// 按指定尺寸对图像pic进行非拉伸缩放
        /// </summary>
        public static Bitmap shrinkTo(Image pic, Size S, Boolean cutting)
        {
            //创建图像
            Bitmap tmp = new Bitmap(S.Width, S.Height);     //按指定大小创建位图

            //绘制
            Graphics g = Graphics.FromImage(tmp);           //从位图创建Graphics对象
            g.Clear(Color.FromArgb(0, 0, 0, 0));            //清空

            Boolean mode = (float)pic.Width / S.Width > (float)pic.Height / S.Height;   //zoom缩放
            if (cutting) mode = !mode;                      //裁切缩放

            //计算Zoom绘制区域             
            if (mode)
                S.Height = (int)((float)pic.Height * S.Width / pic.Width);
            else
                S.Width = (int)((float)pic.Width * S.Height / pic.Height);
            Point P = new Point((tmp.Width - S.Width) / 2, (tmp.Height - S.Height) / 2);

            g.DrawImage(pic, new Rectangle(P, S));

            return tmp;     //返回构建的新图像
        }



        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        private const Int32 CURSOR_SHOWING = 0x00000001;
        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public Int32 x;
            public Int32 y;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        /// <summary>
        /// 截取屏幕指定区域为Image，保存到路径savePath下，haveCursor是否包含鼠标
        /// </summary>
        public static Image getScreen(int x=0, int y=0, int width=-1, int height=-1, String savePath="", bool haveCursor = true)
        {
            if (width == -1) width = SystemInformation.VirtualScreen.Width;
            if (height == -1) height = SystemInformation.VirtualScreen.Height;

            Bitmap tmp = new Bitmap(width, height);                 //按指定大小创建位图
            Graphics g = Graphics.FromImage(tmp);                   //从位图创建Graphics对象
            g.CopyFromScreen(x, y, 0, 0, new Size(width, height));  //绘制

            // 绘制鼠标
            if (haveCursor)
            {
                CURSORINFO pci;
                pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                GetCursorInfo(out pci);
                System.Windows.Forms.Cursor cur = new System.Windows.Forms.Cursor(pci.hCursor);
                cur.Draw(g, new Rectangle(pci.ptScreenPos.x, pci.ptScreenPos.y, cur.Size.Width, cur.Size.Height));
            }

            if (!savePath.Equals("")) saveIcon(tmp, tmp.Size, savePath);     // 保存到指定的路径下

            return tmp;     //返回构建的新图像
        }


        //在图像pic上添加logo
        public static Bitmap SetLogo(Image pic, Image logo)
        {
            //创建图像
            Bitmap tmp = new Bitmap(pic.Width, pic.Height);//按指定大小创建位图

            //绘制
            Graphics g = Graphics.FromImage(tmp);          //从位图创建Graphics对象
            g.Clear(Color.FromArgb(0, 0, 0, 0));           //清空

            g.DrawImage(pic, 0, 0);         //从pic的给定区域进行绘制
            g.DrawImage(logo, 0, 0);        //为图像添加logo

            return tmp;                     //返回构建的新图像
        }


        //保存图像pic到文件fileName中，指定图像保存格式
        public static void SaveToFile(Image pic, string fileName, bool replace, ImageFormat format)    //ImageFormat.Jpeg
        {
            //若图像已存在，则删除
            if (System.IO.File.Exists(fileName) && replace)
                System.IO.File.Delete(fileName);

            //若不存在则创建
            if (!System.IO.File.Exists(fileName))
            {
                if (format == null) format = getFormat(fileName);   //根据拓展名获取图像的对应存储类型

                if (format == ImageFormat.MemoryBmp) pic.Save(fileName);
                else pic.Save(fileName, format);                    //按给定格式保存图像
            }
        }

        //根据文件拓展名，获取对应的存储类型
        public static ImageFormat getFormat(string filePath)
        {
            ImageFormat format = ImageFormat.MemoryBmp;
            String Ext = System.IO.Path.GetExtension(filePath).ToLower();

            if (Ext.Equals(".png")) format = ImageFormat.Png;
            else if (Ext.Equals(".jpg") || Ext.Equals(".jpeg")) format = ImageFormat.Jpeg;
            else if (Ext.Equals(".bmp")) format = ImageFormat.Bmp;
            else if (Ext.Equals(".gif")) format = ImageFormat.Gif;
            else if (Ext.Equals(".ico")) format = ImageFormat.Icon;
            else if (Ext.Equals(".emf")) format = ImageFormat.Emf;
            else if (Ext.Equals(".exif")) format = ImageFormat.Exif;
            else if (Ext.Equals(".tiff")) format = ImageFormat.Tiff;
            else if (Ext.Equals(".wmf")) format = ImageFormat.Wmf;
            else if (Ext.Equals(".memorybmp")) format = ImageFormat.MemoryBmp;

            return format;
        }

        # endregion


        /// <summary>
        /// 获取apk解压路径下，指定名称的图像，的完整路径
        /// </summary>
        public static String getDrawable(String unpackDir, String drawableName)
        {
            // 转换 @drawable/icon 或 icon.png 转化为icon
            drawableName = trimDrawable(drawableName); 

            // 优先选择drawable目录下的图像
            String drawableDir = unpackDir + "\\res\\drawable";
            List<String> Path = getPath(drawableDir, drawableName);
            if (Path.Count > 0) return preferPngFile(Path);

            // 若drawable目录下未找到对应名称的图像，则在res目录下寻找
            String resDir = unpackDir + "\\res";
            Path = getPath(resDir, drawableName);
            return preferPngFile(Path);
        }

        /// <summary>
        /// 从游戏icon和渠道icon角标，创建带角标的icon图像
        /// </summary>
        public static Image createNewIcon(String gameIcon, String channelIcon)
        {
            Image gamePic = Bitmap.FromFile(gameIcon);          // 获取游戏icon
            Image channelPic = Bitmap.FromFile(channelIcon);    // 获取渠道icon角标

            Image logo = shrinkTo(channelPic, gamePic.Size, false);  // 缩放角标为icon对应的尺寸
            Image newPic = SetLogo(gamePic, logo);              // 添加渠道角标到游戏icon上

            gamePic.Dispose();
            channelPic.Dispose();

            return newPic;
        }

        /// <summary>
        /// 缩放icon为指定的尺寸，并保存到路径PathName
        /// </summary>
        public static void saveIcon(Image icon, Size size, String PathName)
        {
            Image tmp = shrinkTo(icon, size, false);
            SaveToFile(tmp, PathName, true, null);
        }

        /// <summary>
        /// 保存Icon图像为安卓对应的各种适配尺寸
        /// </summary>
        public static void saveIconAllSize(String resDir, Image icon, String iconName, Cmd.Callback call)
        {
            // 安卓icon适配尺寸和存储目录
            int[] sizes = { 512, 192, 144, 96, 72, 48, 36 };
            String[] drawableDirNames = { "drawable", "drawable-xxxhdpi", "drawable-xxhdpi", "drawable-xhdpi", "drawable-hdpi", "drawable-mdpi", "drawable-ldpi" };

            for(int i=0; i< sizes.Length; i++)
            {
                String drawableDir = drawableDirNames[i];
                Size size = new Size(sizes[i], sizes[i]);
                List<String> list = getDrawablePath(resDir, drawableDir);
                foreach(String dir in list)
                {
                    String relative = ApkCombine.relativePath(dir, resDir);  // 获取相对路径名
                    if (call != null) call("【I2】 " + "游戏icon尺寸适配：( " + size.Width + "x" + size.Height + " )" + relative + "\\" + iconName);
                    saveIcon(icon, size, dir + "\\" + iconName);
                }
            }
        }

        /// <summary>
        /// 当SourceDrawable和TargetDrawable都存在时，使用使用Source替换Target
        /// 如：SourceDrawable = @drawable/icon 或 icon
        /// </summary>
        public static void ReplaceDrawable(String unpackDir, String TargetDrawable, String SourceDrawable, Cmd.Callback call)
        {
            if (TargetDrawable == null || TargetDrawable.Equals("") || SourceDrawable.Equals("") || TargetDrawable.Equals(SourceDrawable)) return;

            String TargetPath = getDrawable(unpackDir, TargetDrawable);     // 获取目标文件路径
            if (TargetPath.Equals("")) return;

            String SourcePath = getDrawable(unpackDir, SourceDrawable);     // 获取原文件路径
            if (SourcePath.Equals("")) return;
            if (TargetPath.Equals(SourcePath)) return;

            // 生成新的路径文件名
            String TargetPathName = TargetPath.Substring(0, TargetPath.LastIndexOf(".")) + SourcePath.Substring(SourcePath.LastIndexOf("."));

            // 删除目标文件
            File.Delete(TargetPath);

            // 使用Source文件替换Target
            File.Copy(SourcePath, TargetPathName, true);

            // 删除Source
            File.Delete(SourcePath);


            if (call != null)
            {
                String SoureRelative = ApkCombine.relativePath(SourcePath, unpackDir);
                String TargetRelative = ApkCombine.relativePath(TargetPath, unpackDir);

                call("【I2】 " + "替换" + TargetRelative + "为" + SoureRelative);
            }
        }

        /// <summary>
        /// 根据给定的drawable图像名GameIcon和ChannelIcon生成新的游戏Icon并自动适配各种安卓icon尺寸
        /// 如： @drawable/icon 或 icon
        /// </summary>
        public static void AutoConfigeIcon(String unpackDir, String drawable_Game, String drawable_Channel, Cmd.Callback call)
        {
            if (drawable_Game == null) return;

            drawable_Game = trimDrawable(drawable_Game);    // 获取drawable名称 如： @drawable/icon 或 icon.png 转化为icon

            if (!drawable_Game.Equals(""))
            {
                String drawable_Game_Path = getDrawable(unpackDir, drawable_Game);  // 获取drawable对应路径

                // 获取渠道角标对应的图像路径
                String drawable_Channel_Path = "";
                if (!drawable_Channel.Equals(""))
                {
                    drawable_Channel_Path = getDrawable(unpackDir, drawable_Channel);// 获取角标对应路径

                    if (drawable_Channel_Path.Equals("") && call != null)
                    {
                        call("【I2】 " + "未获取到渠道角标：" + drawable_Channel);
                    }
                }

                // 获取或生成新的icon
                Image gamePic = null;
                if (!drawable_Channel_Path.Equals(""))
                {
                    if (call != null) call("【I2】 " + "从游戏icon：" + drawable_Game_Path + "和渠道角标" + drawable_Channel_Path + "生成新的游戏icon");
                    gamePic = createNewIcon(drawable_Game_Path, drawable_Channel_Path);
                }
                else
                {
                    Image tmp = Bitmap.FromFile(drawable_Game_Path);
                    gamePic = shrinkTo(tmp, tmp.Size, false);
                    tmp.Dispose();
                }


                // 保存各种适配尺寸
                if (gamePic != null) saveIconAllSize(unpackDir + "\\res", gamePic, drawable_Game.Contains(".") ? drawable_Game : (drawable_Game + ".png"), call);
            }
        }

        // 对drawable串进行转换 如： @drawable/icon 或 icon.png 转化为icon
        private static string trimDrawable(String drawableStr)
        {
            if (drawableStr.Contains("/")) drawableStr = drawableStr.Substring(drawableStr.LastIndexOf("/")+1);
            if (drawableStr.Contains(".")) drawableStr = drawableStr.Substring(0, drawableStr.LastIndexOf("."));
            return drawableStr;
        }


    }
}
