namespace DataLogTester
{
    partial class FormWarning
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWarning));
            this.label警告 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBox警告 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox警告)).BeginInit();
            this.SuspendLayout();
            // 
            // label警告
            // 
            this.label警告.BackColor = System.Drawing.Color.OrangeRed;
            this.label警告.Font = new System.Drawing.Font("メイリオ", 33.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label警告.Location = new System.Drawing.Point(23, 23);
            this.label警告.Name = "label警告";
            this.label警告.Size = new System.Drawing.Size(621, 126);
            this.label警告.TabIndex = 0;
            this.label警告.Text = "　   警告\r\n      WARNING";
            this.label警告.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.pictureBox警告);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label警告);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(696, 320);
            this.panel1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Beige;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.label1.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(493, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 35);
            this.label1.TabIndex = 2;
            this.label1.Text = "OK";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("メイリオ", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(13, 171);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(657, 81);
            this.label2.TabIndex = 3;
            this.label2.Text = "試験合格品を再試験しないでください！\r\n\r\n";
            // 
            // pictureBox警告
            // 
            this.pictureBox警告.BackColor = System.Drawing.Color.OrangeRed;
            this.pictureBox警告.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox警告.Image")));
            this.pictureBox警告.Location = new System.Drawing.Point(20, 23);
            this.pictureBox警告.Name = "pictureBox警告";
            this.pictureBox警告.Size = new System.Drawing.Size(124, 126);
            this.pictureBox警告.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox警告.TabIndex = 4;
            this.pictureBox警告.TabStop = false;
            // 
            // FormWarning
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ClientSize = new System.Drawing.Size(722, 344);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormWarning";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormWarning";
            this.Load += new System.EventHandler(this.FormWarning_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox警告)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label警告;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox警告;
    }
}