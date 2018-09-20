namespace APK_Tool
{
    partial class Form3
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelGame = new System.Windows.Forms.Panel();
            this.panelInnerL = new System.Windows.Forms.Panel();
            this.gameListBox = new System.Windows.Forms.CheckedListBox();
            this.checkAllGame = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panelChannel = new System.Windows.Forms.Panel();
            this.panelInnerR = new System.Windows.Forms.Panel();
            this.channelListBox = new System.Windows.Forms.CheckedListBox();
            this.checkAllChannel = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panelButton = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_sign = new System.Windows.Forms.ComboBox();
            this.Combine = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.游戏目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开计费包目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开缓存目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开输出目录ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.comboBox_selectGame = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panelGame.SuspendLayout();
            this.panelInnerL.SuspendLayout();
            this.panelChannel.SuspendLayout();
            this.panelInnerR.SuspendLayout();
            this.panelButton.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 2);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
            this.splitContainer1.Size = new System.Drawing.Size(707, 547);
            this.splitContainer1.SplitterDistance = 201;
            this.splitContainer1.TabIndex = 10;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelGame);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelChannel);
            this.splitContainer2.Size = new System.Drawing.Size(707, 201);
            this.splitContainer2.SplitterDistance = 347;
            this.splitContainer2.TabIndex = 0;
            // 
            // panelGame
            // 
            this.panelGame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelGame.Controls.Add(this.comboBox_selectGame);
            this.panelGame.Controls.Add(this.panelInnerL);
            this.panelGame.Controls.Add(this.checkAllGame);
            this.panelGame.Controls.Add(this.label1);
            this.panelGame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGame.Location = new System.Drawing.Point(0, 0);
            this.panelGame.Name = "panelGame";
            this.panelGame.Size = new System.Drawing.Size(347, 201);
            this.panelGame.TabIndex = 14;
            // 
            // panelInnerL
            // 
            this.panelInnerL.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInnerL.Controls.Add(this.gameListBox);
            this.panelInnerL.Location = new System.Drawing.Point(0, 29);
            this.panelInnerL.Name = "panelInnerL";
            this.panelInnerL.Size = new System.Drawing.Size(345, 190);
            this.panelInnerL.TabIndex = 7;
            // 
            // gameListBox
            // 
            this.gameListBox.AllowDrop = true;
            this.gameListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameListBox.CheckOnClick = true;
            this.gameListBox.FormattingEnabled = true;
            this.gameListBox.Location = new System.Drawing.Point(1, 1);
            this.gameListBox.Margin = new System.Windows.Forms.Padding(1);
            this.gameListBox.Name = "gameListBox";
            this.gameListBox.Size = new System.Drawing.Size(343, 164);
            this.gameListBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.gameListBox, "拖拽游戏裸包或目录至此，载入游戏包\r\n");
            this.gameListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.gameListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            // 
            // checkAllGame
            // 
            this.checkAllGame.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllGame.AutoSize = true;
            this.checkAllGame.Location = new System.Drawing.Point(294, 7);
            this.checkAllGame.Name = "checkAllGame";
            this.checkAllGame.Size = new System.Drawing.Size(48, 16);
            this.checkAllGame.TabIndex = 6;
            this.checkAllGame.Text = "全选";
            this.checkAllGame.UseVisualStyleBackColor = true;
            this.checkAllGame.CheckedChanged += new System.EventHandler(this.checkAllGame_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "游戏列表：";
            // 
            // panelChannel
            // 
            this.panelChannel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelChannel.Controls.Add(this.panelInnerR);
            this.panelChannel.Controls.Add(this.checkAllChannel);
            this.panelChannel.Controls.Add(this.label2);
            this.panelChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChannel.Location = new System.Drawing.Point(0, 0);
            this.panelChannel.Name = "panelChannel";
            this.panelChannel.Size = new System.Drawing.Size(356, 201);
            this.panelChannel.TabIndex = 12;
            // 
            // panelInnerR
            // 
            this.panelInnerR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInnerR.Controls.Add(this.channelListBox);
            this.panelInnerR.Location = new System.Drawing.Point(0, 28);
            this.panelInnerR.Name = "panelInnerR";
            this.panelInnerR.Size = new System.Drawing.Size(354, 190);
            this.panelInnerR.TabIndex = 8;
            // 
            // channelListBox
            // 
            this.channelListBox.AllowDrop = true;
            this.channelListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.channelListBox.CheckOnClick = true;
            this.channelListBox.FormattingEnabled = true;
            this.channelListBox.Location = new System.Drawing.Point(1, 1);
            this.channelListBox.Margin = new System.Windows.Forms.Padding(1);
            this.channelListBox.Name = "channelListBox";
            this.channelListBox.Size = new System.Drawing.Size(352, 164);
            this.channelListBox.TabIndex = 6;
            this.toolTip1.SetToolTip(this.channelListBox, "拖拽渠道计费包或目录至此，载入渠道包");
            this.channelListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.channelListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            // 
            // checkAllChannel
            // 
            this.checkAllChannel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkAllChannel.AutoSize = true;
            this.checkAllChannel.Location = new System.Drawing.Point(303, 6);
            this.checkAllChannel.Name = "checkAllChannel";
            this.checkAllChannel.Size = new System.Drawing.Size(48, 16);
            this.checkAllChannel.TabIndex = 6;
            this.checkAllChannel.Text = "全选";
            this.checkAllChannel.UseVisualStyleBackColor = true;
            this.checkAllChannel.CheckedChanged += new System.EventHandler(this.checkAllChannel_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "渠道列表：";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(707, 342);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // panelButton
            // 
            this.panelButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButton.Controls.Add(this.label3);
            this.panelButton.Controls.Add(this.comboBox_sign);
            this.panelButton.Controls.Add(this.Combine);
            this.panelButton.Location = new System.Drawing.Point(1, 551);
            this.panelButton.Name = "panelButton";
            this.panelButton.Size = new System.Drawing.Size(705, 28);
            this.panelButton.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "选择签名：";
            // 
            // comboBox_sign
            // 
            this.comboBox_sign.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_sign.FormattingEnabled = true;
            this.comboBox_sign.Location = new System.Drawing.Point(68, 3);
            this.comboBox_sign.Name = "comboBox_sign";
            this.comboBox_sign.Size = new System.Drawing.Size(73, 20);
            this.comboBox_sign.TabIndex = 11;
            // 
            // Combine
            // 
            this.Combine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Combine.Location = new System.Drawing.Point(609, 3);
            this.Combine.Name = "Combine";
            this.Combine.Size = new System.Drawing.Size(75, 21);
            this.Combine.TabIndex = 10;
            this.Combine.Text = "打包";
            this.Combine.UseVisualStyleBackColor = true;
            this.Combine.Click += new System.EventHandler(this.Combine_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.游戏目录ToolStripMenuItem,
            this.打开计费包目录ToolStripMenuItem,
            this.打开缓存目录ToolStripMenuItem,
            this.打开输出目录ToolStripMenuItem,
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 98);
            // 
            // 游戏目录ToolStripMenuItem
            // 
            this.游戏目录ToolStripMenuItem.Name = "游戏目录ToolStripMenuItem";
            this.游戏目录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.游戏目录ToolStripMenuItem.Text = "裸包目录";
            this.游戏目录ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // 打开计费包目录ToolStripMenuItem
            // 
            this.打开计费包目录ToolStripMenuItem.Name = "打开计费包目录ToolStripMenuItem";
            this.打开计费包目录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打开计费包目录ToolStripMenuItem.Text = "渠道目录";
            this.打开计费包目录ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // 打开缓存目录ToolStripMenuItem
            // 
            this.打开缓存目录ToolStripMenuItem.Name = "打开缓存目录ToolStripMenuItem";
            this.打开缓存目录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打开缓存目录ToolStripMenuItem.Text = "缓存目录";
            this.打开缓存目录ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // 打开输出目录ToolStripMenuItem
            // 
            this.打开输出目录ToolStripMenuItem.Name = "打开输出目录ToolStripMenuItem";
            this.打开输出目录ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.打开输出目录ToolStripMenuItem.Text = "输出目录";
            this.打开输出目录ToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(121, 6);
            // 
            // comboBox_selectGame
            // 
            this.comboBox_selectGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_selectGame.FormattingEnabled = true;
            this.comboBox_selectGame.Location = new System.Drawing.Point(68, 3);
            this.comboBox_selectGame.Name = "comboBox_selectGame";
            this.comboBox_selectGame.Size = new System.Drawing.Size(198, 20);
            this.comboBox_selectGame.TabIndex = 12;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 582);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.panelButton);
            this.Controls.Add(this.splitContainer1);
            this.Name = "Form3";
            this.Text = "网游打包工具";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panelGame.ResumeLayout(false);
            this.panelGame.PerformLayout();
            this.panelInnerL.ResumeLayout(false);
            this.panelChannel.ResumeLayout(false);
            this.panelChannel.PerformLayout();
            this.panelInnerR.ResumeLayout(false);
            this.panelButton.ResumeLayout(false);
            this.panelButton.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panelGame;
        private System.Windows.Forms.Panel panelInnerL;
        private System.Windows.Forms.CheckedListBox gameListBox;
        private System.Windows.Forms.CheckBox checkAllGame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelChannel;
        private System.Windows.Forms.Panel panelInnerR;
        private System.Windows.Forms.CheckedListBox channelListBox;
        private System.Windows.Forms.CheckBox checkAllChannel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_sign;
        private System.Windows.Forms.Button Combine;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 游戏目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开计费包目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开缓存目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 打开输出目录ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ComboBox comboBox_selectGame;
    }
}