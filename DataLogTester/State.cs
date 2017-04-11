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
        //パブリックメンバ
        public static Configuration Setting { get; set; }
        public static TestSpec TestSpec { get; set; }

        public static Cam0Property cam0Prop { get; set; }

        //インスタンスをXMLデータに変換する
        public static bool Serialization<T>(T obj, string xmlFilePath)
        {
            try
            {
                //XmlSerializerオブジェクトを作成
                //オブジェクトの型を指定する
                System.Xml.Serialization.XmlSerializer serializer =
                    new System.Xml.Serialization.XmlSerializer(typeof(T));
                //書き込むファイルを開く（UTF-8 BOM無し）
                System.IO.StreamWriter sw = new System.IO.StreamWriter(xmlFilePath, false, new System.Text.UTF8Encoding(false));
                //シリアル化し、XMLファイルに保存する
                serializer.Serialize(sw, obj);
                //ファイルを閉じる
                sw.Close();

                return true;

            }
            catch
            {
                return false;
            }

        }

        //XMLデータからインスタンスを生成する
        public static T Deserialize<T>(string xmlFilePath)
        {
            System.Xml.Serialization.XmlSerializer serializer;
            using (var sr = new System.IO.StreamReader(xmlFilePath, new System.Text.UTF8Encoding(false)))
            {
                serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(sr);
            }
        }

        //個別設定のロード
        public static void LoadConfigData()
        {
            //Configファイルのロード
            Setting = Deserialize<Configuration>(Constants.filePath_Configuration);

            //TestSpecファイルのロード
            TestSpec = Deserialize<TestSpec>(Constants.filePath_TestSpec);//TODO:

            //カメラプロパティファイルのロード
            cam0Prop = Deserialize<Cam0Property>(Constants.filePath_Cam0Property);

        }

        //********************************************************
        //個別設定データの保存
        //********************************************************
        public static bool Save個別データ()
        {
            try
            {
                //Configファイルの保存 j作業者リストの更新
                Serialization<Configuration>(Setting, Constants.filePath_Configuration);
                
                //Cam0プロパティの保存
                Serialization<Cam0Property>(cam0Prop, Constants.filePath_Cam0Property);

                return true;
            }
            catch
            {
                return false;

            }

        }

        public static List<TestProperty> 試験項目リスト = new List<TestProperty>()
        {
            new TestProperty(0, "重複試験チェック"),
            new TestProperty(10, "コネクタ実装チェック"),
            new TestProperty(20, "検査ソフト書き込み"),
            new TestProperty(30, "Vcc電圧チェック"),
            new TestProperty(40, "+3.3V電圧チェック"),
            new TestProperty(50, "消費電流チェック"),
            new TestProperty(60, "Port1 RS422通信チェック"),
            new TestProperty(61, "Port2 RS422通信チェック"),
            new TestProperty(62, "Port3 RS422通信チェック"),
            new TestProperty(63, "Port4 RS422通信チェック"),
            new TestProperty(70, "Port1 RS232C通信チェック"),
            new TestProperty(71, "Port2 RS232C通信チェック"),
            new TestProperty(72, "Port3 RS232C通信チェック"),
            new TestProperty(73, "Port4 RS232C通信チェック"),
            new TestProperty(80, "LED点灯チェック"),
            new TestProperty(90, "SW3チェック"),
            new TestProperty(100, "SDカードチェック"),
            new TestProperty(110, "リアルタイムクロックセット"),
            new TestProperty(120, "SW1チェック"),
            new TestProperty(130, "SW2チェック"),
            new TestProperty(140, "リアルタイムクロックチェック"),
            new TestProperty(150, "S1チェック"),
            new TestProperty(160, "SW1出荷設定"),
            new TestProperty(161, "SW2出荷設定"),
            new TestProperty(162, "S1出荷設定"),
            new TestProperty(170, "製品ソフト書き込み"),
            new TestProperty(180, "最終通電チェック"),

        };


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






    }
}
