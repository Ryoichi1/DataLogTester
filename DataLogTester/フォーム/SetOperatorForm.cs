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
    public partial class SetOperatorForm : Form
    {
        
        public SetOperatorForm()
        {
            InitializeComponent();
        }

        private void SetOperatorForm_Load(object sender, EventArgs e)
        {
            foreach (string name in State.Setting.作業者リスト)
            {
                listBoxOperatorName.Items.Add(name);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            buttonCancel.Enabled = false;
            this.Close();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            buttonOk.Enabled = false;
            buttonCancel.Enabled = false;

            //作業者一覧を更新する
            State.Setting.作業者リスト.Clear();
            foreach(string opeName in listBoxOperatorName.Items)
            {
                State.Setting.作業者リスト.Add(opeName);
            }

            
            this.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxNewName.Text == "")
            {
                return;
            }

            listBoxOperatorName.Items.Add(textBoxNewName.Text);
            textBoxNewName.Text = "";

        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBoxOperatorName.SelectedIndex < 0)
            {
                MessageBox.Show("削除する名前が選択されていません");
                return;
            }

            listBoxOperatorName.Items.RemoveAt(listBoxOperatorName.SelectedIndex);
        
        }


    }
}
