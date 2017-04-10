using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace DataLogTester
{
    public static class TEST_スイッチ
    {
        private static int BitData;
        private static string OldBuff;
        private static string NewBuff;
        private static string sBuff;


        public static bool CheckS1(SwMode mode, Action<SwName, string, int> process1, Action<string> process2)
        {

            int Spec = 0;
            if (mode == SwMode.奇数On)
            {
                Spec = 85;
            }
            else if (mode == SwMode.AllOn)
            {
                Spec = 255;
            }
            else if (mode == SwMode.AllOff)
            {
                Spec = 0;
            }

            //スイッチデータの取り込み
            foreach (var i in Enumerable.Range(0, 10))
            {
                Application.DoEvents();
                General.Wait(1000);
                Target.sendData(Target.PortName.P1_232C, "ReadS1");//通信ログに追加します
                if (Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) break;
                if (i == 9) return false;
            }

            ////通信ログに書き込む処理
            //OldBuff = NewBuff;
            //NewBuff = Target.RecieveData.Substring(1);
            //if (OldBuff != NewBuff) process2("Rx_" + NewBuff + "\r\n");//MainForm通信ログの更新

            //ビットデータの生成
            sBuff = Target.RecieveData.Substring(4);
            BitData = Convert.ToInt32(sBuff, 16);

            //表示更新
            process1(SwName.S1, sBuff.ToUpper(), BitData);//MainForm表示の更新

            return (BitData == Spec);

        }

 

        public static bool CheckSw1_2(SwName name, Action<SwName, string, int> process1, Action<string> process2)
        {
            int start = (name == SwName.SW1) ? 8 : 7; 

            //現在のビットが何かチェックする
            Target.sendData(Target.PortName.P1_232C, "ReadSw1_2");
            if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) return false;
            NewBuff = Target.RecieveData.Substring(1);
            
            sBuff = Target.RecieveData.Substring(start, 1);
            BitData = Convert.ToInt32(sBuff, 16);//現在のビット 先頭はSTXであることに注意
            process1(name, sBuff.ToUpper(), BitData);//MianFormの更新


            //スタートビットにより列挙する順番を決める
            List<int> Spec = new List<int>();
            foreach (int i in Enumerable.Range(0, 16))
            {
                if (BitData > 15) BitData = 0;
                Spec.Add(BitData++);
            }

            foreach (int spec in Spec)
            {
                while (true)
                {
                    General.Wait(250);
                    Target.sendData(Target.PortName.P1_232C, "ReadSw1_2", false);
                    if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000, false) || General.checkStopButton()) return false;

                    //通信ログに書き込む処理
                    OldBuff = NewBuff;
                    NewBuff = Target.RecieveData.Substring(1);
                    if (OldBuff != NewBuff) process2("Rx_" + NewBuff + "\r\n");


                    sBuff = Target.RecieveData.Substring(start, 1);
                    BitData = Convert.ToInt32(sBuff, 16);
                    process1(name, sBuff.ToUpper(), BitData);//MianFormの更新

                    if (BitData == spec)
                    {
                        General.PlaySound2(Constants.OnSound);
                        break;
                    }
                }
            }

            //音鳴らして終了
            General.PlaySound2(Constants.EndSound);
            return true;

        }

        public static bool InitializeSw1_2(SwName name, int setData ,Action<SwName, string, int> process1, Action<string> process2)
        {
            int start = (name == SwName.SW1) ? 8 : 7; 

            //現在のビットが何かチェックする
            Target.sendData(Target.PortName.P1_232C, "ReadSw1_2");
            if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) return false;
            NewBuff = Target.RecieveData.Substring(1);

            sBuff = Target.RecieveData.Substring(start, 1);
            BitData = Convert.ToInt32(sBuff, 16);//現在のビット 先頭はSTXであることに注意
            process1(name, sBuff.ToUpper(), BitData);//MianFormの更新

            while (true)
            {
                Application.DoEvents();
                Target.sendData(Target.PortName.P1_232C, "ReadSw1_2", false);
                if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000, false) || General.checkStopButton()) return false;
                
                OldBuff = NewBuff;
                NewBuff = Target.RecieveData.Substring(1);

                if (OldBuff != NewBuff) process2("Rx_" + NewBuff + "\r\n");

                sBuff = Target.RecieveData.Substring(start, 1);
                BitData = Convert.ToInt32(sBuff, 16);
                process1(name, sBuff.ToUpper(), BitData);//MianFormの更新

                if (BitData == setData)
                {
                    //音鳴らして終了
                    General.PlaySound2(Constants.OnSound);
                    break;
                }
                General.Wait(50);
            }

            return true;
        }
    }
}
