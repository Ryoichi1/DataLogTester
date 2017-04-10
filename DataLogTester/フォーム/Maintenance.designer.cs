namespace DataLogTester
{
    partial class Maintenance
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
            this.buttonSolenoid1 = new System.Windows.Forms.Button();
            this.buttonSolenoid2 = new System.Windows.Forms.Button();
            this.buttonCameraProperty = new System.Windows.Forms.Button();
            this.buttonLedOn = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonRs422 = new System.Windows.Forms.Button();
            this.buttonRs232c = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonPower = new System.Windows.Forms.Button();
            this.buttonLight = new System.Windows.Forms.Button();
            this.buttonE1emulator = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonSolenoid1
            // 
            this.buttonSolenoid1.Location = new System.Drawing.Point(17, 18);
            this.buttonSolenoid1.Name = "buttonSolenoid1";
            this.buttonSolenoid1.Size = new System.Drawing.Size(81, 26);
            this.buttonSolenoid1.TabIndex = 0;
            this.buttonSolenoid1.Text = "S2押し";
            this.buttonSolenoid1.UseVisualStyleBackColor = true;
            this.buttonSolenoid1.Click += new System.EventHandler(this.buttonSolenoid1_Click);
            // 
            // buttonSolenoid2
            // 
            this.buttonSolenoid2.Location = new System.Drawing.Point(17, 60);
            this.buttonSolenoid2.Name = "buttonSolenoid2";
            this.buttonSolenoid2.Size = new System.Drawing.Size(81, 26);
            this.buttonSolenoid2.TabIndex = 1;
            this.buttonSolenoid2.Text = "合格スタンプ";
            this.buttonSolenoid2.UseVisualStyleBackColor = true;
            this.buttonSolenoid2.Click += new System.EventHandler(this.buttonSolenoid2_Click);
            // 
            // buttonCameraProperty
            // 
            this.buttonCameraProperty.Location = new System.Drawing.Point(16, 60);
            this.buttonCameraProperty.Name = "buttonCameraProperty";
            this.buttonCameraProperty.Size = new System.Drawing.Size(112, 26);
            this.buttonCameraProperty.TabIndex = 14;
            this.buttonCameraProperty.Text = "カメラプロパティ";
            this.buttonCameraProperty.UseVisualStyleBackColor = true;
            this.buttonCameraProperty.Click += new System.EventHandler(this.buttonCameraProperty_Click);
            // 
            // buttonLedOn
            // 
            this.buttonLedOn.Location = new System.Drawing.Point(16, 18);
            this.buttonLedOn.Name = "buttonLedOn";
            this.buttonLedOn.Size = new System.Drawing.Size(112, 26);
            this.buttonLedOn.TabIndex = 15;
            this.buttonLedOn.Text = "LEDを全点灯させる";
            this.buttonLedOn.UseVisualStyleBackColor = true;
            this.buttonLedOn.Click += new System.EventHandler(this.buttonLedOn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonRs232c);
            this.groupBox1.Controls.Add(this.buttonRs422);
            this.groupBox1.Location = new System.Drawing.Point(12, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(116, 105);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "通信系";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonSolenoid1);
            this.groupBox2.Controls.Add(this.buttonSolenoid2);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(116, 102);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ソレノイド";
            // 
            // buttonRs422
            // 
            this.buttonRs422.Location = new System.Drawing.Point(17, 62);
            this.buttonRs422.Name = "buttonRs422";
            this.buttonRs422.Size = new System.Drawing.Size(91, 31);
            this.buttonRs422.TabIndex = 14;
            this.buttonRs422.Text = "RS422 接続";
            this.buttonRs422.UseVisualStyleBackColor = true;
            this.buttonRs422.Click += new System.EventHandler(this.buttonRs422_Click);
            // 
            // buttonRs232c
            // 
            this.buttonRs232c.Location = new System.Drawing.Point(17, 18);
            this.buttonRs232c.Name = "buttonRs232c";
            this.buttonRs232c.Size = new System.Drawing.Size(91, 29);
            this.buttonRs232c.TabIndex = 15;
            this.buttonRs232c.Text = "RS232C 接続";
            this.buttonRs232c.UseVisualStyleBackColor = true;
            this.buttonRs232c.Click += new System.EventHandler(this.buttonRs232c_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonLedOn);
            this.groupBox3.Controls.Add(this.buttonCameraProperty);
            this.groupBox3.Location = new System.Drawing.Point(148, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(151, 102);
            this.groupBox3.TabIndex = 18;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "カメラ";
            // 
            // buttonPower
            // 
            this.buttonPower.Location = new System.Drawing.Point(148, 137);
            this.buttonPower.Name = "buttonPower";
            this.buttonPower.Size = new System.Drawing.Size(72, 30);
            this.buttonPower.TabIndex = 19;
            this.buttonPower.Text = "電源 ON";
            this.buttonPower.UseVisualStyleBackColor = true;
            this.buttonPower.Click += new System.EventHandler(this.buttonPower_Click);
            // 
            // buttonLight
            // 
            this.buttonLight.Location = new System.Drawing.Point(148, 173);
            this.buttonLight.Name = "buttonLight";
            this.buttonLight.Size = new System.Drawing.Size(72, 30);
            this.buttonLight.TabIndex = 20;
            this.buttonLight.Text = "照明 ON";
            this.buttonLight.UseVisualStyleBackColor = true;
            this.buttonLight.Click += new System.EventHandler(this.buttonLight_Click);
            // 
            // buttonE1emulator
            // 
            this.buttonE1emulator.Location = new System.Drawing.Point(148, 212);
            this.buttonE1emulator.Name = "buttonE1emulator";
            this.buttonE1emulator.Size = new System.Drawing.Size(119, 30);
            this.buttonE1emulator.TabIndex = 21;
            this.buttonE1emulator.Text = "E1エミュレータ 接続";
            this.buttonE1emulator.UseVisualStyleBackColor = true;
            this.buttonE1emulator.Click += new System.EventHandler(this.buttonE1emulator_Click);
            // 
            // Maintenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 254);
            this.Controls.Add(this.buttonE1emulator);
            this.Controls.Add(this.buttonLight);
            this.Controls.Add(this.buttonPower);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Maintenance";
            this.Text = "メンテナンス";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Maintenance_FormClosed);
            this.Load += new System.EventHandler(this.Maintenance_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonSolenoid1;
        private System.Windows.Forms.Button buttonSolenoid2;
        private System.Windows.Forms.Button buttonCameraProperty;
        private System.Windows.Forms.Button buttonLedOn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonRs232c;
        private System.Windows.Forms.Button buttonRs422;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonPower;
        private System.Windows.Forms.Button buttonLight;
        private System.Windows.Forms.Button buttonE1emulator;
    }
}