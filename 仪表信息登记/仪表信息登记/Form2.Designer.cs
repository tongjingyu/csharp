namespace BaseManage
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
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(50, 107);
            this.textBox8.Name = "textBox8";
            this.textBox8.PasswordChar = '*';
            this.textBox8.Size = new System.Drawing.Size(133, 21);
            this.textBox8.TabIndex = 16;
            this.textBox8.Text = "123456";
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(50, 56);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(133, 21);
            this.textBox7.TabIndex = 14;
            this.textBox7.Text = "TestUser";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(50, 151);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(133, 28);
            this.button10.TabIndex = 13;
            this.button10.Text = "登陆";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(228, 226);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.button10);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "请登陆";
            this.Load += new System.EventHandler(this.Form2_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Button button10;
    }
}