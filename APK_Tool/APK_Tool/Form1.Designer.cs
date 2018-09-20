namespace APK_Tool
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.file = new System.Windows.Forms.TextBox();
            this.unPack = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.comboBox_sign = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // file
            // 
            this.file.AllowDrop = true;
            this.file.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.file.Location = new System.Drawing.Point(12, 12);
            this.file.Name = "file";
            this.file.Size = new System.Drawing.Size(462, 21);
            this.file.TabIndex = 0;
            this.toolTip1.SetToolTip(this.file, "拖动apk至此，进行解包\r\n或拖动解包目录至此，进行打包");
            this.file.TextChanged += new System.EventHandler(this.file_TextChanged);
            this.file.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.file.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            // 
            // unPack
            // 
            this.unPack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.unPack.Location = new System.Drawing.Point(480, 10);
            this.unPack.Name = "unPack";
            this.unPack.Size = new System.Drawing.Size(75, 23);
            this.unPack.TabIndex = 1;
            this.unPack.Text = "解包";
            this.unPack.UseVisualStyleBackColor = true;
            this.unPack.Click += new System.EventHandler(this.unPack_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(12, 39);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(542, 290);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // comboBox_sign
            // 
            this.comboBox_sign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_sign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sign.FormattingEnabled = true;
            this.comboBox_sign.Location = new System.Drawing.Point(401, 12);
            this.comboBox_sign.Name = "comboBox_sign";
            this.comboBox_sign.Size = new System.Drawing.Size(73, 20);
            this.comboBox_sign.TabIndex = 6;
            this.comboBox_sign.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 340);
            this.Controls.Add(this.comboBox_sign);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.unPack);
            this.Controls.Add(this.file);
            this.Name = "Form1";
            this.Text = "apk 解包、打包";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox file;
        private System.Windows.Forms.Button unPack;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ComboBox comboBox_sign;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

