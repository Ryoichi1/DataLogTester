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
            foreach (string name in State.作業者リスト)
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
            
            //パラメータファイル　作業者一覧を更新する
            
            //リストボックスの項目数を取得
            //int cnt = listBoxOperatorName.Items.Count;

            //配列を作って作業者名をぶっこむ
            var name = new List<string>();

            foreach(string opeName in listBoxOperatorName.Items)
            {
                name.Add(opeName);
            }
            foreach (int i in Enumerable.Range(0, 10 - name.Count)) 
            {
                name.Add("予約");
            }

            //パラメータファイルの作業者名一覧を更新
            if (!SetOperatorName(name))
            {
                MessageBox.Show("パラメータファイルの更新に失敗しました");
                return;
            }
            
            this.Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxNewName.Text == "")
            {
                return;
            }

            if (listBoxOperatorName.Items.Count >= 10) //とりあえず作業者登録は10名までとする
            {
                MessageBox.Show("作業者登録は10名までです");
                textBoxNewName.Text = "";
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

        //**************************************************************************
        //作業者一覧の更新
        //引数：
        //戻値：
        //**************************************************************************
        public bool SetOperatorName(List<string> name)
        {
            OpenOffice calc = new OpenOffice();
            //parameterファイルを開く
            calc.OpenFile(Constants.ParameterFilePath);


            // sheetを取得           
            calc.SelectSheet("OperatorName");


            //作業者一覧の更新
            //行＝ROW 列＝COLUMN 

            int i = 0;//カウンタの初期化
            foreach(string opeName in name)
            {
                calc.cell = calc.sheet.getCellByPosition(1, 1 + i);
                calc.cell.setFormula(opeName);
                i++;
            }


            // Calcファイルを保存して閉じる
            if (!calc.SaveFile())
            {
                return false;
            }

            return true;
        }
    }
}
