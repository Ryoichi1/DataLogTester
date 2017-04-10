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
    public static class Target
    {
        //列挙型の宣言
        public enum PortName
        {
            P1_232C, P2_232C, P3_232C, P4_232C, P1_422, P2_422, P3_422,P4_422,
        }

        public static Action<string> Process1;
        public static Action<string> Process2;
        public static Action<string> Process3;

        //静的メンバの宣言
        public static SerialPort Port1_232c;    //Port1 RS232C
        public static SerialPort Port1_422;     //Port1 RS422
        public static SerialPort Port2_232c;    //Port2 RS232C
        public static SerialPort Port2_422;     //Port2 RS422
        public static SerialPort Port3_232c;    //Port3 RS232C
        public static SerialPort Port3_422;     //Port3 RS422
        public static SerialPort Port4_232c;    //Port4 RS232C
        public static SerialPort Port4_422;     //Port4 RS422


        public static string RecieveData { get; private set; }  //Targetから受信した生データ

        //コンストラクタ
        static Target()
        {
            Port1_232c = new SerialPort();
            Port2_232c = new SerialPort();
            Port3_232c = new SerialPort();
            Port4_232c = new SerialPort();

            Port1_422 = new SerialPort();
            Port2_422 = new SerialPort();
            Port3_422 = new SerialPort();
            Port4_422 = new SerialPort();
        
        }

        //デリゲートのセット
        public static void InitTarget(Action process)
        {
            process();
        }

        //イニシャライズ
        public static bool InitPort()
        {
            try
            {
                //ポート1 RS232Cの設定
                Port1_232c.BaudRate = 9600;
                Port1_232c.DataBits = 7;
                Port1_232c.Parity = System.IO.Ports.Parity.Even;
                Port1_232c.StopBits = System.IO.Ports.StopBits.Two;
                Port1_232c.NewLine = "\r\n" + (char)0x03;
                Port1_232c.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port1_232c.IsOpen == false)
                {
                    Port1_232c.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port1_RS232C初期化異常");
                return false;
            }

            try
            {
                //ポート2 RS232Cの設定
                Port2_232c.BaudRate = 9600;
                Port2_232c.DataBits = 7;
                Port2_232c.Parity = System.IO.Ports.Parity.Even;
                Port2_232c.StopBits = System.IO.Ports.StopBits.Two;
                Port2_232c.NewLine = "\r\n" + (char)0x03;
                Port2_232c.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port2_232c.IsOpen == false)
                {
                    Port2_232c.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port2_RS232C初期化異常");
                return false;
            }

            try
            {
                //ポート3 RS232Cの設定
                Port3_232c.BaudRate = 9600;
                Port3_232c.DataBits = 7;
                Port3_232c.Parity = System.IO.Ports.Parity.Even;
                Port3_232c.StopBits = System.IO.Ports.StopBits.Two;
                Port3_232c.NewLine = "\r\n" + (char)0x03;
                Port3_232c.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port3_232c.IsOpen == false)
                {
                    Port3_232c.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port3_RS232C初期化異常");
                return false;
            }

            try
            {
                //ポート4 RS232Cの設定
                Port4_232c.BaudRate = 9600;
                Port4_232c.DataBits = 7;
                Port4_232c.Parity = System.IO.Ports.Parity.Even;
                Port4_232c.StopBits = System.IO.Ports.StopBits.Two;
                Port4_232c.NewLine = "\r\n" + (char)0x03;
                Port4_232c.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port4_232c.IsOpen == false)
                {
                    Port4_232c.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port4_RS232C初期化異常");
                return false;
            }

            try
            {
                //ポート1 RS422の設定
                Port1_422.BaudRate = 9600;
                Port1_422.DataBits = 7;
                Port1_422.Parity = System.IO.Ports.Parity.Even;
                Port1_422.StopBits = System.IO.Ports.StopBits.Two;
                Port1_422.NewLine = "\r\n" + (char)0x03;
                Port1_422.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port1_422.IsOpen == false)
                {
                    Port1_422.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port1_RS422初期化異常");
                return false;
            }
            
            try
            {
            //ポート2 RS422の設定
                Port2_422.BaudRate = 9600;
                Port2_422.DataBits = 7;
                Port2_422.Parity = System.IO.Ports.Parity.Even;
                Port2_422.StopBits = System.IO.Ports.StopBits.Two;
                Port2_422.NewLine = "\r\n" + (char)0x03;
                Port2_422.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port2_422.IsOpen == false)
                {
                    Port2_422.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port2_RS422初期化異常");
                return false;
            }
                
            try
            {
            //ポート3 RS422の設定
                Port3_422.BaudRate = 9600;
                Port3_422.DataBits = 7;
                Port3_422.Parity = System.IO.Ports.Parity.Even;
                Port3_422.StopBits = System.IO.Ports.StopBits.Two;
                Port3_422.NewLine = "\r\n" + (char)0x03;
                Port3_422.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port3_422.IsOpen == false)
                {
                    Port3_422.Open();
                }
            }
            catch
            {
                MessageBox.Show("Port3_RS422初期化異常");
                return false;
            }

            try
            {
            //ポート4 RS422の設定
                Port4_422.BaudRate = 9600;
                Port4_422.DataBits = 7;
                Port4_422.Parity = System.IO.Ports.Parity.Even;
                Port4_422.StopBits = System.IO.Ports.StopBits.Two;
                Port4_422.NewLine = "\r\n" + (char)0x03;
                Port4_422.ReadTimeout = 1500;

                //通信ポートを開く               
                if (Port4_422.IsOpen == false)
                {
                    Port4_422.Open();
                }                   
            }
            catch
            {
                MessageBox.Show("Port4_RS422初期化異常");
                return false;
            }

            return true;
            
        }


        //**************************************************************************
        //Targetに文字列を送信する
        //引数：コマンド
        //戻値：ブール値
        //**************************************************************************
        public static bool sendData(PortName pName, string Data, bool setLog = true)
        {
            string sendData = (char)0x02 + Data;
            if (setLog) Process1("Tx_" + Data + "\r\n");

            try
            {
                ClearBuff();
                switch (pName)
                {
                    case PortName.P1_232C:
                        Port1_232c.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P2_232C:
                        Port2_232c.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P3_232C:
                        Port3_232c.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P4_232C:
                        Port4_232c.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P1_422:
                        Port1_422.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P2_422:
                        Port2_422.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P3_422:
                        Port3_422.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                    case PortName.P4_422:
                        Port4_422.WriteLine(sendData);// +"\r\n"は付加されている
                        break;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        //**************************************************************************
        //Targetからの受信データを読み取る（試験ソフト動作時）
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ブール値
        //**************************************************************************
        public static bool ReadRecieveData(PortName pName, int time, bool setLog = true)
        {
            //ReadLine()で取り込んだデータにはNewLine値（"\r\n" + (char)0x03）は含まれません!

            RecieveData = null;//念のため初期化
            string buffer = null;
            try
            {
                switch (pName)
                {
                    case PortName.P1_232C:
                        Port1_232c.ReadTimeout = time;
                        buffer = Port1_232c.ReadLine();
                        break;
                    case PortName.P2_232C:
                        Port2_232c.ReadTimeout = time;
                        buffer = Port2_232c.ReadLine();
                        break;
                    case PortName.P3_232C:
                        Port3_232c.ReadTimeout = time;
                        buffer = Port3_232c.ReadLine();
                        break;
                    case PortName.P4_232C:
                        Port4_232c.ReadTimeout = time;
                        buffer = Port4_232c.ReadLine();
                        break;
                    case PortName.P1_422:
                        Port1_422.ReadTimeout = time;
                        buffer = Port1_422.ReadLine();
                        break;
                    case PortName.P2_422:
                        Port2_422.ReadTimeout = time;
                        buffer = Port2_422.ReadLine();
                        break;
                    case PortName.P3_422:
                        Port3_422.ReadTimeout = time;
                        buffer = Port3_422.ReadLine();
                        break;
                    case PortName.P4_422:
                        Port4_422.ReadTimeout = time;
                        buffer = Port4_422.ReadLine();
                        break;
                }

            }
            catch (TimeoutException)
            {
                if (setLog) Process1("Rx_" + "Error" + "\r\n");
                return false;
            }

            RecieveData = buffer;
            if(setLog) Process1("Rx_" + Target.RecieveData.Substring(1) + "\r\n");           
            return true;
        }

        //**************************************************************************
        //電源投入後にTargetから送信されるデータを読み取る（製品ソフト動作時） 正常であればPort1から"@00RD2120000354*"が返ってくる
        //引数：指定時間（ｍｓｅｃ）
        //戻値：ブール値
        //**************************************************************************
        public static bool GetInitData()
        {
            RecieveData = null;//念のため初期化
            string buffer = null;
            
            //電源ON
            General.PowerOn();//このメソッドの中でCOM受信バッファクリアしてます
            General.Wait(3000);//正常な場合製品からデータが送信されてくるので待つ

            //SW3を押してリセットかける（稀に電源投入後に通信しないことがあるため）
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.H);
            General.Wait(500);
            Target.ClearBuff();//受信バッファのクリア
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.L);
            try
            {
                Port1_232c.ReadTimeout = 5000;
                buffer = Port1_232c.ReadTo("*");

                RecieveData = buffer;
                Process1("Rx_" + Target.RecieveData + "\r\n");
                return true;
            }
            catch (TimeoutException)
            {
                Process1("Rx_" + "Error" + "\r\n");
                return false;
            }
            finally
            {
                General.Wait(500);
                General.PowerOff();
            }


        }



        //**************************************************************************
        //COMポートを閉じる処理
        //引数：なし
        //戻値：なし
        //**************************************************************************   
        public static void ClosePort()
        {
            if (Port1_232c.IsOpen) Port1_232c.Close();
            if (Port2_232c.IsOpen) Port2_232c.Close();
            if (Port3_232c.IsOpen) Port3_232c.Close();
            if (Port4_232c.IsOpen) Port4_232c.Close();
            if (Port1_422.IsOpen)  Port1_422.Close();
            if (Port2_422.IsOpen)  Port2_422.Close();           
            if (Port3_422.IsOpen)  Port3_422.Close();           
            if (Port4_422.IsOpen)  Port4_422.Close();
        }

        //**************************************************************************
        //各ＣＯＭポートの受信バッファ内にある受信データのバイト数を取得する
        //引数：
        //戻値：int
        //**************************************************************************
        public static int GetByteData(PortName pName)
        {
            try
            {
                switch (pName)
                {
                    case PortName.P1_232C:
                        return Port1_232c.BytesToRead;
                    case PortName.P2_232C:
                        return Port2_232c.BytesToRead;
                    case PortName.P3_232C:
                        return Port3_232c.BytesToRead;
                    case PortName.P4_232C:
                        return Port4_232c.BytesToRead;
                    case PortName.P1_422:
                        return Port1_422.BytesToRead;
                    case PortName.P2_422:
                        return Port2_422.BytesToRead;
                    case PortName.P3_422:
                        return Port3_422.BytesToRead;
                    case PortName.P4_422:
                        return Port4_422.BytesToRead;
                    default:
                        return 0;
                }
            }
            catch (TimeoutException)
            {
                return 0;
            }

        }

        //**************************************************************************
        //受信バッファをクリアする
        //引数：
        //戻値：bool
        //**************************************************************************
        public static bool ClearBuff()
        {
            Port1_232c.DiscardInBuffer();
            Port2_232c.DiscardInBuffer();
            Port3_232c.DiscardInBuffer();
            Port4_232c.DiscardInBuffer();
            
            Port1_422.DiscardInBuffer();
            Port2_422.DiscardInBuffer();
            Port3_422.DiscardInBuffer();
            Port4_422.DiscardInBuffer();


            return true;
        }

        ////ターゲットにコマンドを送信する
        //public void sendCommand(Target.PortName pName, string data)
        //{
        //    Target.sendData(pName, data);
        //    textBoxCommLog.Text += "Tx_" + data + "\r\n";
        //    this.Refresh();
        //}

        ////ターゲットにコマンドを送信する
        //public void sendCommand2(Target.PortName pName, string data)
        //{
        //    Target.sendData(pName, data);
        //    //textBoxCommLog.Text += "Tx_" + data + "\r\n";
        //    //this.Refresh();
        //}

        ////ターゲットからデータを受信する（試験ソフト動作時）
        //public bool getData(Target.PortName pName, int waitTime)
        //{
        //    if (!Target.ReadRecieveData(pName, waitTime))
        //    {
        //        textBoxCommLog.Text += "Rx_" + "Error" + "\r\n";
        //        this.Refresh();
        //        return false;
        //    }

        //    textBoxCommLog.Text += "Rx_" + Target.RecieveData.Substring(1) + "\r\n";
        //    this.Refresh();
        //    return true;

        //}

        //ターゲットからデータを受信する（SW1,SW2,S1のデータ取得専用）
        //public bool getData2(Target.PortName pName, int waitTime)
        //{
        //    string buff;
        //    buff = Target.RecieveData;//データを受信する前に、前回取得したデータを取り込む

        //    if (!Target.ReadRecieveData(pName, waitTime))
        //    {
        //        textBoxCommLog.Text += "Rx_" + "Error" + "\r\n";
        //        this.Refresh();
        //        return false;
        //    }

        //    if (buff != Target.RecieveData) //取得したデータに変化があればログに表示する
        //    {
        //        textBoxCommLog.Text += "Rx_" + Target.RecieveData.Substring(1) + "\r\n";
        //        this.Refresh();
        //    }
        //    return true;
        //}


        ////ターゲットからデータを受信する（製品ソフト動作時）
        //public bool getData2(int waitTime)
        //{
        //    if (!Target.GetInitData(waitTime))
        //    {
        //        textBoxCommLog.Text += "Rx_" + "Error" + "\r\n";
        //        this.Refresh();
        //        return false;
        //    }

        //    textBoxCommLog.Text += "Rx_" + Target.RecieveData + "\r\n";
        //    this.Refresh();
        //    return true;

        //}
    }
}
