namespace 透明时钟演示
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.我的博客ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.银色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.黑色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.红色ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.花瓣ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.置顶ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.关闭ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.我的博客ToolStripMenuItem,
            this.银色ToolStripMenuItem,
            this.黑色ToolStripMenuItem,
            this.红色ToolStripMenuItem,
            this.花瓣ToolStripMenuItem,
            this.置顶ToolStripMenuItem,
            this.关闭ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 158);
            // 
            // 我的博客ToolStripMenuItem
            // 
            this.我的博客ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("我的博客ToolStripMenuItem.Image")));
            this.我的博客ToolStripMenuItem.Name = "我的博客ToolStripMenuItem";
            this.我的博客ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.我的博客ToolStripMenuItem.Text = "我的博客";
            this.我的博客ToolStripMenuItem.Click += new System.EventHandler(this.我的博客ToolStripMenuItem_Click);
            // 
            // 银色ToolStripMenuItem
            // 
            this.银色ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("银色ToolStripMenuItem.Image")));
            this.银色ToolStripMenuItem.Name = "银色ToolStripMenuItem";
            this.银色ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.银色ToolStripMenuItem.Text = "银色";
            this.银色ToolStripMenuItem.Click += new System.EventHandler(this.银色ToolStripMenuItem_Click);
            // 
            // 黑色ToolStripMenuItem
            // 
            this.黑色ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("黑色ToolStripMenuItem.Image")));
            this.黑色ToolStripMenuItem.Name = "黑色ToolStripMenuItem";
            this.黑色ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.黑色ToolStripMenuItem.Text = "黑色";
            this.黑色ToolStripMenuItem.Click += new System.EventHandler(this.黑色ToolStripMenuItem_Click);
            // 
            // 红色ToolStripMenuItem
            // 
            this.红色ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("红色ToolStripMenuItem.Image")));
            this.红色ToolStripMenuItem.Name = "红色ToolStripMenuItem";
            this.红色ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.红色ToolStripMenuItem.Text = "红色";
            this.红色ToolStripMenuItem.Click += new System.EventHandler(this.红色ToolStripMenuItem_Click);
            // 
            // 花瓣ToolStripMenuItem
            // 
            this.花瓣ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("花瓣ToolStripMenuItem.Image")));
            this.花瓣ToolStripMenuItem.Name = "花瓣ToolStripMenuItem";
            this.花瓣ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.花瓣ToolStripMenuItem.Text = "花瓣";
            this.花瓣ToolStripMenuItem.Click += new System.EventHandler(this.花瓣ToolStripMenuItem_Click);
            // 
            // 置顶ToolStripMenuItem
            // 
            this.置顶ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("置顶ToolStripMenuItem.Image")));
            this.置顶ToolStripMenuItem.Name = "置顶ToolStripMenuItem";
            this.置顶ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.置顶ToolStripMenuItem.Text = "置顶";
            this.置顶ToolStripMenuItem.Click += new System.EventHandler(this.置顶ToolStripMenuItem_Click);
            // 
            // 关闭ToolStripMenuItem
            // 
            this.关闭ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("关闭ToolStripMenuItem.Image")));
            this.关闭ToolStripMenuItem.Name = "关闭ToolStripMenuItem";
            this.关闭ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.关闭ToolStripMenuItem.Text = "关闭";
            this.关闭ToolStripMenuItem.Click += new System.EventHandler(this.关闭ToolStripMenuItem_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 5000;
            this.toolTip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.toolTip1.ForeColor = System.Drawing.Color.Blue;
            this.toolTip1.InitialDelay = 50;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipTitle = "您好！现在时间";
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 10;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(130, 130);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(111, 0);
            this.Name = "Form1";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp_1);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown_1);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove_1);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem 银色ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 黑色ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 红色ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 花瓣ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 关闭ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 置顶ToolStripMenuItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ToolStripMenuItem 我的博客ToolStripMenuItem;
    }
}

