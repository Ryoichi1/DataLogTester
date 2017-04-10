using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Threading;
using System.Threading.Tasks;

namespace DataLogTester
{

    public static  class R6441b 
    {
        //定数の宣言
        public enum ComNumber { COM1, COM2, COM3, COM4, COM5, COM6 }
        public enum ErrorCode { Normal, ResponseErr, OverRange, DataErr, TimeoutErr, MeasErr, InitErr, SendErr, Other }
        private enum Mode { DEFAULT, DCV, DCA, FREQ }


        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;
        public static string RecieveData;//R6441Bから受信した生データ
        private static Mode MeasMode;//現在の計測モード

        public static double VoltData { get; private set; }
        public static double AmpData { get; private set; }
        public static double FreqData { get; private set; }

        //静的コンストラクタ
        static R6441b()
        {
            port = new SerialPort();
        }


        public static bool InitR6441b(ComNumber num) //引数portnameには"COM1"、"COM2"・・・を渡す
        {
            try
            {
                if (!port.IsOpen)
                {
                    //R6441B用のシリアルポート設定
                    port.PortName = num.ToString();
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.DtrEnable = true;
                    port.NewLine = ("\r\n");
                    //ポートオープン
                    port.Open();
                }
                General.Wait(500);
                MeasMode = Mode.DEFAULT;
                return (SendData("Z,F1,R5,PR2") && ReadRecieveData(1000)); //Z：初期化 コマンドを送信して=>の返信があるかチェックする
            }
            catch
            {
                return false;
            }
        }

        //**************************************************************************
        //ターゲットにコマンドを送る
        //引数：なし
        //戻値：bool
        //**************************************************************************
        private static bool SendData(string cmd)
        {
            try
            {
                port.DiscardInBuffer();//COM受信バッファクリア
                port.WriteLine(cmd);
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //R6441Bからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private static bool ReadRecieveData(int time)
        {
            RecieveData = null;//念のため初期化
            port.ReadTimeout = time;
            try
            {
                RecieveData = port.ReadTo("=>\r\n");
                return true;
            }
            catch (TimeoutException)
            {
                return false;
            }
        }


        //**************************************************************************
        //DC電圧を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************       
        public static  bool SetDcV()
        {
            if (MeasMode != Mode.DCV)
            {
                if (!SendData("Z,F1,R5,PR2")) return false; // Z：初期化,F1：直流電圧測定,R5：20Vレンジ,PR2：サンプリングレートMid
                if (!ReadRecieveData(1000)) return false;
                General.Wait(1000);//モード変えたらウェイト
                MeasMode = Mode.DCV;
            }
            return true;

        }
        
        public static bool GetDcV()
        {
            double buff = 0;        
            try
            {

                
                //'MD?:測定データ出力要求
                if (!SendData("MD?") || !ReadRecieveData(200)) return false;
                return (Double.TryParse(RecieveData.Substring(4), out buff ));
            }
            catch 
            {
                return false;
            }
            finally
            {
                VoltData = buff;
            }
        }

        //**************************************************************************
        //DC電流を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************      
        public static bool SetDcA()
        {
            if (MeasMode != Mode.DCA)
            {
                if (!SendData("Z,F5,R6,PR2")) return false; //Z：初期化,F5：直流電流測定,R6：200mAレンジ,PR2：サンプリングレートMid
                if (!ReadRecieveData(1000)) return false;
                General.Wait(1000);//モード変えたらウェイト
                MeasMode = Mode.DCA;
            }
            return true;
        }
        
        public static bool GetDcA()
        {
            double buff = 0;
            try
            {
                //'MD?:測定データ出力要求
                if (!SendData("MD?") || !ReadRecieveData(1000)) return false;
                return (Double.TryParse(RecieveData.Substring(4), out buff));
            }
            catch
            {
                return false;
            }
            finally
            {
                AmpData = buff;
            }
        }

        //**************************************************************************
        //周波数を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool SetFreq()
        {
            if (MeasMode != Mode.FREQ)
            {
                if (!SendData("Z,F50,R4,PR1")) return false; //Z：初期化,F50：周波数測定,R5：20kHzレンジ,PR1：サンプリングレートFAST
                if (!ReadRecieveData(1000)) return false;
                //General.Wait(3000);//モード変えたらウェイト
                MeasMode = Mode.FREQ;
            }
            return true;
        }

        public static bool GetFreq()
        {
            double buff  = 0;
            try
            {
                //'MD?:測定データ出力要求
                if (!SendData("MD?") || !ReadRecieveData(200)) return false;
                return (Double.TryParse(RecieveData.Substring(5), out buff));
            }
            catch
            {
                return false;
            }
            finally
            {
                FreqData = buff;
            }
        }


        //**************************************************************************
        //COMポートを閉じる処理
        //引数：なし
        //戻値：なし
        //**************************************************************************   
        public static void ClosePort()
        {
            if (port.IsOpen) port.Close();
        }

    }


}