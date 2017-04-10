using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Timers;
using System.Threading;

namespace DataLogTester
{
    //＜注意事項＞
    //①このクラスはＦＤＴをシンプルインターフェイスで立ち上げることを前提に作ってあります

    public static class FlashDevelopmentToolkit
    {
        //********************************************************************************************************
        // 外部プロセスのメイン・ウィンドウを起動するためのWin32 API
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, StringBuilder lParam);

        private const int WM_GETTEXT = 0x000D;

        // ShowWindowAsync関数のパラメータに渡す定義値
        private const int SW_RESTORE = 9;  // 画面を元の大きさに戻す
        
        //********************************************************************************************************
        
  
        //列挙型の宣言      
        public enum WriteMode
        {
            TestMode,   //検査ソフト書き込み
            ProductMode //製品ソフト書き込み
        }

        
        //静的メンバの宣言
        public static string WorkSpacePath;  //FDTのワークスペースファイルのありか   
        public static string TestFilePath;  //試験用ソフトのありか
        public static string ProductFilePath;  //製品ソフトのありか
        public static string CheckSum;  //製品ソフトのチェックサム（FDTのログ画面に表示される値）
        private static System.Timers.Timer Tm;

        
        //フラグ
        private static bool FlagTimer;
        public static bool FlagWrite { get; private set; }
        public static bool FlagSum { get; private set; }
        

        //コンストラクタ
        static FlashDevelopmentToolkit()
        {          
            //タイマー（ウィンドウハンドル取得用）の設定
            Tm = new System.Timers.Timer();
            Tm.Enabled = false;
            Tm.Interval = 5000;
            Tm.Elapsed += new ElapsedEventHandler(tm_Tick);
        }

        //イニシャライズ(外部からラムダ式を渡し、用途に応じてカスタマイズする)
        public static void InitFdt(Action method)
        {
            method();//用途に応じた処理
        }

        //タイマーイベントハンドラ
        private static void tm_Tick(object source, ElapsedEventArgs e)
        {
            Tm.Stop();
            FlagTimer = false;//タイムアウト
        
        }

        public static bool WriteFirmware(WriteMode mode)
        {
            //フラグの初期化
            FlagWrite = false;
            FlagSum = false;

            Process Fdt = new Process();

            try
            {

                //プロセスを作成してFDTを立ち上げる
                IntPtr hWnd = IntPtr.Zero;
                Fdt.StartInfo.FileName = WorkSpacePath;
                Fdt.Start();
                Fdt.WaitForInputIdle();//指定されたプロセスで未処理の入力が存在せず、ユーザーからの入力を待っている状態になるまで、またはタイムアウト時間が経過するまで待機します。

                //FDTのウィンドウハンドル取得
                FlagTimer = true;
                Tm.Start();
                while (hWnd == IntPtr.Zero)
                {
                    Application.DoEvents();
                    if (!FlagTimer) return false;
                    hWnd = FindWindow(null, "FDT Simple Interface   (Supported Version)");
                }

                IntPtr ログテキストハンドル = FindWindowEx(hWnd, IntPtr.Zero, "RICHEDIT", "");

                //FDTを最前面に表示してアクティブにする（センドキー送るため）
                SetForegroundWindow(hWnd);
                General.Wait(1000);

                SendKeys.SendWait("{TAB}");
                General.Wait(300);
                SendKeys.SendWait("{TAB}");
                General.Wait(300);

                //ファームウェアのファイルパスを入力
                switch (mode)
                {
                    case WriteMode.TestMode:
                        SendKeys.SendWait(TestFilePath);
                        General.Wait(400);
                        break;
                    case WriteMode.ProductMode:
                        SendKeys.SendWait(ProductFilePath);
                        General.Wait(400);
                        break;
                }
                SendKeys.SendWait("{ENTER}");
                General.Wait(400);
                SendKeys.SendWait("{TAB}");
                General.Wait(300);
                SendKeys.SendWait("{TAB}");
                General.Wait(300);
                SendKeys.SendWait("P");
                General.Wait(300);
                SendKeys.SendWait("5");
                General.Wait(300);
                SendKeys.SendWait("{ENTER}");
                General.Wait(2500);
                SendKeys.SendWait("{ENTER}");

                int MaxSize = 5000;
                string LogBuff = "";

                for (; ; )
                {
                    var sb = new StringBuilder(MaxSize);
                    General.Wait(1000);//インターバル1秒　※インターバル無しの場合FDTがこける
                    SendMessage(ログテキストハンドル, WM_GETTEXT, MaxSize - 1, sb);
                    LogBuff = sb.ToString();
                    if (LogBuff.IndexOf("Error") >= 0)
                    {
                        return false;
                    }
                    if (LogBuff.IndexOf("Disconnected") >= 0)
                    {
                        break;
                    }
                }
                if (LogBuff.IndexOf("書き込みが完了しました") < 0) return false;
                FlagWrite = true;

                if (mode == FlashDevelopmentToolkit.WriteMode.ProductMode)
                {
                    //チェックサムがあっているかの判定
                    if (LogBuff.IndexOf(CheckSum) < 0) return false;
                }
                FlagSum = true;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                General.Wait(1500);
                if (Fdt != null)
                {
                    Fdt.Kill();
                    Fdt.Close();
                    Fdt.Dispose();
                }
            }
        
        }



    
    
    
    }
}
