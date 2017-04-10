using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;

namespace DataLogTester
{


    public static class State
    {

        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□
        //parameter.odsﾌｧｲﾙからロードしたデータの保存用リスト（ジェネリックコレクション使用）
        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□

        private static List<string> ParameterSpec = new List<string>();         //検査スペック
        private static List<string> ParameterOperator = new List<string>();     //作業者リスト
        private static List<TestProperty> ParameterTestProperty = new List<TestProperty>(); //検査項目
        private static List<string> ParameterLedPoint = new List<string>();     //LEDの座標データ（画像検査用）
        private static List<Model> ParameterModel = new List<Model>();        //製品型番リスト


        
        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□
        //試験用パラメータ（クラス外からは読み取り専用のプロパティ）　※プロパティの自動実装を利用　C#3.0より対応
        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□

        //①製品ソフトウェア情報
        public static string FirmVer{get; private set;}
        public static string FirmSum {get; private set;}
        public static string FirmName {get; private set;}

        //②試験スペック
        public static double VccMax {get; private set;}     //電源電圧5V上限
        public static double VccMin {get; private set;}     //電源電圧5V下限
        public static double VeeMax {get; private set;}     //電源電圧3.3V上限
        public static double VeeMin{get; private set;}      //電源電圧3.3V下限
        public static double CurrMax{get; private set;}     //消費電流上限
        public static double CurrMin { get; private set; }  //消費電流下限
        public static double RtcMax{get; private set;}      //リアルタイムクロック誤差上限
        public static float Led_H_Max { get; private set; }
        public static float Led_H_Min { get; private set; }


        //③LED X座標
        public static int Tx1_X{get; private set;}
        public static int Tx2_X{get; private set;}
        public static int Tx3_X{get; private set;}
        public static int Tx4_X{get; private set;}
        public static int Rx1_X{get; private set;}
        public static int Rx2_X{get; private set;}
        public static int Rx3_X{get; private set;}
        public static int Rx4_X{get; private set;}
        public static int Vcc_X{get; private set;}
        public static int Run_X{get; private set;}
        public static int Cpu_X{get; private set;}
        public static int Di_X{get; private set;}
        public static int Do_X{get; private set;}
        
        //④LED Y座標
        public static int Tx1_Y{get; private set;}
        public static int Tx2_Y{get; private set;}
        public static int Tx3_Y{get; private set;}
        public static int Tx4_Y{get; private set;}
        public static int Rx1_Y{get; private set;}
        public static int Rx2_Y{get; private set;}
        public static int Rx3_Y{get; private set;}
        public static int Rx4_Y{get; private set;}
        public static int Vcc_Y{get; private set;}
        public static int Run_Y{get; private set;}
        public static int Cpu_Y{get; private set;}
        public static int Di_Y{get; private set;}
        public static int Do_Y{get; private set;}






        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□
        //検査用グローバル変数
        //□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□□
        public static string SdMemory;    //検査する製品型番に実装するSDカードの容量　型番が入力されたときにタイマーイベントで代入する



        private static int ProgressBarCnt;
        public static int 電源投入時の待ち時間;
        public static int StepNo;
        public static string 試験項目;
        public static string エラーメッセージ;
        public static int TotalStep;//トータルステップ数（プログレスバーの参照用）

        public static Multimeter ItemMultimeter { get; set; }


        //**************************************************************************
        //パラメータ取得用プロパティ
        //引数：なし
        //戻値：List<string>
        //**************************************************************************

        public static List<string> 作業者リスト
        {
            get
            {
                return ParameterOperator;
            }
        }

        public static List<TestProperty> 試験項目リスト
        {
            get
            {
                return ParameterTestProperty;
            }
        }

        public static List<Model> 製品型番リスト
        {
            get
            {
                return ParameterModel;
            }
        }

        public static int プログレスバーカウント
        {
            get
            {
                return ProgressBarCnt;
            }

            set
            {
                if (value <= TotalStep)
                {
                    ProgressBarCnt = value;
                }
                else
                {
                    return; //プログレスバーカウンターがトータルステップ数を超えたらセットを禁止する（オーバーフロー防止）
                }
            }
        }


