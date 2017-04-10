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
    public partial class SplashForm : Form
    {

        //Splashフォーム
        private static SplashForm _form = null;

        public SplashForm()
        {
            InitializeComponent();
           
            pictureBox1.Controls.Add(labelVer);//バージョン表示の背景を透明にするため
            labelVer.Text = "バージョン" + Constants.CheckerSoftVer;
        }

        /// <summary>
        /// Splashフォーム
        /// </summary>
        public static SplashForm Form
        {
            get { return _form; }
        }

        /// <summary>
        /// Splashフォームを表示する
        /// </summary>
        public static void ShowSplash()
        {
            if (_form == null)
            {
                //Application.IdleイベントハンドラでSplashフォームを閉じる
                Application.Idle += new EventHandler(Application_Idle);
                //Splashフォームを表示する
                _form = new SplashForm();
                _form.Show();
                _form.Refresh();
            }
        }

        //アプリケーションがアイドル状態になった時
        private static void Application_Idle(object sender, EventArgs e)
        {
            //Splashフォームがあるか調べる
            if (_form != null && _form.IsDisposed == false)
            {
                //Splashフォームを閉じる
                _form.Close();
            }
            _form = null;
            //Application.Idleイベントハンドラの削除
            Application.Idle -= new EventHandler(Application_Idle);
        }
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
