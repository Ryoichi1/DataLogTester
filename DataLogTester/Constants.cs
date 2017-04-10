using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLogTester
{
    static class Constants
    {

        //*******************************************************************************
        //定数の宣言
        //*******************************************************************************    

        //検査ソフトＶｅｒ
        //public const string CheckerSoftVer = "1.00";   //新規作成 
        //public const string CheckerSoftVer = "1.10";   //メンテナンスモードでのカメラプロパティ画面表示機能を追加、クラス構成をステート集約型に変更
        //public const string CheckerSoftVer = "1.20";   //リアルタイムクロックチェック時に発生する例外への対応
        //public const string CheckerSoftVer = "1.30";   //リトライ処理変更（３回までは自動で行う）
        //public const string CheckerSoftVer = "1.40";    //ファームウェア書き込みの判定処理変更
                                                        //（アプリケーションウィンドウに直接メッセージを送り、パネルに表示されるログデータを取得・解析）
                                                        //S1チェック方法を変更（奇数ON→全部ON→全部OFF）
                                                        //LED色判定をrgb平均からhsvのH平均に変更
        //public const string CheckerSoftVer = "1.50"; //2014.9.18 試験合格品を再試験できないように変更（QK対策）
        //public const string CheckerSoftVer = "1.60"; //2014.11.13 立ち上げ時に再試験禁止の警告画面表示を追加（QK対策）
        public const string CheckerSoftVer = "1.70"; //2015.3.19 マルチメータを34401AとR6441B両方に対応（フォームロード時に自動で検出）
        //音声データのパス
        public const string WavDataPath = @"C:\DataLog\検査ソフト\Host\Wav\";
        public const string PassSound = WavDataPath + "Pass.wav";
        public const string FailSound = WavDataPath + "Fail.wav";
        public const string NoticeSound = WavDataPath + "Notice.wav";
        public const string OpeCodeSound = WavDataPath + "Opecode.wav";
        public const string OperatorSound = WavDataPath + "Operator.wav";
        public const string ModelSound = WavDataPath + "Model.wav";
        public const string S1Sound = WavDataPath + "Sw1.wav";
        public const string S2Sound = WavDataPath + "Sw2.wav";
        public const string S1FSound = WavDataPath + "Sw1F.wav";
        public const string S2FSound = WavDataPath + "Sw2F.wav";
        public const string DipSwOnSound = WavDataPath + "DipSwOn.wav";
        public const string DipSwOffSound = WavDataPath + "DipSwOff.wav";
        public const string OnSound = WavDataPath + "On.wav";
        public const string OffSound = WavDataPath + "Off.wav";
        public const string EndSound = WavDataPath + "End.wav";
        public const string AllOffSound = WavDataPath + "AllOff.wav";

        //画像データのパス
        public const string BmpDataPath = @"C:\DataLog\検査ソフト\Host\Bmp\";
        public const string DipOffPic = BmpDataPath + "DipOff.bmp";
        public const string DipOnPic = BmpDataPath + "DipOn.bmp";

        public const string SdefaultPic = BmpDataPath + "Sdefault.bmp";
        public const string S0Pic = BmpDataPath + "S0.bmp";
        public const string S1Pic = BmpDataPath + "S1.bmp";
        public const string S2Pic = BmpDataPath + "S2.bmp";
        public const string S3Pic = BmpDataPath + "S3.bmp";
        public const string S4Pic = BmpDataPath + "S4.bmp";
        public const string S5Pic = BmpDataPath + "S5.bmp";
        public const string S6Pic = BmpDataPath + "S6.bmp";
        public const string S7Pic = BmpDataPath + "S7.bmp";
        public const string S8Pic = BmpDataPath + "S8.bmp";
        public const string S9Pic = BmpDataPath + "S9.bmp";
        public const string SaPic = BmpDataPath + "Sa.bmp";
        public const string SbPic = BmpDataPath + "Sb.bmp";
        public const string ScPic = BmpDataPath + "Sc.bmp";
        public const string SdPic = BmpDataPath + "Sd.bmp";
        public const string SePic = BmpDataPath + "Se.bmp";
        public const string SfPic = BmpDataPath + "Sf.bmp";

        //パラメータファイルのパス
        public const string ParameterFilePath = @"C:\DataLog\検査ソフト\Host\parameter.ods";
        //オフセット用画像ファイルのパス
        public const string OffsetPictureFilePath = @"C:\DataLog\画像検査\OffsetSample.BMP";
        //検査データファイルのパス
        public const string TestDataPath = @"C:\DataLog\検査データ\";
        //製品ソフトウェア保存フォルダのパス
        public const string FirmPath = @"C:\DataLog\製品Firmware\";
        //試験用ソフトウェア（Target）のファイルパス
        public const string TestFirmPath = @"C:\DataLog\検査ソフト\Target\sdfunc07.mot";
        //FDTワークスペースのありか
        public const string FdtWorkSpacePath = @"C:\DataLog\FirmWriter\FirmWriter.AWS";

        //作業者へのメッセージ
        public const string MessOperator = "作業者名を選択してください";
        public const string MessModel = "バーコードリーダーで型番を読み取ってください";
        public const string MessOpeCode = "バーコードリーダーで工番を読み取ってください";
        public const string MessSet = "製品をセットして、開始ボタンを押してください";
        public const string MessRemove = "製品を取り外して、確認ボタンを押してください";
        public const string MessWait = "しばらくお待ちください・・・・・";

        //画像検査用定数
        public const float BinaryThreshold = 0.35F;//グレースケール→二値化へ変換する際の閾値
        public const float BinaryThreshold2 = 0.1F;//グレースケール→二値化へ変換する際の閾値(DI)
        public const int 検査するピクセル数  = 400;//(縦20*横20ピクセル)
        public const int LedThreshold = 280;      //LED(DI以外)検査時の閾値(20*20 = 400ピクセルのうち300ピクセルが白だったら合格)
        public const int LedThreshold2 = 230;     //LED(DI)検査時の閾値(20*20 = 400ピクセルのうち300ピクセルが白だったら合格)
        //リアルタイムクロック検査時にターゲットに設定する初期値
        public const string DefaultTime = "/13/01/01/01/01/00";


    
    
    }
}