        //**************************************************************************
        //パラメータ（検査スペック、作業者一覧、試験項目、ＬＥＤ座標、色データ、製品型番リストのロード（parameter.odsファイルより読み出し）
        //引数：なし
        //戻値：bool値
        //**************************************************************************
        public static bool LoadParameter()
        {

            try
            {

                //すべての要素をクリア
                ParameterSpec.Clear();
                ParameterOperator.Clear();
                ParameterTestProperty.Clear();
                ParameterLedPoint.Clear();
                ParameterModel.Clear();

                OpenOffice calc = new OpenOffice();

                //parameterファイルを開く
                calc.OpenFile(Constants.ParameterFilePath);

                // sheetを取得　"Spec"
                calc.SelectSheet("Spec");

                //①試験用パラメータの取り込み
                int i = 0;
                int row = 1;    //開始行番号
                string cellString = "";

                while (true)
                {
                    calc.cell = calc.sheet.getCellByPosition(1, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    if (cellString == "") break;
                    ParameterSpec.Add(cellString);

                    i++;
                }

                // sheetを取得　"OperatorName"
                calc.SelectSheet("OperatorName");

                //②作業者一覧の取り込み               
                //行＝i 列＝j 
                int j = 1;
                int k = 1;
                calc.cell = calc.sheet.getCellByPosition(k, j);
                // セルに入力されている文字列を取得
                cellString = calc.cell.getFormula();
                while (cellString != "予約" && j < 11)
                {
                    ParameterOperator.Add(cellString);
                    j++;
                    calc.cell = calc.sheet.getCellByPosition(k, j);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                }


                // sheetを取得　"TestCase"
                calc.SelectSheet("TestCase");

                //④検査項目の取り込み
                i = 0;
                row = 1;    //開始行番号
                cellString = "";

                while (true)
                {
                    var buff = new TestProperty();

                    calc.cell = calc.sheet.getCellByPosition(0, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    if (cellString == "") break;
                    buff.stepNo = Int32.Parse(cellString);

                    calc.cell = calc.sheet.getCellByPosition(1, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    buff.testCase = cellString;

                    calc.cell = calc.sheet.getCellByPosition(2, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    buff.errorMessage = cellString;

                    ParameterTestProperty.Add(buff); //リストに追加

                    i++;
                }

                // sheetを取得　"LedPoint"
                calc.SelectSheet("LedPoint");

                //④検査項目の取り込み
                i = 0;
                row = 1;    //開始行番号
                cellString = "";

                while (true)
                {
                    calc.cell = calc.sheet.getCellByPosition(1, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    if (cellString == "") break;
                    ParameterLedPoint.Add(cellString);

                    calc.cell = calc.sheet.getCellByPosition(2, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    ParameterLedPoint.Add(cellString);

                    i++;
                }



                // sheetを取得　"製品型番"
                calc.SelectSheet("製品型番");

                i = 0;
                row = 1;    //開始行番号
                cellString = "";

                while (true)
                {
                    var buff = new Model();

                    calc.cell = calc.sheet.getCellByPosition(0, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    if (cellString == "") break;
                    buff.name = cellString;

                    calc.cell = calc.sheet.getCellByPosition(1, row + i);
                    // セルに入力されている文字列を取得
                    cellString = calc.cell.getFormula();
                    buff.memory = cellString;

                    ParameterModel.Add(buff); //リストに追加

                    i++;
                }


                calc.CloseFile();

                return true;
            }
            catch
            {
                return false;
            }
        }
        //**************************************************************************
        //検査スペックのセット
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static bool SetSpec()
        {
            try
            {
                //検査規格用プロパティに試験スペックをセットする
                FirmVer = ParameterSpec[0];
                FirmSum = ParameterSpec[1];
                FirmName = ParameterSpec[2];
                VccMax = Double.Parse(ParameterSpec[3]);
                VccMin = Double.Parse(ParameterSpec[4]);
                VeeMax = Double.Parse(ParameterSpec[5]);
                VeeMin = Double.Parse(ParameterSpec[6]);
                CurrMax = Double.Parse(ParameterSpec[7]);
                CurrMin = Double.Parse(ParameterSpec[8]);
                RtcMax = Double.Parse(ParameterSpec[9]);
                Led_H_Max = float.Parse(ParameterSpec[10]);
                Led_H_Min = float.Parse(ParameterSpec[11]);


                Rx1_X = Int32.Parse(ParameterLedPoint[0]);
                Rx1_Y = Int32.Parse(ParameterLedPoint[1]);
                Rx2_X = Int32.Parse(ParameterLedPoint[2]);
                Rx2_Y = Int32.Parse(ParameterLedPoint[3]);
                Rx3_X = Int32.Parse(ParameterLedPoint[4]);
                Rx3_Y = Int32.Parse(ParameterLedPoint[5]);
                Rx4_X = Int32.Parse(ParameterLedPoint[6]);
                Rx4_Y = Int32.Parse(ParameterLedPoint[7]);

                Tx1_X = Int32.Parse(ParameterLedPoint[8]);
                Tx1_Y = Int32.Parse(ParameterLedPoint[9]);
                Tx2_X = Int32.Parse(ParameterLedPoint[10]);
                Tx2_Y = Int32.Parse(ParameterLedPoint[11]);
                Tx3_X = Int32.Parse(ParameterLedPoint[12]);
                Tx3_Y = Int32.Parse(ParameterLedPoint[13]);
                Tx4_X = Int32.Parse(ParameterLedPoint[14]);
                Tx4_Y = Int32.Parse(ParameterLedPoint[15]);

                Vcc_X = Int32.Parse(ParameterLedPoint[16]);
                Vcc_Y = Int32.Parse(ParameterLedPoint[17]);
                Run_X = Int32.Parse(ParameterLedPoint[18]);
                Run_Y = Int32.Parse(ParameterLedPoint[19]);
                Cpu_X = Int32.Parse(ParameterLedPoint[20]);
                Cpu_Y = Int32.Parse(ParameterLedPoint[21]);
                Di_X = Int32.Parse(ParameterLedPoint[22]);
                Di_Y = Int32.Parse(ParameterLedPoint[23]);
                Do_X = Int32.Parse(ParameterLedPoint[24]);
                Do_Y = Int32.Parse(ParameterLedPoint[25]);



                

                return true;
            }
            catch
            {
                return false;
            }

        }


    }
}
