using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataLogTester
{
    public partial class VerInfoForm : Form
    {
        public VerInfoForm()
        {
            InitializeComponent();
        }

        private void VerInfoForm_Load(object sender, EventArgs e)
        {
            labelInformation.Text = "バージョン" + Constants.CheckerSoftVer;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    

    
    
    
    }
}
