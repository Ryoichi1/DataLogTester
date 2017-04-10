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
    public class LedProp
    {
        public LedName name;
        public int PointX;
        public int PointY;
        public int Red;
        public int Green;
        public int Blue;
        public float H;
        public Color Color;
        public bool FlagColor;
        public bool FlagCount;
    }

    public static class TEST_LED
    {
        private static int Offset = (int)Math.Sqrt(Constants.検査するピクセル数);
        public static List<LedProp> LedPropList;

        private static Bitmap 元画像;
        private static Bitmap 二値化画像DI;
        private static Bitmap 二値化画像DI以外;

        //静的コンストラクタ
        static TEST_LED()
        {}

        public static bool CheckLed(Action メインフォームの描画処理)
        {
            InitLedProp();

            //ターゲットにコマンド送信
            Target.sendData(Target.PortName.P1_232C, "LedCheck");//このコマンドのみ返信なし
            General.Wait(4500);//正常ならここで全点してるはず

            //全点灯画像の取得
            IntPtr m_ip = General.cam.Click();
            元画像 = new Bitmap(General.cam.Width, General.cam.Height, General.cam.Stride, PixelFormat.Format24bppRgb, m_ip);
            元画像.RotateFlip(RotateFlipType.RotateNoneFlipY); // 上下逆さになる事がある

            
            //LEDの色判定（2013.8.7追加）
            GetColor();

            //元画像の二値化
            Binarize(Constants.BinaryThreshold, ref 二値化画像DI以外);
            Binarize(Constants.BinaryThreshold2, ref 二値化画像DI);

            //LEDが点灯しているかのチェック
            GetPixel();

            //デバッグ用
            //二値化画像.Save(@"C:\DataLog\画像検査\nichika.bmp", ImageFormat.Bmp);

            //元画像に四角形描画
            SetRectangle();
            
            //表示用画像ファイルをbmpで保存する
            元画像.Save(@"C:\DataLog\画像検査\source.bmp", ImageFormat.Bmp);
            
            元画像.Dispose();
            二値化画像DI.Dispose();
            二値化画像DI以外.Dispose();
            メインフォームの描画処理();

            //フラグのチェック
            bool ColorFlag = LedPropList.All(c => c.FlagColor);
            bool LedFlag = LedPropList.All(c => c.FlagCount);

            return (ColorFlag && LedFlag);
        }

        //LEDプロパティのイニシャライズ（試験毎に行う）
        private static void InitLedProp()
        {
            LedPropList = new List<LedProp>();

            foreach (LedName name in Enum.GetValues(typeof(LedName)))
            {
                var prop = new LedProp();
                prop.name = name;
                switch (name)
                {
                    case LedName.TX1:
                        prop.PointX = State.Tx1_X - Offset / 2;
                        prop.PointY = State.Tx1_Y - Offset / 2;
                        break;
                    case LedName.TX2:
                        prop.PointX = State.Tx2_X - Offset / 2;
                        prop.PointY = State.Tx2_Y - Offset / 2;
                        break;
                    case LedName.TX3:
                        prop.PointX = State.Tx3_X - Offset / 2;
                        prop.PointY = State.Tx3_Y - Offset / 2;
                        break;
                    case LedName.TX4:
                        prop.PointX = State.Tx4_X - Offset / 2;
                        prop.PointY = State.Tx4_Y - Offset / 2;
                        break;
                    case LedName.RX1:
                        prop.PointX = State.Rx1_X - Offset / 2;
                        prop.PointY = State.Rx1_Y - Offset / 2;
                        break;
                    case LedName.RX2:
                        prop.PointX = State.Rx2_X - Offset / 2;
                        prop.PointY = State.Rx2_Y - Offset / 2;
                        break;
                    case LedName.RX3:
                        prop.PointX = State.Rx3_X - Offset / 2;
                        prop.PointY = State.Rx3_Y - Offset / 2;
                        break;
                    case LedName.RX4:
                        prop.PointX = State.Rx4_X - Offset / 2;
                        prop.PointY = State.Rx4_Y - Offset / 2;
                        break;
                    case LedName.VCC:
                        prop.PointX = State.Vcc_X - Offset / 2;
                        prop.PointY = State.Vcc_Y - Offset / 2;
                        break;
                    case LedName.RUN:
                        prop.PointX = State.Run_X - Offset / 2;
                        prop.PointY = State.Run_Y - Offset / 2;
                        break;
                    case LedName.CPU:
                        prop.PointX = State.Cpu_X - Offset / 2;
                        prop.PointY = State.Cpu_Y - Offset / 2;
                        break;
                    case LedName.DI:
                        prop.PointX = State.Di_X - Offset / 2;
                        prop.PointY = State.Di_Y - Offset / 2;
                        break;
                    case LedName.DO:
                        prop.PointX = State.Do_X - Offset / 2;
                        prop.PointY = State.Do_Y - Offset / 2;
                        break;
                }

                LedPropList.Add(prop);
            }
        }
        
        //画像を二値化する
        private static void Binarize(float 閾値, ref Bitmap 画像)
        {
            //引数でもらったBMPファイルを二値化する>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            //RGBの比率(YIQカラーモデル)
            const float r = 0.298912F;
            const float g = 0.586611F;
            const float b = 0.114478F;

            //ColorMatrixにセットする行列を 5 * 5 の配列で用意
            //入力のRGBの各チャンネルを重み付けをした上で、
            //出力するRGBがR = G = B となるような行列をセット
            float[][] matrixElement =
                                          {new float[]{r, r, r, 0, 0},
                                           new float[]{g, g, g, 0, 0},
                                           new float[]{b, b, b, 0, 0},
                                           new float[]{0, 0, 0, 1, 0},
                                           new float[]{0, 0, 0, 0, 1}};

            //ColorMatrixオブジェクトの作成
            ColorMatrix matrix = new ColorMatrix(matrixElement);

            //ImageAttributesにセット
            ImageAttributes attr = new ImageAttributes();
            attr.SetColorMatrix(matrix);

            //閾値の設定
            attr.SetThreshold(閾値);

            int imageWidth = 元画像.Width;
            int imageHeight = 元画像.Height;

            //新しいビットマップを用意
            画像 = new Bitmap(imageWidth, imageHeight);

            //新しいビットマップにImageAttributesを指定して
            //元のビットマップを描画
            Graphics graph = Graphics.FromImage(画像);
            graph.DrawImage(元画像,
                            new Rectangle(0, 0, imageWidth, imageHeight),
                            0, 0, imageWidth, imageHeight,
                            GraphicsUnit.Pixel,
                            attr);
            //この時点でrefで受け取った画像には元画像を閾値で二値化したものが入る
            graph.Dispose();   
        
        }

        //LED輝度を取得する
        private static void GetPixel()
        {
            foreach (LedProp prop in LedPropList)
            {
                var 二値化画像 = (prop.name == LedName.DI) ? 二値化画像DI : 二値化画像DI以外;
                //LED点灯しているかのチェック（検査するピクセル数の白色部分をカウントする）
                int count = 0;
                for (int i = 0; i < Offset; i++)
                {
                    for (int j = 0; j < Offset; j++)
                    {
                        Color c = 二値化画像.GetPixel(prop.PointX + j, prop.PointY + i);
                        if (c.R == 255 && c.G == 255 && c.B == 255) count++;//ピクセルが白だったらカウントをインクリメントする
                    }
                }
                prop.FlagCount = (count >= ((prop.name != LedName.DI) ? Constants.LedThreshold : Constants.LedThreshold2));
                //デバッグ用（DIの点灯カウントみる）
                //if (prop.name == LedName.DI)
                //{
                //    int cnt = count;
                //}
            }
        }

        //LEDカラーを取得する
        private static void GetColor()
        {
            foreach(var prop in LedPropList)
            {
                prop.Red = 0;
                prop.Green = 0;
                prop.Blue = 0;
                prop.H = 0;
                int totalCnt = 0;

                //LEDの点灯色を取得（検査するピクセル数の色を取得）
                for (int i = 0; i < Offset; i++)
                {
                    for (int j = 0; j < Offset; j++)
                    {
                        Color c = 元画像.GetPixel(prop.PointX + j, prop.PointY + i);
                        if (c.R > 240 && c.G > 240 && c.B > 240) continue;//白っぽかったら取り込まない
                        prop.Red += c.R;
                        prop.Green += c.G;
                        prop.Blue += c.B;
                        prop.H += HsvColor.FromRgb(c).H;
                        totalCnt++;
                    }
                }
                if (totalCnt == 0)
                {
                    prop.Color = Color.FromArgb(0,0,0);
                    prop.FlagColor = false;
                    return;
                }
                prop.Red /= totalCnt;
                prop.Green /= totalCnt;
                prop.Blue /= totalCnt;
                prop.H /= totalCnt;

                prop.Color = Color.FromArgb(prop.Red, prop.Green, prop.Blue);
                prop.FlagColor = ((prop.H >= State.Led_H_Min) && (prop.H <= State.Led_H_Max)); 
            }
        }

        //各LEDの画像検査枠の表示
        private static void SetRectangle()
        {
            //四角形の表示
            // x,y：表示位置の中心
            // flg:四角形の色  緑（true）／赤（false）
            Graphics graph = Graphics.FromImage(元画像);
            Pen pn;

            foreach (LedProp prop in LedPropList)
            {
                if (prop.FlagCount)
                {
                    pn = new Pen(Color.Orange, 4);//ＯＫの枠色はオレンジ

                }
                else
                {
                    pn = new Pen(Color.Red, 4);   //ＮＧの枠色は赤
                }
                //x,y座標から-5オフセット位置を頂点として右下方向に10×10ピクセルの四角形を描画
                graph.DrawRectangle(pn, prop.PointX, prop.PointY, 20, 20);
                pn.Dispose();
            }
            graph.Dispose();

        }

        public static void GetOffsetPic()
        {
            //静止画像の取得（bmp形式）
            IntPtr m_ip = General.cam.Click();
            元画像 = new Bitmap(General.cam.Width, General.cam.Height, General.cam.Stride, PixelFormat.Format24bppRgb, m_ip);
            元画像.RotateFlip(RotateFlipType.RotateNoneFlipY); // 上下逆さになる事がある

            //元画像の二値化
            Binarize(Constants.BinaryThreshold, ref 二値化画像DI以外);

            //表示用画像ファイルをbmpで保存する
            二値化画像DI以外.Save(Constants.OffsetPictureFilePath, ImageFormat.Bmp);
        
        }




    }
}
