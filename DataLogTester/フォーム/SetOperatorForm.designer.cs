namespace DataLogTester
{
    partial class SetOperatorForm
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
            this.buttonOk = new System.Windows.Forms.Button();
            this.listBoxOperatorName = new System.Windows.Forms.ListBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxNewName = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonOk.Location = new System.Drawing.Point(234, 206);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(102, 38);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // listBoxOperatorName
            // 
            this.listBoxOperatorName.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBoxOperatorName.FormattingEnabled = true;
            this.listBoxOperatorName.ItemHeight = 28;
            this.listBoxOperatorName.Location = new System.Drawing.Point(8, 3);
            this.listBoxOperatorName.Name = "listBoxOperatorName";
            this.listBoxOperatorName.Size = new System.Drawing.Size(203, 172);
            this.listBoxOperatorName.TabIndex = 1;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonAdd.Location = new System.Drawing.Point(230, 17);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(66, 31);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.Text = "追加";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonDelete.Location = new System.Drawing.Point(230, 88);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(66, 32);
            this.buttonDelete.TabIndex = 3;
            this.buttonDelete.Text = "削除";
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCancel.Location = new System.Drawing.Point(363, 206);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(99, 38);
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textBoxNewName);
            this.panel1.Controls.Add(this.listBoxOperatorName);
            this.panel1.Controls.Add(this.buttonAdd);
            this.panel1.Controls.Add(this.buttonDelete);
            this.panel1.Location = new System.Drawing.Point(3, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(459, 188);
            this.panel1.TabIndex = 5;
            // 
            // textBoxNewName
            // 
            this.textBoxNewName.Font = new System.Drawing.Font("メイリオ", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBoxNewName.Location = new System.Drawing.Point(302, 12);
            this.textBoxNewName.Name = "textBoxNewName";
            this.textBoxNewName.Size = new System.Drawing.Size(146, 36);
            this.textBoxNewName.TabIndex = 4;
            // 
            // SetOperatorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 253);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Name = "SetOperatorForm";
            this.Text = "作業者一覧の変更";
            this.Load += new System.EventHandler(this.SetOperatorForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.ListBox listBoxOperatorName;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxNewName;
    }
}