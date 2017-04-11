using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;

namespace DataLogTester
{
    public partial class CameraOffsetForm : Form
    {

        private bool gbolFlgC;  //クロスカーソル表示ON/OFF
        private int intXC;      //クロスカーソルの表示位置
        private int intYC;
        private int intW;       //イメージのサイズ
        private int intH;

        string buffx = null;
        string buffy = null;

        private Bitmap bitm;    //イメージのドットカラー取得用
        private Graphics gra;   //イメージのクロスカーソル





        //定数の宣言
        public enum ErrorCode
        {
            Load_NORMAL,
            Load_ERROR
        }

        public CameraOffsetForm()
        {

            InitializeComponent();

            //

            labelMessage.Text = null;

            // FileStream を開く
            System.IO.FileStream hStream = new System.IO.FileStream(Constants.OffsetPictureFilePath, System.IO.FileMode.Open);

            // FileStream から画像を読み込んで表示
            pictureBox1.Image = Image.FromStream(hStream);

            // FileStream を閉じる (正しくは オブジェクトの破棄を保証する を参照)
            hStream.Close();

            intW = pictureBox1.Width;
            intH = pictureBox1.Height;

            bitm = new Bitmap(pictureBox1.Image);
            gbolFlgC = false;     //クロスカーソル非表示

        }

        //フォームロード時の処理
        private void CameraOffsetForm_Load(object sender, EventArgs e)
        {
            SetLedPoint();//パラメータファイルから取り込んだLED座標をセットする
        }

        //フォーム閉じる前の処理
        private void FormOffset_FormClosed(object sender, FormClosedEventArgs e)
        {

        }


        //クロスカーソルの表示
        // x,y:クロスカーソルの表示位置
        // flg:クロスカーソルを表示（true）／消す（false）
        private void CrossCursor(int x, int y, bool flg = true)
        {
            //ドットでクロスカーソルの表示
            for (int i = 0; i < 11; i++)
            {
                fncPicelXor(x + i, y, flg);
            }

            for (int i = 1; i < 11; i++)
            {
                fncPicelXor(x - i, y, flg);
            }


            for (int i = 0; i < 11; i++)
            {
                fncPicelXor(x, y + i, flg);
            }

            for (int i = 0; i < 11; i++)
            {
                fncPicelXor(x, y - i, flg);
            }

            // //四角の表示
            // for (int i = -intD; i <= intD; i++)
            // {
            //     fncPicelXor(x + i, y - intD, flg);
            //     fncPicelXor(x + i, y + intD, flg);
            //     fncPicelXor(x + intD, y + i, flg);
            //     fncPicelXor(x - intD, y + i, flg);
            //}
        }
        //ピクセルの色の変更
        // x,y:ピクセル位置
        // flg:色を反転（true）／色を戻す（false）
        private void fncPicelXor(int x, int y, bool flg)
        {
            if (x < 0 || y < 0 || x >= intW || y >= intH) { return; }  //領域外
            int P = bitm.GetPixel(x, y).ToArgb();    // ピクセルデータの取得
            if (flg) { P ^= 0xffffff; }    // 反転色の計算
            Pen pn = new Pen(Color.FromArgb(P), 0.2F);
            gra.DrawRectangle(pn, x, y, 0.2F, 0.2F);
            pn.Dispose();
        }


        //イベント登録済み
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //イメージ上のカーソル位置
            int x0 = pictureBox1.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y0 = pictureBox1.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            //マウス位置を表示
            buffx = x0.ToString();
            buffy = y0.ToString();
        }

