namespace APK_Tool
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.Combine = new System.Windows.Forms.Button();
            this.dirDest = new System.Windows.Forms.TextBox();
            this.comboBox_sign = new System.Windows.Forms.ComboBox();
            this.dirAdd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.Location = new System.Drawing.Point(13, 62);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(551, 296);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // Combine
            // 
            this.Combine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Combine.Location = new System.Drawing.Point(489, 35);
            this.Combine.Name = "Combine";
            this.Combine.Size = new System.Drawing.Size(75, 21);
            this.Combine.TabIndex = 7;
            this.Combine.Text = "混合";
            this.toolTip1.SetToolTip(this.Combine, "混合附加包中的文件到基础包中");
            this.Combine.UseVisualStyleBackColor = true;
            this.Combine.Click += new System.EventHandler(this.Combine_Click);
            // 
            // dirDest
            // 
            this.dirDest.AllowDrop = true;
            this.dirDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dirDest.Location = new System.Drawing.Point(70, 13);
            this.dirDest.Name = "dirDest";
            this.dirDest.Size = new System.Drawing.Size(413, 21);
            this.dirDest.TabIndex = 6;
            this.toolTip1.SetToolTip(this.dirDest, "使用游戏apk包 或 apk解包根目录作为基础包，\r\n可一次性载入多个apk包进行处理");
            this.dirDest.TextChanged += new System.EventHandler(this.apkDir_TextChanged);
            this.dirDest.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.dirDest.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            // 
            // comboBox_sign
            // 
            this.comboBox_sign.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox_sign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sign.FormattingEnabled = true;
            this.comboBox_sign.Location = new System.Drawing.Point(489, 13);
            this.comboBox_sign.Name = "comboBox_sign";
            this.comboBox_sign.Size = new System.Drawing.Size(73, 20);
            this.comboBox_sign.TabIndex = 9;
            // 
            // dirAdd
            // 
            this.dirAdd.AllowDrop = true;
            this.dirAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dirAdd.Location = new System.Drawing.Point(70, 36);
            this.dirAdd.Name = "dirAdd";
            this.dirAdd.Size = new System.Drawing.Size(413, 21);
            this.dirAdd.TabIndex = 10;
            this.toolTip1.SetToolTip(this.dirAdd, "使用渠道计费apk包 或 apk解包根目录作为附加包\r\n可一次性载入多个apk包进行处理");
            this.dirAdd.TextChanged += new System.EventHandler(this.apkDir_TextChanged);
            this.dirAdd.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.dirAdd.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "基础包：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "附加包：";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 362);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 23);
            this.button2.TabIndex = 15;
            this.button2.Text = "获取默认配置";
            this.toolTip1.SetToolTip(this.button2, "生成附加包的基本配置信息，配置信息控制拷贝计费包中的文件。\r\n配置信息决定哪些文件被复制、替换、或修改。");
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(119, 362);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "测试";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(576, 388);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dirAdd);
            this.Controls.Add(this.comboBox_sign);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.Combine);
            this.Controls.Add(this.dirDest);
            this.Name = "Form2";
            this.Text = "apk混合";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button Combine;
        private System.Windows.Forms.TextBox dirDest;
        private System.Windows.Forms.ComboBox comboBox_sign;
        private System.Windows.Forms.TextBox dirAdd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}