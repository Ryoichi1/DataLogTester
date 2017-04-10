using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Windows.Forms;
using System.Timers;
using System.Threading;

namespace DataLogTester
{

    public static class Agilent34401A
    {
        //列挙型の宣言
        public enum ComNumber
        {
            COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, COM10,
        }
        
        //変数の宣言（インスタンスメンバーになります）
        private static SerialPort port;
        private static string RecieveData;//34401Aから受信した生データ

        public static double VoltData { get; private set; }//計測したDC_VOLTデータ
        public static double CurrData { get; private set; }//計測したAC_CURRENTデータ


        //コンストラクタ
        static Agilent34401A()
        {
            port = new SerialPort();
        }


        //**************************************************************************
        //34401Aの初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static bool Init34401A(ComNumber comNum)
        {
            try
            {
                if (!port.IsOpen)
                { 
                    port.PortName = comNum.ToString(); 
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = System.IO.Ports.Parity.None;
                    port.StopBits = System.IO.Ports.StopBits.One;
                    port.DtrEnable = true;
                    port.NewLine = ("\r\n");
                    port.Open();
                }
                //コマンド送信
                port.WriteLine(":SYST:REM");
                General.Wait(500);
                port.WriteLine("*CLS");
                General.Wait(500);
                port.WriteLine("*RST");
                General.Wait(500);
                
                //クエリ送信
                port.WriteLine(":SYST:ERR?");
                bool respons = ReadRecieveData(1000);
                if (respons) return true;
                
                return false;
            }
            catch
            {
                return false;
            }
        }



        //**************************************************************************
        //34401Aからの受信データを読み取る
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ErrorCode
        //**************************************************************************
        private static bool ReadRecieveData(int time)
        {
            
            RecieveData = null;//念のため初期化
            string buffer = null;
            port.ReadTimeout = time;
            try
            {
                buffer = port.ReadTo("\r\n");
            }
            catch (TimeoutException)
            {
                return false;
            }

            RecieveData = buffer;
            return true;
        }


        //**************************************************************************
        //直流電圧を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool GetDcVoltage()
        {
            try
            {              
                port.WriteLine(":MEAS:VOLT:DC?");
                General.Wait(500);

                bool respons = ReadRecieveData(1000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                VoltData = Double.Parse(RecieveData);

                return true;
            }
            catch
            {
                return false;

            }

        }


        //**************************************************************************
        //DC電流を取得する
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool GetDcCurrent()
        {
            try
            {
                port.WriteLine(":MEAS:CURR:DC?");


                bool respons = ReadRecieveData(2000);
                if (!respons)
                {
                    return respons;//falseが返ります
                }

                CurrData = Double.Parse(RecieveData);
                return true;
            }
            catch
            {
                return false;

            }
        }

        //**************************************************************************
        //COMポートを閉じる
        //引数：なし
        //戻値：bool
        //**************************************************************************   
        public static bool ClosePort()
        {
            try
            {
                //ca100用のポートが開いているかどうかの判定
                if (port.IsOpen)
                {
                    port.WriteLine(":SYST:LOC");
                    port.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }


}