        private void buttonOffset_Click(object sender, EventArgs e)
        {

            //LED座標の調整は左上（RX1）→右下（SD_DO1）に向かって順に行う

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxRx1X.Focus();
            labelMessage.Text = "RX1の座標をクリックしてください";
            textBoxRx1X.BackColor = Color.SkyBlue;
            textBoxRx1Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxRx1X.Text = buffx;
            textBoxRx1Y.Text = buffy;
            textBoxRx1X.BackColor = Color.White;
            textBoxRx1Y.BackColor = Color.White;

            //*************************************************            
            buffx = null;
            buffy = null;
            textBoxTx1X.Focus();
            labelMessage.Text = "TX1の座標をクリックしてください";
            textBoxTx1X.BackColor = Color.SkyBlue;
            textBoxTx1Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxTx1X.Text = buffx;
            textBoxTx1Y.Text = buffy;
            textBoxTx1X.BackColor = Color.White;
            textBoxTx1Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxTx3X.Focus();
            labelMessage.Text = "TX3の座標をクリックしてください";
            textBoxTx3X.BackColor = Color.SkyBlue;
            textBoxTx3Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxTx3X.Text = buffx;
            textBoxTx3Y.Text = buffy;
            textBoxTx3X.BackColor = Color.White;
            textBoxTx3Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxRx3X.Focus();
            labelMessage.Text = "RX3の座標をクリックしてください";
            textBoxRx3X.BackColor = Color.SkyBlue;
            textBoxRx3Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxRx3X.Text = buffx;
            textBoxRx3Y.Text = buffy;
            textBoxRx3X.BackColor = Color.White;
            textBoxRx3Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxTx2X.Focus();
            labelMessage.Text = "TX2の座標をクリックしてください";
            textBoxTx2X.BackColor = Color.SkyBlue;
            textBoxTx2Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxTx2X.Text = buffx;
            textBoxTx2Y.Text = buffy;
            textBoxTx2X.BackColor = Color.White;
            textBoxTx2Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxRx2X.Focus();
            labelMessage.Text = "RX2の座標をクリックしてください";
            textBoxRx2X.BackColor = Color.SkyBlue;
            textBoxRx2Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxRx2X.Text = buffx;
            textBoxRx2Y.Text = buffy;
            textBoxRx2X.BackColor = Color.White;
            textBoxRx2Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxVccX.Focus();
            labelMessage.Text = "VCC1の座標をクリックしてください";
            textBoxVccX.BackColor = Color.SkyBlue;
            textBoxVccY.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxVccX.Text = buffx;
            textBoxVccY.Text = buffy;
            textBoxVccX.BackColor = Color.White;
            textBoxVccY.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxRunX.Focus();
            labelMessage.Text = "DEB1の座標をクリックしてください";
            textBoxRunX.BackColor = Color.SkyBlue;
            textBoxRunY.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxRunX.Text = buffx;
            textBoxRunY.Text = buffy;
            textBoxRunX.BackColor = Color.White;
            textBoxRunY.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxCpuX.Focus();
            labelMessage.Text = "CPU1の座標をクリックしてください";
            textBoxCpuX.BackColor = Color.SkyBlue;
            textBoxCpuY.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxCpuX.Text = buffx;
            textBoxCpuY.Text = buffy;
            textBoxCpuX.BackColor = Color.White;
            textBoxCpuY.BackColor = Color.White;


            //*************************************************
            buffx = null;
            buffy = null;
            textBoxTx4X.Focus();
            labelMessage.Text = "TX4の座標をクリックしてください";
            textBoxTx4X.BackColor = Color.SkyBlue;
            textBoxTx4Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxTx4X.Text = buffx;
            textBoxTx4Y.Text = buffy;
            textBoxTx4X.BackColor = Color.White;
            textBoxTx4Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxRx4X.Focus();
            labelMessage.Text = "RX4の座標をクリックしてください";
            textBoxRx4X.BackColor = Color.SkyBlue;
            textBoxRx4Y.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxRx4X.Text = buffx;
            textBoxRx4Y.Text = buffy;
            textBoxRx4X.BackColor = Color.White;
            textBoxRx4Y.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxSdDiX.Focus();
            labelMessage.Text = "SD_DI1の座標をクリックしてください";
            textBoxSdDiX.BackColor = Color.SkyBlue;
            textBoxSdDiY.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxSdDiX.Text = buffx;
            textBoxSdDiY.Text = buffy;
            textBoxSdDiX.BackColor = Color.White;
            textBoxSdDiY.BackColor = Color.White;

            //*************************************************
            buffx = null;
            buffy = null;
            textBoxSdDoX.Focus();
            labelMessage.Text = "SD_DO1の座標をクリックしてください";
            textBoxSdDoX.BackColor = Color.SkyBlue;
            textBoxSdDoY.BackColor = Color.SkyBlue;

            do
            {
                System.Windows.Forms.Application.DoEvents();

            } while (buffx == null || buffy == null);
            textBoxSdDoX.Text = buffx;
            textBoxSdDoY.Text = buffy;
            textBoxSdDoX.BackColor = Color.White;
            textBoxSdDoY.BackColor = Color.White;




            labelMessage.Text = null;
            MessageBox.Show("オフセット完了しました！");

        }

