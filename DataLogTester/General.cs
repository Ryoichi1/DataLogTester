using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using DirectShowLib;
using System.Drawing.Imaging;

namespace DataLogTester
{
    public static class General
    {

        public static EPX64S io = new EPX64S();
        public static Capture cam;
        public static System.Media.SoundPlayer player = null;

        //**************************************************************************
        //EPX64のリセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void ResetIo()
        {
            //IOを初期化する処理（出力をすべてＬに落とす）
            io.OutByte(EPX64S.PORT.P0, 0);
            io.OutByte(EPX64S.PORT.P1, 0);
            io.OutByte(EPX64S.PORT.P2, 0);
            General.Wait(100);
            Flags.PowerFlag = false;
        }

        //**************************************************************************
        //製品への電源供給
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static void PowerOn()
        {
            //電源投入前の処理(電源投入直後に、Targetより"ReqData"が送られてくるかチェックするため)
            Target.ClearBuff();
            //電源ON
            io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, EPX64S.OUT.H);
        }

        public static void PowerOff()
        {
            //電源ON
            io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, EPX64S.OUT.L);
        }

        public static bool 電源投入時の処理()
        {
            foreach (var i in Enumerable.Range(1, 5))
            {
                //USB-シリアル変換機に接続
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                General.Wait(500);

                //電源ON
                General.PowerOn();
                General.Wait(State.電源投入時の待ち時間);

                Application.DoEvents();
                if (Flags.StopFlag) return false;
                if (Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) break;
                ResetIo();
                Wait(500);
                State.電源投入時の待ち時間 += 500;
                if (i == 5) return false;    
            }
            return (Target.RecieveData.IndexOf("ReqData") >= 0);
        }
        //**************************************************************************
        //ウェイト（10msec単位で指定）
        //引数：なし
        //戻値：なし
        //**************************************************************************  
        public static void Wait(int time)
        {
            for (int i = 0; i < (time / 10); i++)
            {
                Thread.Sleep(10);
                Application.DoEvents();
            }
        }

        //**************************************************************************
        //プレス治具のレバーが下がっているかどうかの判定
        //引数：なし
        //戻値：プレス治具のレバーが下がっていればtrue、上がっていればfalseを返す
        //**************************************************************************
        public static bool CheckPress()
        {
            if (!General.io.ReadInputData(EPX64S.PORT.P3) )
            {
                Flags.Epx64Stutas = false;
                return false;
            }

            byte buff;

            //Port1チェック
            buff = General.io.P3InputData;
            return ((buff & 0x01) == 0x00);
        }

        //**************************************************************************
        //WAVEファイルを再生する
        //引数：なし
        //戻値：なし
        //**************************************************************************  

        //WAVEファイルを再生する（再生中は他の割り込み処理を受け付けない）
        public static void PlaySound(string waveFile)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = new System.Media.SoundPlayer(waveFile);
            //最後まで再生し終えるまで待機する
            //player.Play();
            player.PlaySync();
        }
        //WAVEファイルを再生する（非同期で再生）
        public static void PlaySound2(string waveFile)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = new System.Media.SoundPlayer(waveFile);
            //最後まで再生し終えるまで待機する
            player.Play();
        }
        //WAVEファイルをループ再生する
        public static void PlayLoopSound(string waveFile)
        {
            //再生されているときは止める
            if (player != null)
                player.Stop();

            //waveファイルを読み込む
            player = new System.Media.SoundPlayer(waveFile);
            //次のようにすると、ループ再生される
            player.PlayLooping();
        }
        //再生されているWAVEファイルを止める
        public static void StopSound()
        {
            if (player != null)
            {
                player.Stop();
                player.Dispose();
                player = null;
            }
        }



        //**************************************************************************
        //停止ボタンが押されたかどうかの判定
        //引数：なし
        //戻値：bool
        //**************************************************************************
        public static bool checkStopButton()
        {
            Application.DoEvents();//ここで割り込みを受け付ける
            return Flags.StopFlag;
        }

        //**************************************************************************
        //現在時刻を秒数に変換する（RTC確認用）　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static bool ConvertTime(string timeData, out double result)
        {
            result = 0;
            try //Targetからの時刻データが文字化けしていた場合例外発生するため、try/catchで対応（2014/5/29修正　V1.20）
            {
                //文字列（"/yy/MM/dd/HH/mm/ss"）の解析
                //HHが10番目

                string buff = timeData.Substring(10, 2); //HHの抽出
                double hh = Double.Parse(buff) * 3600;

                buff = timeData.Substring(13, 2); //mmの抽出
                double mm = Double.Parse(buff) * 60;

                buff = timeData.Substring(16, 2); //ssの抽出
                double ss = Double.Parse(buff);

                result = hh + mm + ss;
                return true;
            }
            catch
            {
                return false;
            }
        }


        //**************************************************************************
        //検査データの保存　　　　
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static bool SaveData(List<string> testData)
        {
            try
            {
                var 工番 = testData[0];
                //既存検査データ（同じ工番名のファイル）が存在するかどうかチェックする
                if (!System.IO.File.Exists(Constants.TestDataPath + 工番 + ".ods"))
                {
                    //既存検査データがなければ新規作成
                    File.Copy(Constants.TestDataPath + "format.ods", Constants.TestDataPath + 工番 + ".ods");
                }

                string filepath = Constants.TestDataPath + 工番 + ".ods";

                OpenOffice calc = new OpenOffice();
                //parameterファイルを開く
                calc.OpenFile(filepath);


                // sheetを取得
                calc.SelectSheet("Sheet1");

                //使用されているセルの最終行を検索する
                int i = 0;
                string cellString;
                while (true)
                {
                    Application.DoEvents();
                    calc.cell = calc.sheet.getCellByPosition(0, i);
                    cellString = calc.cell.getFormula();
                    if (cellString == "") break;
                    i++;
                }

                int newRow = i;
                int j = 0;
                foreach(var data in testData)
                {
                    calc.cell = calc.sheet.getCellByPosition(j, newRow);
                    calc.cell.setFormula(data);
                    j++;
                }

                // Calcファイルを保存して閉じる
                if (!calc.SaveFile()) return false;

                return true;

            }
            catch
            {
                return false;
            }


        }


    
    
    
    
    
    
    
    
    
    
    
    
    
    
    
    }
}