        //ラジオボタンClickイベント
        private void radioButtonTx1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx2_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx3_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx4_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx2_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx3_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx4_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonCpu1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonDeb1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonVcc1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonSdDi1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonSdDo1_Click(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        //ラジオボタンCheckedChangedイベント
        private void radioButtonTx1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx2_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx3_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonTx4_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx2_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx3_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonRx4_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonVcc1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonDeb1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonCpu1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonSdDi1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        private void radioButtonSdDo1_CheckedChanged(object sender, EventArgs e)
        {
            ShowCrossCursor();
        }

        //ラジオボタンPreviewKeyDownイベント
        private void radioButtonTx1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonTx2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonTx3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonTx4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonRx1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonRx2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonRx3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonRx4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonVcc1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonDeb1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonCpu1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonSdDi1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        private void radioButtonSdDo1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            MoveCursor(e);
        }

        //ラジオボタンKeyDownイベント
        private void radioButtonRx1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonTx1.Focus();
            }
        }

        private void radioButtonTx1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonTx3.Focus();
            }
        }

        private void radioButtonTx3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonRx3.Focus();
            }
        }

        private void radioButtonRx3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonTx2.Focus();
            }
        }

        private void radioButtonTx2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonRx2.Focus();
            }
        }

        private void radioButtonRx2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonVcc1.Focus();
            }
        }

        private void radioButtonVcc1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonDeb1.Focus();
            }
        }

        private void radioButtonDeb1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonCpu1.Focus();
            }
        }

        private void radioButtonCpu1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonTx4.Focus();
            }
        }

        private void radioButtonTx4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonRx4.Focus();
            }
        }

        private void radioButtonRx4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonSdDi1.Focus();
            }
        }

        private void radioButtonSdDi1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonSdDo1.Focus();
            }
        }

        private void radioButtonSdDo1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                radioButtonRx1.Focus();
            }
        }

        //ラジオボタンEnterイベント  
        private void radioButtonRx1_Enter(object sender, EventArgs e)
        {
            radioButtonRx1.Checked = true;
        }

        private void radioButtonTx1_Enter(object sender, EventArgs e)
        {
            radioButtonTx1.Checked = true;
        }

        private void radioButtonTx3_Enter(object sender, EventArgs e)
        {
            radioButtonTx3.Checked = true;
        }

        private void radioButtonRx3_Enter(object sender, EventArgs e)
        {
            radioButtonRx3.Checked = true;
        }

        private void radioButtonTx2_Enter(object sender, EventArgs e)
        {
            radioButtonTx2.Checked = true;
        }

        private void radioButtonRx2_Enter(object sender, EventArgs e)
        {
            radioButtonRx2.Checked = true;
        }

        private void radioButtonVcc1_Enter(object sender, EventArgs e)
        {
            radioButtonVcc1.Checked = true;
        }

        private void radioButtonDeb1_Enter(object sender, EventArgs e)
        {
            radioButtonDeb1.Checked = true;
        }

        private void radioButtonCpu1_Enter(object sender, EventArgs e)
        {
            radioButtonCpu1.Checked = true;
        }

        private void radioButtonTx4_Enter(object sender, EventArgs e)
        {
            radioButtonTx4.Checked = true;
        }

        private void radioButtonRx4_Enter(object sender, EventArgs e)
        {
            radioButtonRx4.Checked = true;
        }

        private void radioButtonSdDi1_Enter(object sender, EventArgs e)
        {
            radioButtonSdDi1.Checked = true;
        }

        private void radioButtonSdDo1_Enter(object sender, EventArgs e)
        {
            radioButtonSdDo1.Checked = true;
        }



        private void buttonSave_Click(object sender, EventArgs e)
        {
            buttonOffset.Enabled = false;
            buttonSave.Enabled = false;
            groupBox2.Enabled = false;

            try
            {

                State.cam0Prop.TX1 = textBoxTx1X.Text + "," + textBoxTx1Y.Text;
                State.cam0Prop.TX2 = textBoxTx2X.Text + "," + textBoxTx2Y.Text;
                State.cam0Prop.TX3 = textBoxTx3X.Text + "," + textBoxTx3Y.Text;
                State.cam0Prop.TX4 = textBoxTx4X.Text + "," + textBoxTx4Y.Text;

                State.cam0Prop.RX1 = textBoxRx1X.Text + "," + textBoxRx1Y.Text;
                State.cam0Prop.RX2 = textBoxRx2X.Text + "," + textBoxRx2Y.Text;
                State.cam0Prop.RX3 = textBoxRx3X.Text + "," + textBoxRx3Y.Text;
                State.cam0Prop.RX4 = textBoxRx4X.Text + "," + textBoxRx4Y.Text;

                State.cam0Prop.VCC = textBoxVccX.Text + "," + textBoxVccY.Text;
                State.cam0Prop.RUN = textBoxRunX.Text + "," + textBoxRunY.Text;
                State.cam0Prop.CPU = textBoxCpuX.Text + "," + textBoxCpuY.Text;
                State.cam0Prop.SD_DI = textBoxSdDiX.Text + "," + textBoxSdDiY.Text;
                State.cam0Prop.SD_DO = textBoxSdDoX.Text + "," + textBoxSdDoY.Text;

                MessageBox.Show("LED座標データが保存されました");

            }
            catch
            {
                MessageBox.Show("LED座標データの保存に失敗しました2");
            }

            this.Close();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (gbolFlgC == true)
            {   //前回のクロスカーソルを消す
                this.Text = "CrossCursor";
                gra = Graphics.FromImage(pictureBox1.Image);
                CrossCursor(intXC, intYC, false);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
                gbolFlgC = false;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //イメージ上のカーソル位置
            int x0 = pictureBox1.PointToClient(System.Windows.Forms.Cursor.Position).X;
            int y0 = pictureBox1.PointToClient(System.Windows.Forms.Cursor.Position).Y;
            //マウス位置を表示
            this.Text = "x,y=" + x0.ToString() + "/" + y0.ToString();


            //gra = Graphics.FromImage(pictureBox1.Image);
            //if (gbolFlgC == true)
            //{   //前回のクロスカーソルを消す
            //    CrossCursor(intXC, intYC, false);
            //}

            //gbolFlgC = true;  //クロスカーソルを表示
            //intXC = x0;
            //intYC = y0;
            //CrossCursor(intXC, intYC);
            //gra.Dispose();
            //pictureBox1.Refresh();  //途中表示
        }

        private static Point makePoint(string data)
        {

            var pointArr = data.Split(',').ToArray();
            int x = Int32.Parse(pointArr[0]);
            int y = Int32.Parse(pointArr[1]);
            return new Point(x, y);

        }

        private void SetLedPoint()
        {

            textBoxRx1X.Text = makePoint(State.cam0Prop.RX1).X.ToString();
            textBoxRx1Y.Text = makePoint(State.cam0Prop.RX1).Y.ToString();
            textBoxRx2X.Text = makePoint(State.cam0Prop.RX2).X.ToString();
            textBoxRx2Y.Text = makePoint(State.cam0Prop.RX2).Y.ToString();
            textBoxRx3X.Text = makePoint(State.cam0Prop.RX3).X.ToString();
            textBoxRx3Y.Text = makePoint(State.cam0Prop.RX3).Y.ToString();
            textBoxRx4X.Text = makePoint(State.cam0Prop.RX4).X.ToString();
            textBoxRx4Y.Text = makePoint(State.cam0Prop.RX4).Y.ToString();

            textBoxTx1X.Text = makePoint(State.cam0Prop.TX1).X.ToString();
            textBoxTx1Y.Text = makePoint(State.cam0Prop.TX1).Y.ToString();
            textBoxTx2X.Text = makePoint(State.cam0Prop.TX2).X.ToString();
            textBoxTx2Y.Text = makePoint(State.cam0Prop.TX2).Y.ToString();
            textBoxTx3X.Text = makePoint(State.cam0Prop.TX3).X.ToString();
            textBoxTx3Y.Text = makePoint(State.cam0Prop.TX3).Y.ToString();
            textBoxTx4X.Text = makePoint(State.cam0Prop.TX4).X.ToString();
            textBoxTx4Y.Text = makePoint(State.cam0Prop.TX4).Y.ToString();

            textBoxVccX.Text = makePoint(State.cam0Prop.VCC).X.ToString();
            textBoxVccY.Text = makePoint(State.cam0Prop.VCC).Y.ToString();

            textBoxRunX.Text = makePoint(State.cam0Prop.RUN).X.ToString();
            textBoxRunY.Text = makePoint(State.cam0Prop.RUN).Y.ToString();

            textBoxCpuX.Text = makePoint(State.cam0Prop.CPU).X.ToString();
            textBoxCpuY.Text = makePoint(State.cam0Prop.CPU).Y.ToString();

            textBoxSdDiX.Text = makePoint(State.cam0Prop.SD_DI).X.ToString();
            textBoxSdDiY.Text = makePoint(State.cam0Prop.SD_DI).Y.ToString();

            textBoxSdDoX.Text = makePoint(State.cam0Prop.SD_DO).X.ToString();
            textBoxSdDoY.Text = makePoint(State.cam0Prop.SD_DO).Y.ToString();


        }


        //十字キーボタンを押したときの処理
        private void buttonUp_Click(object sender, EventArgs e)
        {

            if (radioButtonTx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx1Y.Text = (Int32.Parse(textBoxTx1Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx1X.Text);
                intYC = Int32.Parse(textBoxTx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonTx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx2Y.Text = (Int32.Parse(textBoxTx2Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx2X.Text);
                intYC = Int32.Parse(textBoxTx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx3Y.Text = (Int32.Parse(textBoxTx3Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx3X.Text);
                intYC = Int32.Parse(textBoxTx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx4Y.Text = (Int32.Parse(textBoxTx4Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx4X.Text);
                intYC = Int32.Parse(textBoxTx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx1Y.Text = (Int32.Parse(textBoxRx1Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx1X.Text);
                intYC = Int32.Parse(textBoxRx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx2Y.Text = (Int32.Parse(textBoxRx2Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx2X.Text);
                intYC = Int32.Parse(textBoxRx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx3Y.Text = (Int32.Parse(textBoxRx3Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx3X.Text);
                intYC = Int32.Parse(textBoxRx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx4Y.Text = (Int32.Parse(textBoxRx4Y.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx4X.Text);
                intYC = Int32.Parse(textBoxRx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonCpu1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxCpuY.Text = (Int32.Parse(textBoxCpuY.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxCpuX.Text);
                intYC = Int32.Parse(textBoxCpuY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonDeb1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxRunY.Text = (Int32.Parse(textBoxRunY.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRunX.Text);
                intYC = Int32.Parse(textBoxRunY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonVcc1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxVccY.Text = (Int32.Parse(textBoxVccY.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxVccX.Text);
                intYC = Int32.Parse(textBoxVccY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonSdDi1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);
                textBoxSdDiY.Text = (Int32.Parse(textBoxSdDiY.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDiX.Text);
                intYC = Int32.Parse(textBoxSdDiY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonSdDo1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);
                textBoxSdDoY.Text = (Int32.Parse(textBoxSdDoY.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDoX.Text);
                intYC = Int32.Parse(textBoxSdDoY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {

            if (radioButtonTx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx1Y.Text = (Int32.Parse(textBoxTx1Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx1X.Text);
                intYC = Int32.Parse(textBoxTx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonTx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx2Y.Text = (Int32.Parse(textBoxTx2Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx2X.Text);
                intYC = Int32.Parse(textBoxTx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx3Y.Text = (Int32.Parse(textBoxTx3Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx3X.Text);
                intYC = Int32.Parse(textBoxTx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx4Y.Text = (Int32.Parse(textBoxTx4Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx4X.Text);
                intYC = Int32.Parse(textBoxTx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx1Y.Text = (Int32.Parse(textBoxRx1Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx1X.Text);
                intYC = Int32.Parse(textBoxRx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx2Y.Text = (Int32.Parse(textBoxRx2Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx2X.Text);
                intYC = Int32.Parse(textBoxRx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx3Y.Text = (Int32.Parse(textBoxRx3Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx3X.Text);
                intYC = Int32.Parse(textBoxRx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx4Y.Text = (Int32.Parse(textBoxRx4Y.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx4X.Text);
                intYC = Int32.Parse(textBoxRx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonCpu1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxCpuY.Text = (Int32.Parse(textBoxCpuY.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxCpuX.Text);
                intYC = Int32.Parse(textBoxCpuY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonDeb1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRunY.Text = (Int32.Parse(textBoxRunY.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRunX.Text);
                intYC = Int32.Parse(textBoxRunY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonVcc1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxVccY.Text = (Int32.Parse(textBoxVccY.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxVccX.Text);
                intYC = Int32.Parse(textBoxVccY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDi1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxSdDiY.Text = (Int32.Parse(textBoxSdDiY.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDiX.Text);
                intYC = Int32.Parse(textBoxSdDiY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDo1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxSdDoY.Text = (Int32.Parse(textBoxSdDoY.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDoX.Text);
                intYC = Int32.Parse(textBoxSdDoY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {

            if (radioButtonTx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx1X.Text = (Int32.Parse(textBoxTx1X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx1X.Text);
                intYC = Int32.Parse(textBoxTx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonTx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx2X.Text = (Int32.Parse(textBoxTx2X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx2X.Text);
                intYC = Int32.Parse(textBoxTx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx3X.Text = (Int32.Parse(textBoxTx3X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx3X.Text);
                intYC = Int32.Parse(textBoxTx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx4X.Text = (Int32.Parse(textBoxTx4X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx4X.Text);
                intYC = Int32.Parse(textBoxTx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx1X.Text = (Int32.Parse(textBoxRx1X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx1X.Text);
                intYC = Int32.Parse(textBoxRx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx2X.Text = (Int32.Parse(textBoxRx2X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx2X.Text);
                intYC = Int32.Parse(textBoxRx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx3X.Text = (Int32.Parse(textBoxRx3X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx3X.Text);
                intYC = Int32.Parse(textBoxRx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx4X.Text = (Int32.Parse(textBoxRx4X.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx4X.Text);
                intYC = Int32.Parse(textBoxRx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonCpu1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxCpuX.Text = (Int32.Parse(textBoxCpuX.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxCpuX.Text);
                intYC = Int32.Parse(textBoxCpuY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonDeb1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRunX.Text = (Int32.Parse(textBoxRunX.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRunX.Text);
                intYC = Int32.Parse(textBoxRunY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonVcc1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxVccX.Text = (Int32.Parse(textBoxVccX.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxVccX.Text);
                intYC = Int32.Parse(textBoxVccY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDi1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxSdDiX.Text = (Int32.Parse(textBoxSdDiX.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDiX.Text);
                intYC = Int32.Parse(textBoxSdDiY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDo1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxSdDoX.Text = (Int32.Parse(textBoxSdDoX.Text) - 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDoX.Text);
                intYC = Int32.Parse(textBoxSdDoY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {

            if (radioButtonTx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx1X.Text = (Int32.Parse(textBoxTx1X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx1X.Text);
                intYC = Int32.Parse(textBoxTx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonTx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx2X.Text = (Int32.Parse(textBoxTx2X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx2X.Text);
                intYC = Int32.Parse(textBoxTx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx3X.Text = (Int32.Parse(textBoxTx3X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx3X.Text);
                intYC = Int32.Parse(textBoxTx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonTx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxTx4X.Text = (Int32.Parse(textBoxTx4X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxTx4X.Text);
                intYC = Int32.Parse(textBoxTx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx1X.Text = (Int32.Parse(textBoxRx1X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx1X.Text);
                intYC = Int32.Parse(textBoxRx1Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx2.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx2X.Text = (Int32.Parse(textBoxRx2X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx2X.Text);
                intYC = Int32.Parse(textBoxRx2Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonRx3.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx3X.Text = (Int32.Parse(textBoxRx3X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx3X.Text);
                intYC = Int32.Parse(textBoxRx3Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonRx4.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRx4X.Text = (Int32.Parse(textBoxRx4X.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRx4X.Text);
                intYC = Int32.Parse(textBoxRx4Y.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonCpu1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxCpuX.Text = (Int32.Parse(textBoxCpuX.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxCpuX.Text);
                intYC = Int32.Parse(textBoxCpuY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

            else if (radioButtonDeb1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxRunX.Text = (Int32.Parse(textBoxRunX.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxRunX.Text);
                intYC = Int32.Parse(textBoxRunY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonVcc1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);


                textBoxVccX.Text = (Int32.Parse(textBoxVccX.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxVccX.Text);
                intYC = Int32.Parse(textBoxVccY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDi1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxSdDiX.Text = (Int32.Parse(textBoxSdDiX.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDiX.Text);
                intYC = Int32.Parse(textBoxSdDiY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }
            else if (radioButtonSdDo1.Checked)
            {
                gra = Graphics.FromImage(pictureBox1.Image);

                //前回のクロスカーソルを消す
                CrossCursor(intXC, intYC, false);

                textBoxSdDoX.Text = (Int32.Parse(textBoxSdDoX.Text) + 1).ToString();
                gbolFlgC = true;  //クロスカーソルを表示
                intXC = Int32.Parse(textBoxSdDoX.Text);
                intYC = Int32.Parse(textBoxSdDoY.Text);
                CrossCursor(intXC, intYC);
                gra.Dispose();
                pictureBox1.Refresh();  //途中表示
            }

        }


        //クロスカーソルを表示する
        private void ShowCrossCursor()
        {
            gra = Graphics.FromImage(pictureBox1.Image);

            //前回のクロスカーソルを消す
            CrossCursor(intXC, intYC, false);

            if (radioButtonTx1.Checked)
            {
                intXC = Int32.Parse(textBoxTx1X.Text);
                intYC = Int32.Parse(textBoxTx1Y.Text);
            }

            if (radioButtonTx2.Checked)
            {
                intXC = Int32.Parse(textBoxTx2X.Text);
                intYC = Int32.Parse(textBoxTx2Y.Text);
            }

            if (radioButtonTx3.Checked)
            {
                intXC = Int32.Parse(textBoxTx3X.Text);
                intYC = Int32.Parse(textBoxTx3Y.Text);
            }

            if (radioButtonTx4.Checked)
            {
                intXC = Int32.Parse(textBoxTx4X.Text);
                intYC = Int32.Parse(textBoxTx4Y.Text);
            }

            if (radioButtonRx1.Checked)
            {
                intXC = Int32.Parse(textBoxRx1X.Text);
                intYC = Int32.Parse(textBoxRx1Y.Text);
            }

            if (radioButtonRx2.Checked)
            {
                intXC = Int32.Parse(textBoxRx2X.Text);
                intYC = Int32.Parse(textBoxRx2Y.Text);
            }

            if (radioButtonRx3.Checked)
            {
                intXC = Int32.Parse(textBoxRx3X.Text);
                intYC = Int32.Parse(textBoxRx3Y.Text);
            }

            if (radioButtonRx4.Checked)
            {
                intXC = Int32.Parse(textBoxRx4X.Text);
                intYC = Int32.Parse(textBoxRx4Y.Text);
            }

            if (radioButtonVcc1.Checked)
            {
                intXC = Int32.Parse(textBoxVccX.Text);
                intYC = Int32.Parse(textBoxVccY.Text);
            }

            if (radioButtonDeb1.Checked)
            {
                intXC = Int32.Parse(textBoxRunX.Text);
                intYC = Int32.Parse(textBoxRunY.Text);
            }

            if (radioButtonCpu1.Checked)
            {
                intXC = Int32.Parse(textBoxCpuX.Text);
                intYC = Int32.Parse(textBoxCpuY.Text);
            }

            if (radioButtonSdDi1.Checked)
            {
                intXC = Int32.Parse(textBoxSdDiX.Text);
                intYC = Int32.Parse(textBoxSdDiY.Text);
            }

            if (radioButtonSdDo1.Checked)
            {
                intXC = Int32.Parse(textBoxSdDoX.Text);
                intYC = Int32.Parse(textBoxSdDoY.Text);
            }


            gbolFlgC = true;  //クロスカーソルを表示

            CrossCursor(intXC, intYC);
            gra.Dispose();
            pictureBox1.Refresh();  //途中表示

        }

        //キーボードの十字キーを押したときにカーソルを移動させる
        private void MoveCursor(PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    e.IsInputKey = true;
                    buttonUp.PerformClick();
                    break;
                case Keys.Down:
                    e.IsInputKey = true;
                    buttonDown.PerformClick();
                    break;
                case Keys.Right:
                    e.IsInputKey = true;
                    buttonRight.PerformClick();
                    break;
                case Keys.Left:
                    e.IsInputKey = true;
                    buttonLeft.PerformClick();
                    break;
                case Keys.Tab:
                    e.IsInputKey = true;
                    break;
                default:
                    break;
            }
        }











    }
}
