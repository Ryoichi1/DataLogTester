using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using DirectShowLib;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Threading;


namespace DataLogTester
{

    public partial class MainForm : Form
    {
        //各種フラグ
        private bool FlagOpeName;   //作業者名が正しく選択されていればTrue
        private bool FlagOpeCode;    //工番が正しく入力されていればTrue
        private bool FlagModel;      //型番が正しく入力されていればTrue


        //インスタンス用変数


        System.Diagnostics.Stopwatch sw;

        Action<string> 通信ログ書き込み;

        //インスタンスコンストラクタ
        public MainForm()
        {
            InitializeComponent();

            //試験用パラメータのロード
            State.LoadConfigData();

            //Targetクラスに渡す
            通信ログ書き込み = this.SetCommLog;
        }


        //フォームイベント**********************************************************************************
        //フォームロード
        private void Form1_Load(object sender, EventArgs e)
        {

            //IOボードの初期化
            foreach (int i in Enumerable.Range(0, 10))
            {
                if (General.io.InitEpx64S(0x07)) break; //0x07→ P7:0 P6:0 P5:0 P4:0 P3:0 P2:1 P1:1 P0:1              
                if (i == 9)
                {
                    MessageBox.Show("IOボード初期化異常\r\n" + "アプリケーションを終了します");
                    Environment.Exit(0);
                }
                General.Wait(1000);
            }

            //IOボードのリセット
            General.ResetIo();

            //マルチメータの初期化
            //マルチメータ機種判別（34401 or R6441b）

            if (R6441b.InitR6441b(R6441b.ComNumber.COM1))
            {
                State.ItemMultimeter = Multimeter.R6441b;
                labelR6441b.BackColor = Color.MediumSeaGreen;
            }
            else
            {
                R6441b.ClosePort();
                if (Agilent34401A.Init34401A(Agilent34401A.ComNumber.COM1))
                {
                    State.ItemMultimeter = Multimeter.AGI34401A;
                    labelAgi34401a.BackColor = Color.MediumSeaGreen;
                }
                else
                {
                    MessageBox.Show("マルチメータ初期化異常\r\n" + "アプリケーションを終了します");
                    Environment.Exit(0);
                }

            }

            //ターゲットの初期化
            Target.InitTarget(() =>
            {

                //デリゲートのセット
                Target.Process1 = 通信ログ書き込み;

                //通信ポートの設定
                Target.Port1_232c.PortName = "COM2";
                Target.Port2_232c.PortName = "COM3";
                Target.Port3_232c.PortName = "COM4";
                Target.Port4_232c.PortName = "COM5";
                Target.Port1_422.PortName = "COM6";
                Target.Port2_422.PortName = "COM7";
                Target.Port3_422.PortName = "COM8";
                Target.Port4_422.PortName = "COM9";

            });

            //ターゲット通信設定の初期化
            if (!Target.InitPort())
            {
                Environment.Exit(0);
            }

            //USBカメラ関連のイニシャライズ
            try
            {
                foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                {
                    comboBoxDevices.Items.Add(ds.Name);
                }
                comboBoxDevices.SelectedIndex = 0;
                General.cam = new Capture(0, 640, 480, 30, panelPreview); // device, x, y, 24FPS, preview
            }
            catch (Exception ex)
            {
                //Console.Error.WriteLine(ex.Message);
            }



            //TODO: どうする？？
            ////検査用パラメータのセット
            //if (!State.SetSpec())
            //{
            //    MessageBox.Show("パラメータセット異常\r\n" + "アプリケーションを終了します");
            //    Environment.Exit(0);
            //}

            //Fdtのイニシャライズ
            FlashDevelopmentToolkit.InitFdt(() =>
            {
                FlashDevelopmentToolkit.WorkSpacePath = Constants.FdtWorkSpacePath;
                FlashDevelopmentToolkit.TestFilePath = Constants.TestFirmPath;
                FlashDevelopmentToolkit.ProductFilePath = Constants.FirmPath + State.TestSpec.FirmName;
                FlashDevelopmentToolkit.CheckSum = State.TestSpec.FirmSum;
            });

            //Stopwatchオブジェクトを作成する
            sw = new System.Diagnostics.Stopwatch();




            //メインフォームの初期化
            InitForm();

            //タイマーの初期化
            timerCurrentTime.Interval = 30;
            timerCurrentTime.Start();

            timerLbMessage.Interval = 1000;
            timerLbMessage.Start();

            timerTextInput.Interval = 500;
            timerTextInput.Stop();

            SetFocus();


        }
        //フォームクローズ
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (General.cam != null) General.cam.Dispose();

            Target.ClosePort();

            General.ResetIo();
            General.io.Close();   // Device Close

            Agilent34401A.ClosePort();
            R6441b.ClosePort();

            State.Save個別データ();
        }

        //画像検査枠　イベントいろいろ**********************************************************************************
        private void comboBoxDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (General.cam != null) General.cam.Dispose();
                General.cam = new Capture(comboBoxDevices.SelectedIndex, 640, 480, 30, panelPreview);
                //General.cam = new Capture(comboBoxDevices.SelectedIndex, 1280, 720, 10, panelPreview);
            }
            catch (Exception)
            {
                //Console.Error.WriteLine(ex.Message);
            }
        }
        private void panelPreview_Click(object sender, EventArgs e)
        {

            try
            {
                if (General.cam != null) General.cam.Dispose();

                comboBoxDevices.Items.Clear();
                foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                {
                    comboBoxDevices.Items.Add(ds.Name);
                }
                comboBoxDevices.SelectedIndex = 0;
                General.cam = new Capture(1, 640, 480, 30, panelPreview);

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                //Console.Error.WriteLine(ex.Message);
            }
        }

        //クリアボタンを押したときの処理**********************************************************************************
        private void buttonClearOperator_Click(object sender, EventArgs e)
        {
            comboBoxOperatorName.SelectedIndex = -1;//コンボボックスを空白にする
            作業者名 = false;
        }
        private void buttonClearOpeCode_Click(object sender, EventArgs e)
        {
            textBoxOpeCode.Text = "";
            工番 = false;
        }
        private void buttonClearModel_Click(object sender, EventArgs e)
        {
            textBoxModel.Text = "";
            製品型式 = false;
        }

        //タイマーイベントの処理**********************************************************************************
        private void timerLbMessage_Tick(object sender, EventArgs e)
        {
            if (!Flags.WarningFlag)
            {
                ShowWarning();
            }

            if (labelMessage.BackColor == Color.WhiteSmoke)
            {
                labelMessage.BackColor = Color.LightBlue;

                switch (labelMessage.Text)
                {
                    case Constants.MessOperator:
                        panelOperator.BackColor = Color.LightBlue;
                        panelOpeCode.BackColor = Color.WhiteSmoke;
                        panelModel.BackColor = Color.WhiteSmoke;
                        buttonStart.BackColor = Color.WhiteSmoke;
                        break;

                    case Constants.MessOpeCode:
                        panelOperator.BackColor = Color.WhiteSmoke;
                        panelOpeCode.BackColor = Color.LightBlue;
                        panelModel.BackColor = Color.WhiteSmoke;
                        buttonStart.BackColor = Color.WhiteSmoke;
                        break;

                    case Constants.MessModel:
                        panelOperator.BackColor = Color.WhiteSmoke;
                        panelOpeCode.BackColor = Color.WhiteSmoke;
                        panelModel.BackColor = Color.LightBlue;
                        buttonStart.BackColor = Color.WhiteSmoke;
                        break;

                    default:
                        panelOperator.BackColor = Color.WhiteSmoke;
                        panelOpeCode.BackColor = Color.WhiteSmoke;
                        panelModel.BackColor = Color.WhiteSmoke;
                        buttonStart.BackColor = Color.LightBlue;
                        break;
                }
            }
            else
            {
                labelMessage.BackColor = Color.WhiteSmoke;

                panelOperator.BackColor = Color.WhiteSmoke;
                panelOpeCode.BackColor = Color.WhiteSmoke;
                panelModel.BackColor = Color.WhiteSmoke;
                buttonStart.BackColor = Color.WhiteSmoke;
            }
        }
        private void timerCurrentTime_Tick(object sender, EventArgs e)
        {
            //タイトルバーの更新
            this.Text = "DataLog試験" + " <Ver" + Constants.CheckerSoftVer + ">    " + "[" + System.DateTime.Now + "]";
        }
        private void timerTextInput_Tick(object sender, EventArgs e)
        {
            timerTextInput.Stop();
            if (!工番)
            {
                textBoxOpeCode.Text = "";
            }

            if (!製品型式)
            {
                textBoxModel.Text = "";
            }
        }

        //開始ボタン　イベントいろいろ**********************************************************************************
        private void buttonStart_Enter(object sender, EventArgs e)
        {
            //入力必須項目が未入力の状態で開始ボタンを押すと、一瞬 "製品をセットして開始ボタンを押してください" が
            //表示され気持ち悪いので、クリックイベントの前にEnterイベントではじく
            if (!checkBoxTestCase.Checked)
            {
                if (!作業者名 || !工番 || !製品型式)
                {
                    SetFocus();
                    return;
                }
            }

            if (buttonStart.Text == "開始")
            {
                labelMessage.Text = Constants.MessSet;
                return;
            }


        }
        private void buttonStart_Click(object sender, EventArgs e)
        {
            switch (buttonStart.Text)
            {
                case "開始":

                    if (checkBoxTestCase.Checked)
                    {
                        if (comboBoxTestCase.SelectedIndex == -1)
                        {
                            MessageBox.Show("1項目試験を選択してください");
                            return;
                        }
                    }
                    else //1項目試験の場合は、作業者名、工番、製品型式の入力は不要
                    {
                        if (!作業者名 || !工番 || !製品型式)
                        {
                            SetFocus();
                            return;
                        }
                    }


                    //プレス治具のレバーが下がっているかどうかの判定
                    if (!General.CheckPress())
                    {
                        MessageBox.Show("プレス治具のレバーが下がっていません！");
                        return;
                    }

                    timerLbMessage.Enabled = false;

                    buttonStart.Text = "停止";
                    buttonStart.BackColor = Color.WhiteSmoke;
                    labelMessage.Text = "";

                    //各種フラグの初期化
                    Flags.StopFlag = false;
                    Flags.RetryFlag = false;

                    //プログレスバー用カウンターの初期化
                    State.プログレスバーカウント = 0;

                    //フォームのロック
                    LockForm(true);

                    //ストップウォッチの初期化
                    sw.Stop();
                    sw.Reset();


                    Test();
                    break;

                case "停止":
                    Flags.StopFlag = true;
                    buttonStart.Enabled = false;

                    break;

                case "確認":
                    General.StopSound();
                    ClearForm();

                    //フォームのロック解除
                    LockForm(false);

                    break;
            }
        }
        private void buttonStart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.IsInputKey = true;
        }
        private void buttonStart_Leave(object sender, EventArgs e)
        {
            buttonStart.BackColor = Color.WhiteSmoke;
        }

        //checkBoxTestCaseイベントいろいろ**********************************************************************************
        private void checkBoxTestCase_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxTestCase.Checked)
            {
                comboBoxTestCase.Text = "";
                comboBoxTestCase.Visible = true;
            }
            else
            {
                comboBoxTestCase.Visible = false;
            }

        }

        //ログ表示用テキストボックスのイベントいろいろ**********************************************************************************
        private void textBoxTestLog_TextChanged(object sender, EventArgs e)
        {
            textBoxTestLog.SelectionStart = textBoxTestLog.Text.Length;
            textBoxTestLog.ScrollToCaret();
        }
        private void textBoxTestLog_Enter(object sender, EventArgs e)
        {

        }
        private void textBoxCommLog_TextChanged(object sender, EventArgs e)
        {
            textBoxCommLog.SelectionStart = textBoxCommLog.Text.Length;
            textBoxCommLog.ScrollToCaret();
        }

        //メニューストリップ　イベントいろいろ**********************************************************************************
        private void MenuFile_Close_Click(object sender, EventArgs e)
        {

        }
        private void MenuUtility_CameraOffset_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            //オフセット用の静止画像を取得する
            MessageBox.Show("ゴールデンサンプルをプレス治具にセットして\r\n" + "OKボタンを押してください");

            //LEDを全点灯させる処理
            General.io.OutByte(EPX64S.PORT.P1, 0x07);//ジャンパー設定をRS422にして、コミニッチに接続する
            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.H);//U8-11をGNDにおとす処理
            General.Wait(500);

            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b0, EPX64S.OUT.H);//電源ON
            General.Wait(4500);
            Target.sendData(Target.PortName.P1_422, "LedCheckALL");
            General.Wait(2000);

            TEST_LED.GetOffsetPic();

            General.cam.Dispose();

            //オフセット画像を取得したらＩＯをリセットしておく
            General.ResetIo();

            //FormOffsetクラスのインスタンスを作成する●●●●●●●●●●●●●●●●●●●●●●●●●●●
            CameraOffsetForm cForm = new CameraOffsetForm();

            //FormOffset1を表示する
            //ここではモーダルダイアログボックスとして表示する
            //オーナーウィンドウにthisを指定する
            cForm.ShowDialog(this);
            //フォームが必要なくなったところで、Disposeを呼び出す
            cForm.Dispose();


            //TODO: カメラ補正したら座標を反映する いちいちconfigファイルをロードしない
            ////検査用パラメータのロード
            //if (!State.LoadParameter())
            //{
            //    MessageBox.Show("パラメータロード異常\r\n" + "アプリケーションを終了します");
            //    Environment.Exit(0);
            //}

            ////検査用パラメータのセット
            //if (!State.SetSpec())
            //{
            //    MessageBox.Show("パラメータセット異常\r\n" + "アプリケーションを終了します");
            //    Environment.Exit(0);
            //}




            this.Enabled = true;

            //メインフォームの初期化
            InitForm();

            this.Refresh();
            ReloadCamera();
        }
        private void MenuUtility_Maintenance_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            //アイテム追加フォームの作成
            Maintenance mForm = new Maintenance();
            // Form をモーダルで表示する
            mForm.ShowDialog();

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            mForm.Dispose();
            this.Enabled = true;
            this.Refresh();
        }
        private void MenuUtility_EditOperator_Click(object sender, EventArgs e)
        {
            this.Enabled = false;

            //アイテム追加フォームの作成
            SetOperatorForm oForm = new SetOperatorForm();

            // Form をモーダルで表示する
            oForm.ShowDialog();

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            oForm.Dispose();
            this.Refresh();

            this.Enabled = true;

            //メインフォームの初期化
            InitForm();

            this.Refresh();
        }
        private void MenuUtility_Reload_Click(object sender, EventArgs e)
        {

            //試験用パラメータのロード
            State.LoadConfigData();

            //TODO: どうする？？
            ////検査用パラメータのセット
            //if (!State.SetSpec())
            //{
            //    MessageBox.Show("パラメータセット異常\r\n" + "アプリケーションを終了します");
            //    Environment.Exit(0);
            //}


            //メインフォームの初期化
            InitForm();
        }

        private void MenuHelp_VerInfo_Click(object sender, EventArgs e)
        {
            // Form1 の新しいインスタンスを生成する
            VerInfoForm vForm = new VerInfoForm();

            // Form1 をモーダルで表示する
            vForm.ShowDialog();

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            vForm.Dispose();
        }


        //comboBoxOperatorNameイベントいろいろ**********************************************************************************
        private void comboBoxOperatorName_DropDownClosed(object sender, EventArgs e)
        {
            if (comboBoxOperatorName.SelectedIndex == -1)
            {
                作業者名 = false;
            }
            else
            {
                作業者名 = true;
            }
        }
        private void comboBoxOperatorName_Leave(object sender, EventArgs e)
        {
            panelOperator.BackColor = Color.WhiteSmoke;
        }

        //textBoxOpeCodeイベントいろいろ**********************************************************************************
        private void textBoxOpeCode_Enter(object sender, EventArgs e)
        {
            if (!作業者名)
            {
                SetFocus();
            }
        }
        private void textBoxOpeCode_TextChanged(object sender, EventArgs e)
        {
            //１文字入力されるごとに、タイマーを初期化する
            timerTextInput.Stop();
            timerTextInput.Start();

            if (textBoxOpeCode.TextLength == 13) //textBoxOpeCodeのMaxlenghthは13に設定してあります
            {
                timerTextInput.Stop();

                //以降は工番が正しく入力されているかどうかの判定
                if (System.Text.RegularExpressions.Regex.IsMatch(
                    textBoxOpeCode.Text, @"^\d-\d\d-\d\d\d\d-\d\d\d$",
                    System.Text.RegularExpressions.RegexOptions.ECMAScript))
                {
                    工番 = true;
                }
                else
                {
                    工番 = false;
                }
            }


        }
        private void textBoxOpeCode_Leave(object sender, EventArgs e)
        {

            panelOpeCode.BackColor = Color.WhiteSmoke;
        }

        //textBoxModelのイベントいろいろ**********************************************************************************
        private void textBoxModel_Enter(object sender, EventArgs e)
        {
            if (!作業者名 || !工番)
            {
                SetFocus();
                return;
            }

        }
        private void textBoxModel_TextChanged(object sender, EventArgs e)
        {
            //１文字入力されたら、タイマーを初期化する
            timerTextInput.Stop();
            timerTextInput.Start();
            foreach (var m in State.TestSpec.SdInfo)
            {
                string[] SdInfoArray = m.Split(',');//{DATALOGGING/R,2G}
                if (textBoxModel.Text == SdInfoArray[0])
                {
                    timerTextInput.Stop();
                    //検査する型番のSDカード容量をセットする
                    State.SdMemory = SdInfoArray[1];

                    製品型式 = true;
                    return;
                }
            }

        }

        private void textBoxModel_Leave(object sender, EventArgs e)
        {
            panelModel.BackColor = Color.WhiteSmoke;
        }


        //作業者名コンボボックスのロック**********************************************************************************
        private void LockOpeName(bool key)
        {
            if (key)
            {
                this.comboBoxOperatorName.Enabled = false;
            }
            else
            {
                this.comboBoxOperatorName.Enabled = true;
            }
        }
        //工番テキストボックスのロック**********************************************************************************
        private void LockOpeCode(bool key)
        {
            if (key)
            {
                textBoxOpeCode.Enabled = false;
            }
            else
            {
                textBoxOpeCode.Enabled = true;
            }
        }
        //型番テキストボックスのロック**********************************************************************************
        private void LockModel(bool key)
        {
            if (key)
            {
                textBoxModel.Enabled = false;
            }
            else
            {
                textBoxModel.Enabled = true;
            }
        }


        //フォーカスのセット**********************************************************************************
        private void SetFocus()
        {
            Application.DoEvents();
            if (!作業者名)
            {
                comboBoxOperatorName.Focus();
                labelMessage.Text = Constants.MessOperator;
                //General.PlaySound2(Constants.OperatorSound);
                return;
            }

            Application.DoEvents();
            if (!工番)
            {
                textBoxOpeCode.Focus();
                labelMessage.Text = Constants.MessOpeCode;
                //General.PlaySound2(Constants.OpeCodeSound);
                return;
            }

            Application.DoEvents();
            if (!製品型式)
            {
                textBoxModel.Focus();
                labelMessage.Text = Constants.MessModel;
                //General.PlaySound2(Constants.ModelSound);
                return;
            }

            Application.DoEvents();
            buttonStart.Focus();
            return;

        }

        //プロパティ**********************************************************************************
        private bool 作業者名
        {
            get
            {
                return FlagOpeName;
            }
            set
            {
                FlagOpeName = value;
                LockOpeName(value);

                if (value)
                {
                    if (comboBoxOperatorName.SelectedItem.ToString() == "畔上")
                    {
                        checkBoxTestCase.Visible = true;
                        checkBoxTestCase.Checked = false;
                    }
                }
                else
                {
                    checkBoxTestCase.Visible = false;
                    comboBoxTestCase.Visible = false;
                }


                SetFocus();
            }
        }
        private bool 工番
        {
            get
            {
                return FlagOpeCode;
            }
            set
            {
                FlagOpeCode = value;
                LockOpeCode(value);
                SetFocus();
            }
        }
        private bool 製品型式
        {
            get
            {
                return FlagModel;
            }
            set
            {
                FlagModel = value;
                LockModel(value);
                SetFocus();
            }
        }

        //フォームのイニシャライズ（フォームロード時）**************************************************************************
        private void InitForm()
        {

            //コンボボックスの初期化
            comboBoxOperatorName.Text = "";
            comboBoxTestCase.Text = "";
            comboBoxTestCase.Visible = false;
            comboBoxDevices.Text = "";

            //テキストボックスの初期化
            textBoxOpeCode.Text = "";
            textBoxModel.Text = "";
            textBoxTestLog.Text = "";
            textBoxCommLog.Text = "";
            textBoxTestLog.ReadOnly = true;
            textBoxCommLog.ReadOnly = true;

            //ラベルの初期化
            labelDecision.Text = "";
            labelErrorMessage.Text = "";
            labelSpec.Text = "";
            labelMeasurement.Text = "";

            labelVccVol.Text = "";
            labelVee.Text = "";
            labelCurrent.Text = "";
            labelRtc.Text = "";
            labelFirmVer.Text = State.TestSpec.FirmVer;
            labelFirmSum.Text = State.TestSpec.FirmSum;

            labelSw1.Text = "SW1";
            labelSw2.Text = "SW2";
            labelS1.Text = "S1";

            //チェックボックスの初期化
            checkBoxSelectWriteFirm.Checked = false;
            checkBoxTestCase.Visible = false;

            SetPicture(0);
            pictureBoxSw1.ImageLocation = Constants.SdefaultPic;
            pictureBoxSw2.ImageLocation = Constants.SdefaultPic;
            pictureBoxS1_1.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_2.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_3.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_4.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_5.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_6.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_7.ImageLocation = Constants.DipOffPic;
            pictureBoxS1_8.ImageLocation = Constants.DipOffPic;


            progressBar1.Value = 0;

            //LED関連
            labelTX1rgb.Text = "";
            labelTX2rgb.Text = "";
            labelTX3rgb.Text = "";
            labelTX4rgb.Text = "";
            labelRX1rgb.Text = "";
            labelRX2rgb.Text = "";
            labelRX3rgb.Text = "";
            labelRX4rgb.Text = "";
            labelCPUrgb.Text = "";
            labelRUNrgb.Text = "";
            labelVCCrgb.Text = "";
            labelDIrgb.Text = "";
            labelDOrgb.Text = "";

            labelTX1.BackColor = SystemColors.Control;
            labelTX2.BackColor = SystemColors.Control;
            labelTX3.BackColor = SystemColors.Control;
            labelTX4.BackColor = SystemColors.Control;
            labelRX1.BackColor = SystemColors.Control;
            labelRX2.BackColor = SystemColors.Control;
            labelRX3.BackColor = SystemColors.Control;
            labelRX4.BackColor = SystemColors.Control;
            labelCPU.BackColor = SystemColors.Control;
            labelRUN.BackColor = SystemColors.Control;
            labelVCC.BackColor = SystemColors.Control;
            labelDI.BackColor = SystemColors.Control;
            labelDO.BackColor = SystemColors.Control;

            //作業者一覧の設定
            comboBoxOperatorName.Items.Clear();

            foreach (string Operator in State.Setting.作業者リスト)
            {
                comboBoxOperatorName.Items.Add(Operator);
            }


            //１項目試験用コンボボックスの設定
            comboBoxTestCase.Enabled = true;
            comboBoxTestCase.Items.Clear();
            foreach (TestProperty testProp in State.試験項目リスト)
            {
                if (testProp.stepNo % 10 == 0) //一項目試験で選択できるのは、枝番以外(１０の倍数)のステップのみとする
                {
                    comboBoxTestCase.Items.Add(testProp.stepNo + "_" + testProp.testCase);
                }
            }

            作業者名 = false;
            工番 = false;
            製品型式 = false;

        }

        //フォームのイニシャライズ（試験終了後、確認ボタンを押した時）**************************************************************************
        private void ClearForm()
        {
            labelForeColorBlack();

            //テキストボックスのクリア
            textBoxTestLog.Text = "";
            textBoxCommLog.Text = "";

            //ラベルのクリア
            labelVccVol.Text = "";
            labelVee.Text = "";
            labelCurrent.Text = "";
            labelRtc.Text = "";

            labelDecision.Text = "";
            labelErrorMessage.Text = "";
            labelSpec.Text = "";
            labelMeasurement.Text = "";
            labelMessage.Text = Constants.MessSet;

            labelSw1.Text = "SW1";
            labelSw2.Text = "SW2";
            labelS1.Text = "S1";

            //開始ボタンの初期化
            buttonStart.Text = "開始";

            SetPicture(0);
            pictureBoxSw1.ImageLocation = Constants.SdefaultPic;
            pictureBoxSw2.ImageLocation = Constants.SdefaultPic;
            pictureBoxS1_1.ImageLocation = Constants.DipOffPic;

            //LED関連
            labelTX1rgb.Text = "";
            labelTX2rgb.Text = "";
            labelTX3rgb.Text = "";
            labelTX4rgb.Text = "";
            labelRX1rgb.Text = "";
            labelRX2rgb.Text = "";
            labelRX3rgb.Text = "";
            labelRX4rgb.Text = "";
            labelCPUrgb.Text = "";
            labelRUNrgb.Text = "";
            labelVCCrgb.Text = "";
            labelDIrgb.Text = "";
            labelDOrgb.Text = "";

            labelTX1rgb.ForeColor = Color.Black;
            labelTX2rgb.ForeColor = Color.Black;
            labelTX3rgb.ForeColor = Color.Black;
            labelTX4rgb.ForeColor = Color.Black;
            labelRX1rgb.ForeColor = Color.Black;
            labelRX2rgb.ForeColor = Color.Black;
            labelRX3rgb.ForeColor = Color.Black;
            labelRX4rgb.ForeColor = Color.Black;
            labelCPUrgb.ForeColor = Color.Black;
            labelRUNrgb.ForeColor = Color.Black;
            labelVCCrgb.ForeColor = Color.Black;
            labelDIrgb.ForeColor = Color.Black;
            labelDOrgb.ForeColor = Color.Black;

            labelTX1.BackColor = SystemColors.Control;
            labelTX2.BackColor = SystemColors.Control;
            labelTX3.BackColor = SystemColors.Control;
            labelTX4.BackColor = SystemColors.Control;
            labelRX1.BackColor = SystemColors.Control;
            labelRX2.BackColor = SystemColors.Control;
            labelRX3.BackColor = SystemColors.Control;
            labelRX4.BackColor = SystemColors.Control;
            labelCPU.BackColor = SystemColors.Control;
            labelRUN.BackColor = SystemColors.Control;
            labelVCC.BackColor = SystemColors.Control;
            labelDI.BackColor = SystemColors.Control;
            labelDO.BackColor = SystemColors.Control;

            //プログレスバーの初期化
            progressBar1.Value = 0;
        }

        public void SetPicture(int mode)
        {
            if (mode == 0)
            {
                pictureBoxDecision.Location = new Point(465, 40);
                pictureBoxDecision.Visible = false;

                panelPreview.Location = new Point(10, 40);
                panelPreview.Visible = true;

            }
            else
            {
                pictureBoxDecision.Location = new Point(10, 40);
                pictureBoxDecision.Visible = true;

                panelPreview.Location = new Point(465, 40);
                panelPreview.Visible = false;
            }


        }

        //テンプレート画像表示用メソッド**************************************************************************
        public void ShowS1Picture(int data)
        {
            int iBuff = 0;

            //bit1の処理
            iBuff = data & 0x01;
            if (iBuff == 0)
            {
                pictureBoxS1_1.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_1.ImageLocation = Constants.DipOnPic;
            }

            //bit2の処理
            iBuff = data & 0x02;
            if (iBuff == 0)
            {
                pictureBoxS1_2.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_2.ImageLocation = Constants.DipOnPic;
            }

            //bit3の処理
            iBuff = data & 0x04;
            if (iBuff == 0)
            {
                pictureBoxS1_3.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_3.ImageLocation = Constants.DipOnPic;
            }

            //bit4の処理
            iBuff = data & 0x08;
            if (iBuff == 0)
            {
                pictureBoxS1_4.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_4.ImageLocation = Constants.DipOnPic;
            }

            //bit5の処理
            iBuff = data & 0x10;
            if (iBuff == 0)
            {
                pictureBoxS1_5.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_5.ImageLocation = Constants.DipOnPic;
            }

            //bit6の処理
            iBuff = data & 0x20;
            if (iBuff == 0)
            {
                pictureBoxS1_6.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_6.ImageLocation = Constants.DipOnPic;
            }

            //bit7の処理
            iBuff = data & 0x40;
            if (iBuff == 0)
            {
                pictureBoxS1_7.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_7.ImageLocation = Constants.DipOnPic;
            }

            //bit8の処理
            iBuff = data & 0x80;
            if (iBuff == 0)
            {
                pictureBoxS1_8.ImageLocation = Constants.DipOffPic;
            }
            else
            {
                pictureBoxS1_8.ImageLocation = Constants.DipOnPic;
            }


        }

        public void ShowSw1Picture(int data)
        {
            switch (data)
            {
                case 0:
                    pictureBoxSw1.ImageLocation = Constants.S0Pic;
                    break;
                case 1:
                    pictureBoxSw1.ImageLocation = Constants.S1Pic;
                    break;
                case 2:
                    pictureBoxSw1.ImageLocation = Constants.S2Pic;
                    break;
                case 3:
                    pictureBoxSw1.ImageLocation = Constants.S3Pic;
                    break;
                case 4:
                    pictureBoxSw1.ImageLocation = Constants.S4Pic;
                    break;
                case 5:
                    pictureBoxSw1.ImageLocation = Constants.S5Pic;
                    break;
                case 6:
                    pictureBoxSw1.ImageLocation = Constants.S6Pic;
                    break;
                case 7:
                    pictureBoxSw1.ImageLocation = Constants.S7Pic;
                    break;
                case 8:
                    pictureBoxSw1.ImageLocation = Constants.S8Pic;
                    break;
                case 9:
                    pictureBoxSw1.ImageLocation = Constants.S9Pic;
                    break;
                case 10:
                    pictureBoxSw1.ImageLocation = Constants.SaPic;
                    break;
                case 11:
                    pictureBoxSw1.ImageLocation = Constants.SbPic;
                    break;
                case 12:
                    pictureBoxSw1.ImageLocation = Constants.ScPic;
                    break;
                case 13:
                    pictureBoxSw1.ImageLocation = Constants.SdPic;
                    break;
                case 14:
                    pictureBoxSw1.ImageLocation = Constants.SePic;
                    break;
                case 15:
                    pictureBoxSw1.ImageLocation = Constants.SfPic;
                    break;

            }

        }

        public void ShowSw2Picture(int data)
        {
            switch (data)
            {
                case 0:
                    pictureBoxSw2.ImageLocation = Constants.S0Pic;
                    break;
                case 1:
                    pictureBoxSw2.ImageLocation = Constants.S1Pic;
                    break;
                case 2:
                    pictureBoxSw2.ImageLocation = Constants.S2Pic;
                    break;
                case 3:
                    pictureBoxSw2.ImageLocation = Constants.S3Pic;
                    break;
                case 4:
                    pictureBoxSw2.ImageLocation = Constants.S4Pic;
                    break;
                case 5:
                    pictureBoxSw2.ImageLocation = Constants.S5Pic;
                    break;
                case 6:
                    pictureBoxSw2.ImageLocation = Constants.S6Pic;
                    break;
                case 7:
                    pictureBoxSw2.ImageLocation = Constants.S7Pic;
                    break;
                case 8:
                    pictureBoxSw2.ImageLocation = Constants.S8Pic;
                    break;
                case 9:
                    pictureBoxSw2.ImageLocation = Constants.S9Pic;
                    break;
                case 10:
                    pictureBoxSw2.ImageLocation = Constants.SaPic;
                    break;
                case 11:
                    pictureBoxSw2.ImageLocation = Constants.SbPic;
                    break;
                case 12:
                    pictureBoxSw2.ImageLocation = Constants.ScPic;
                    break;
                case 13:
                    pictureBoxSw2.ImageLocation = Constants.SdPic;
                    break;
                case 14:
                    pictureBoxSw2.ImageLocation = Constants.SePic;
                    break;
                case 15:
                    pictureBoxSw2.ImageLocation = Constants.SfPic;
                    break;

            }

        }

        //フォームのロック
        private void LockForm(bool data)//true：ロック　false：ロック解除
        {
            if (data)
            {
                buttonClearOperator.Enabled = false;
                buttonClearOpeCode.Enabled = false;
                buttonClearModel.Enabled = false;
                menuStrip1.Enabled = false;
                checkBoxSelectWriteFirm.Enabled = false;
                checkBoxTestCase.Enabled = false;
                comboBoxTestCase.Enabled = false;
                comboBoxDevices.Enabled = false;

            }
            else
            {
                buttonClearOperator.Enabled = true;
                buttonClearOpeCode.Enabled = true;
                buttonClearModel.Enabled = true;
                menuStrip1.Enabled = true;
                checkBoxSelectWriteFirm.Enabled = true;
                checkBoxTestCase.Enabled = true;
                comboBoxTestCase.Enabled = true;
                comboBoxDevices.Enabled = true;

            }

        }

        //検査ログに、ＯＫを書き込む　　
        private void WriteOk()
        {
            buttonStart.Enabled = false;
            textBoxTestLog.Text += "---OK\r\n";
            textBoxTestLog.Select(0, 0);//テキスト全選択回避のため
            this.Refresh();

            State.プログレスバーカウント++;
            progressBar1.Value = (int)(((double)State.プログレスバーカウント / State.TotalStep) * 100);
            this.Refresh();

            buttonStart.Enabled = true;

            //ReTest時に合格した場合、検査データのフォント色を赤→黒に戻す処理
            if (Flags.RetryFlag)
            {
                labelForeColorBlack();
                Flags.RetryFlag = false;
            }

        }

        //検査ログに、ＮＧを書き込む　
        private void WriteNg()
        {
            buttonStart.Enabled = false;
            textBoxTestLog.Text += "---NG\r\n";
            textBoxTestLog.Select(0, 0);//テキスト全選択回避のため
            this.Refresh();
            buttonStart.Enabled = true;
        }

        //検査データのラベル色を黒に戻す（リトライ時）
        private void labelForeColorBlack()
        {
            //ラベルのクリア
            labelVccVol.ForeColor = Color.Black;
            labelVee.ForeColor = Color.Black;
            labelCurrent.ForeColor = Color.Black;
            labelRtc.ForeColor = Color.Black;

            labelTX1rgb.ForeColor = Color.Black;
            labelTX2rgb.ForeColor = Color.Black;
            labelTX3rgb.ForeColor = Color.Black;
            labelTX4rgb.ForeColor = Color.Black;
            labelRX1rgb.ForeColor = Color.Black;
            labelRX2rgb.ForeColor = Color.Black;
            labelRX3rgb.ForeColor = Color.Black;
            labelRX4rgb.ForeColor = Color.Black;
            labelCPUrgb.ForeColor = Color.Black;
            labelRUNrgb.ForeColor = Color.Black;
            labelVCCrgb.ForeColor = Color.Black;
            labelDIrgb.ForeColor = Color.Black;
            labelDOrgb.ForeColor = Color.Black;

        }

        //テストログの更新
        private void SetTestLog()
        {
            labelMessage.Text = "自動試験中・・・　しばらくお待ちください";
            textBoxTestLog.Text += State.StepNo.ToString() + "_" + State.試験項目;
            if (Flags.RetryFlag) textBoxTestLog.Text += "(再)";
            textBoxTestLog.Select(0, 0);//テキスト全選択回避のため
            this.Refresh();
        }

        //通信ログの更新（Targetクラスにデリゲート経由で渡す）
        private void SetCommLog(string data)
        {
            textBoxCommLog.Text += data;//
            textBoxCommLog.Select(0, 0);//テキスト全選択回避のため
            textBoxCommLog.Refresh();
            //this.Refresh();
        }

        //スイッチ表示の更新（ラベルとピクチャー）
        private void SetSwDisplay(SwName name, string LabelData, int BitData)
        {
            switch (name)
            {
                case SwName.SW1:
                    labelSw1.Text = "SW1 [" + LabelData + "]";
                    ShowSw1Picture(BitData);
                    break;

                case SwName.SW2:
                    labelSw2.Text = "SW2 [" + LabelData + "]";
                    ShowSw2Picture(BitData);
                    break;

                case SwName.S1:
                    labelS1.Text = "S1 [" + LabelData + "]";
                    ShowS1Picture(BitData);
                    break;

                default: break;
            }

        }

        //試験ＮＧ時に、判定枠にエラー情報を表示する
        private void ShowErrorInfo()
        {
            //停止ボタンが押されたときのエラー表示
            if (Flags.StopFlag)
            {
                labelErrorMessage.Text = "強制停止";
                labelSpec.Text = "";
                labelMeasurement.Text = "";
                return;
            }

            //通常のエラー表示
            //①エラーメッセージの表示
            labelErrorMessage.Text = State.エラーメッセージ;


            //②検査スペックの表示
            switch (State.StepNo)
            {

                case 30://Vcc電圧の規格
                    labelSpec.Text = "規格値：" + State.TestSpec.VccMin.ToString("F2") + "～" + State.TestSpec.VccMax.ToString("F2") + "(V)";
                    break;

                case 40://3.3V電圧の規格
                    labelSpec.Text = "規格値：" + State.TestSpec.VeeMin.ToString("F2") + "～" + State.TestSpec.VeeMax.ToString("F2") + "(V)";
                    break;

                case 50://消費電流の規格
                    labelSpec.Text = "規格値：" + State.TestSpec.CurrMin.ToString("F2") + "～" + State.TestSpec.CurrMax.ToString("F2") + "(A)";
                    break;

                case 140://リアルタイムクロックの規格
                    labelSpec.Text = "規格値：" + State.TestSpec.RtcMax.ToString("F2") + "秒以下";
                    break;

                case 160://SW1出荷設定の規格
                    labelSpec.Text = "規格値：F";
                    break;

                case 161://SW2出荷設定の規格
                    labelSpec.Text = "規格値：F";
                    break;

                case 162://S1出荷設定の規格
                    labelSpec.Text = "規格値：00";
                    break;
                case 170://製品ソフト書き込みの規格（ファームウェアチェックサム）
                    labelSpec.Text = "チェックサム：" + State.TestSpec.FirmSum;
                    break;

                default:
                    labelSpec.Text = "規格値：　－";
                    break;
            }

            //③計測値の表示
            switch (State.StepNo)
            {
                case 10://コネクタ実装チェック
                    labelMeasurement.Text = "計測値:";
                    if (!TEST_コネクタ.Port1Flag)
                    {
                        labelMeasurement.Text += "P1,";
                    }
                    if (!TEST_コネクタ.Port2Flag)
                    {
                        labelMeasurement.Text += "P2,";
                    }
                    if (!TEST_コネクタ.Port3Flag)
                    {
                        labelMeasurement.Text += "P3,";
                    }
                    if (!TEST_コネクタ.Port4Flag)
                    {
                        labelMeasurement.Text += "P4,";
                    }
                    if (!TEST_コネクタ.Cn1Flag)
                    {
                        labelMeasurement.Text += "CN1,";
                    }
                    if (!TEST_コネクタ.Cn3Flag)
                    {
                        labelMeasurement.Text += "CN3,";
                    }

                    labelMeasurement.Text += "異常";
                    break;

                case 30://Vcc電圧
                    labelMeasurement.Text = "計測値：" + labelVccVol.Text + "(V)";
                    break;

                case 40://Vee電圧
                    labelMeasurement.Text = "計測値：" + labelVee.Text + "(V)";
                    break;

                case 50://消費電流
                    labelMeasurement.Text = "計測値：" + labelCurrent.Text + "（A）";
                    break;

                case 140:
                    labelMeasurement.Text = "計測値：" + labelRtc.Text + "（秒）";
                    break;

                case 160:
                    labelMeasurement.Text = "計測値：" + labelSw1.Text;
                    break;

                case 161:
                    labelMeasurement.Text = "計測値：" + labelSw2.Text;
                    break;

                case 162:
                    labelMeasurement.Text = "計測値：" + labelS1.Text;
                    break;

                case 170://ファームウェアチェックサム
                    labelSpec.Text = "計測値：";
                    break;

                default:
                    labelMeasurement.Text = "計測値：　－";
                    break;
            }





        }

        //WEBカメラの初期化
        private void ReloadCamera()
        {
            try
            {
                if (General.cam != null) General.cam.Dispose();

                comboBoxDevices.Items.Clear();
                foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
                {
                    comboBoxDevices.Items.Add(ds.Name);
                }
                comboBoxDevices.SelectedIndex = 0;
                General.cam = new Capture(1, 640, 480, 30, panelPreview);

            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
                //Console.Error.WriteLine(ex.Message);
            }

        }

        //画像検査パネルの更新
        private void 画像検査パネル更新(LedName name)
        {
            var prop = TEST_LED.LedPropList.Find(l => l.name == name);

            switch (prop.name)
            {
                case LedName.TX1:
                    labelTX1.BackColor = prop.Color;
                    labelTX1rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelTX1rgb.ForeColor = Color.Red;
                    break;
                case LedName.TX2:
                    labelTX2.BackColor = prop.Color;
                    labelTX2rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelTX2rgb.ForeColor = Color.Red;
                    break;
                case LedName.TX3:
                    labelTX3.BackColor = prop.Color;
                    labelTX3rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelTX3rgb.ForeColor = Color.Red;
                    break;
                case LedName.TX4:
                    labelTX4.BackColor = prop.Color;
                    labelTX4rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelTX4rgb.ForeColor = Color.Red;
                    break;

                case LedName.RX1:
                    labelRX1.BackColor = prop.Color;
                    labelRX1rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelRX1rgb.ForeColor = Color.Red;
                    break;
                case LedName.RX2:
                    labelRX2.BackColor = prop.Color;
                    labelRX2rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelRX2rgb.ForeColor = Color.Red;
                    break;
                case LedName.RX3:
                    labelRX3.BackColor = prop.Color;
                    labelRX3rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelRX3rgb.ForeColor = Color.Red;
                    break;
                case LedName.RX4:
                    labelRX4.BackColor = prop.Color;
                    labelRX4rgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelRX4rgb.ForeColor = Color.Red;
                    break;
                case LedName.CPU:
                    labelCPU.BackColor = prop.Color;
                    labelCPUrgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelCPUrgb.ForeColor = Color.Red;
                    break;
                case LedName.VCC:
                    labelVCC.BackColor = prop.Color;
                    labelVCCrgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelVCCrgb.ForeColor = Color.Red;
                    break;
                case LedName.RUN:
                    labelRUN.BackColor = prop.Color;
                    labelRUNrgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelRUNrgb.ForeColor = Color.Red;
                    break;
                case LedName.DI:
                    labelDI.BackColor = prop.Color;
                    labelDIrgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelDIrgb.ForeColor = Color.Red;
                    break;

                case LedName.DO:
                    labelDO.BackColor = prop.Color;
                    labelDOrgb.Text = prop.Red.ToString() + "," + prop.Green.ToString() + "," + prop.Blue.ToString() + "," + prop.H.ToString("F0");
                    if (!prop.FlagColor) labelDOrgb.ForeColor = Color.Red;
                    break;
            }


            if (!TEST_LED.FlagColor || !TEST_LED.FlagOn)
            {
                // FileStream を開く
                System.IO.FileStream hStream = new System.IO.FileStream(@"C:\DataLog\画像検査\source.bmp", System.IO.FileMode.Open);

                // FileStream から画像を読み込んで表示
                pictureBoxDecision.Image = Image.FromStream(hStream);
                SetPicture(1);

                // FileStream を閉じる (正しくは オブジェクトの破棄を保証する を参照)
                hStream.Close();
            }

        }

        //**************************************************************************
        //検査（メインルーチン）
        //引数：なし
        //戻値：なし
        //**************************************************************************
        //メインルーチン
        public void Test()
        {


            DialogResult result;

            double dBuff;   //int型の汎用バッファ定義
            int iBuff;      //int型の汎用バッファ定義
            string sBuff;   //string型の汎用バッファ定義
            int retryCnt = 0;

            State.電源投入時の待ち時間 = 2000;//初期値2秒


            Flags.Epx64Stutas = true;//EPX64のステータス初期化
            State.プログレスバーカウント = 0;
            this.Refresh();

            List<TestProperty> 抽出した試験項目リスト = new List<TestProperty>();
            //int index;//インデックス
            //int MaxIndex;//インデックス最大値

            if (checkBoxTestCase.Checked)
            {
                //試験項目リストの中から一項目試験項目を抽出する
                sBuff = comboBoxTestCase.SelectedItem.ToString();
                sBuff = sBuff.Substring(0, sBuff.IndexOf("_"));　//ステップナンバーの抽出
                iBuff = Int32.Parse(sBuff);//文字列→intに変換
                var query = from n in State.試験項目リスト where (n.stepNo / 10) == (iBuff / 10) select n;
                foreach (TestProperty 抽出データ in query) 抽出した試験項目リスト.Add(抽出データ);
            }
            else
            {
                抽出した試験項目リスト = State.試験項目リスト;　//値渡し
            }

            //トータルステップ数の算出（プログレスバーの参照用）
            State.TotalStep = 抽出した試験項目リスト.Count;

            //試験開始と終了のインデックスを設定する
            int index = 0; //インデックス初期化
            int MaxIndex = 抽出した試験項目リスト.Count - 1;　//インデックス最大値

            //メインループ
            while (true)
            {
                ReTest:
                State.StepNo = 抽出した試験項目リスト[index].stepNo;
                State.試験項目 = 抽出した試験項目リスト[index].testCase;
                State.エラーメッセージ = State.試験項目 + "異常";

                SetTestLog();
                if (General.checkStopButton()) goto TEST_FAIL;


                switch (State.StepNo)   //case番号は、parameterファイル "TestCase"内のステップ№と一致するように設計する
                {
                    case 0: //試験合格品かどうかのチェック（合格品の再試験防止：QK対策）

                        //USB-シリアル変換機に接続
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                        General.Wait(1000);

                        if (!Target.GetInitData())
                        {
                            break;//試験合格していないので次のステップに進む
                        }
                        else
                        {
                            MessageBox.Show("この製品は一度合格しています\r\n合格印が押されているか確認してください", "作業者への警告");
                            goto case 500;
                        }

                    case 10:    //コネクタ実装チェック
                        General.ResetIo();
                        General.Wait(500);
                        if (!TEST_コネクタ.CheckConnector()) goto case 500;
                        retryCnt = 0;
                        break;

                    case 20:    //検査ソフト書き込み    
                        if (!checkBoxSelectWriteFirm.Checked)
                        {

                            //E1を接続する
                            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                            General.Wait(3000);

                            if (!FlashDevelopmentToolkit.WriteFirmware(FlashDevelopmentToolkit.WriteMode.TestMode)) goto case 500;
                            General.Wait(1500);
                            //E1を切断する
                            General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                            General.Wait(500);
                        }
                        retryCnt = 0;
                        break;

                    case 30:    //Vcc電圧チェック

                        //K1 ON（TargetにDC24Vを供給）
                        General.PowerOn();
                        General.Wait(500);


                        //マルチメータをVccラインに接続
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.H);
                        General.Wait(1000);

                        dBuff = 電流電圧チェック.GetDcV();
                        labelVccVol.Text = dBuff.ToString("F2") + "V";
                        if (dBuff < State.TestSpec.VccMin || dBuff > State.TestSpec.VccMax)
                        {
                            labelVccVol.ForeColor = Color.Red;
                            goto case 500;
                        }
                        retryCnt = 0;
                        break;

                    case 40:    //+3.3V電圧チェック

                        if (checkBoxTestCase.Checked || Flags.RetryFlag)
                        {
                            //K1 ON（TargetにDC24Vを供給）
                            General.PowerOn();
                            General.Wait(500);
                        }

                        //マルチメータを3.3Vラインに接続
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b2, EPX64S.OUT.L);
                        General.Wait(1000);

                        dBuff = 電流電圧チェック.GetDcV();
                        labelVee.Text = dBuff.ToString("F2") + "V";
                        if (dBuff < State.TestSpec.VeeMin || dBuff > State.TestSpec.VeeMax)
                        {
                            labelVee.ForeColor = Color.Red;
                            goto case 500;
                        }
                        retryCnt = 0;
                        break;

                    case 50:    //消費電流チェック

                        General.ResetIo();
                        General.Wait(2000);

                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b1, EPX64S.OUT.H);
                        General.Wait(1000);
                        General.PowerOn();
                        General.Wait(5000);//消費電流安定するまで待つ

                        dBuff = 電流電圧チェック.GetDcA();
                        labelCurrent.Text = dBuff.ToString("F2") + "mA";
                        if (dBuff < State.TestSpec.CurrMin || dBuff > State.TestSpec.CurrMax)
                        {
                            labelCurrent.ForeColor = Color.Red;
                            goto case 500;
                        }

                        General.ResetIo();
                        retryCnt = 0;
                        break;

                    case 60:    //Port1 RS422通信チェック
                        General.Wait(1000); //少しウェイトを入れる

                        //RS422ジャンパ接続 ＋ ｺﾐﾆｯﾁ接続
                        General.io.OutByte(EPX64S.PORT.P1, 0x07);
                        General.Wait(1000);

                        //電源ON
                        General.PowerOn();

                        if (!Target.ReadRecieveData(Target.PortName.P1_422, 6000)) goto case 500;

                        if (Target.RecieveData.IndexOf("ReqData") < 0) goto case 500;

                        General.Wait(2000);

                        if (!TEST_通信.Check422(Target.PortName.P1_422)) goto case 500;

                        General.Wait(200);

                        break;

                    case 61:    //Port2 RS422通信チェック
                        if (!TEST_通信.Check422(Target.PortName.P2_422)) goto case 500;
                        General.Wait(200);

                        break;

                    case 62:    //Port3 RS422通信チェック
                        if (!TEST_通信.Check422(Target.PortName.P3_422)) goto case 500;
                        General.Wait(200);

                        break;

                    case 63:    //Port4 RS422通信チェック
                        if (!TEST_通信.Check422(Target.PortName.P4_422)) goto case 500;
                        General.Wait(200);

                        General.ResetIo();
                        General.Wait(1000);
                        retryCnt = 0;
                        break;

                    case 70:    //Port1 RS232C通信チェック

                        //USB-シリアル変換機に接続
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                        General.Wait(500);

                        //電源ON
                        General.PowerOn();

                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 6000)) goto case 500;

                        if (Target.RecieveData.IndexOf("ReqData") < 0) goto case 500;

                        General.Wait(2000);

                        if (!TEST_通信.Check232c(Target.PortName.P1_232C)) goto case 500;

                        General.Wait(200);

                        break;

                    case 71:    //Port2 RS232C通信チェック

                        if (!TEST_通信.Check232c(Target.PortName.P2_232C)) goto case 500;

                        General.Wait(200);

                        break;

                    case 72:    //Port3 RS232C通信チェック

                        if (!TEST_通信.Check232c(Target.PortName.P3_232C)) goto case 500;

                        General.Wait(200);

                        break;

                    case 73:    //Port4 RS232C通信チェック

                        if (!TEST_通信.Check232c(Target.PortName.P4_232C)) goto case 500;
                        retryCnt = 0;
                        break;

                    case 80:    //LED点灯チェック

                        if (checkBoxTestCase.Checked || Flags.RetryFlag)
                        {
                            SetPicture(0);

                            //USB-シリアル変換機に接続
                            if (!General.電源投入時の処理()) goto case 500;
                        }



                        foreach (var name in Enum.GetValues(typeof(LedName)))
                        {
                            var strName = name.ToString();
                            var testName = (LedName)name;

                            if (testName == LedName.DI || testName == LedName.DO)
                            {
                                //すべてのLEDを点灯させるためにはK4をONする必要がある
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.H);
                                General.Wait(200);
                            }
                            else
                            {
                                //すべてのLEDを点灯させるためにはK4をONする必要がある
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.L);
                                General.Wait(200);
                            }


                            try
                            {
                                textBoxTestLog.Text += "\r\n";
                                textBoxTestLog.Text += strName + " ﾁｪｯｸ";
                                textBoxTestLog.Select(0, 0);//テキスト全選択回避のため
                                this.Refresh();
                                if (TEST_LED.CheckLed(testName))
                                {
                                    textBoxTestLog.Text += "---PASS";
                                    General.Wait(1000);
                                }
                                else
                                {
                                    textBoxTestLog.Text += "\r\n";
                                    if (!TEST_LED.FlagColor)
                                    {
                                        textBoxTestLog.Text += "--カラー異常";
                                    }
                                    else
                                    {
                                        textBoxTestLog.Text += "--点灯異常";
                                    }
                                    textBoxTestLog.Select(0, 0);//テキスト全選択回避のため
                                    this.Refresh();
                                    goto case 500;
                                }
                            }
                            finally
                            {
                                画像検査パネル更新(testName);
                            }

                        }


                        //検査終わったらライブ画像に戻す（これいる？？） TODO:
                        General.Wait(1000);
                        SetPicture(0);

                        retryCnt = 0;
                        break;

                    case 90:    //SW3チェック

                        General.ResetIo();
                        General.Wait(3000);

                        //USB-シリアル変換機に接続
                        if (!General.電源投入時の処理()) goto case 500;

                        Target.ClearBuff();//念のためCOM受信バッファクリア

                        //SW3を押す
                        General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.H);
                        General.Wait(500);
                        General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.L);

                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 5000)) goto case 500;

                        if (Target.RecieveData.IndexOf("ReqData") < 0) goto case 500;
                        retryCnt = 0;
                        break;

                    case 100:    //SDカードチェック

                        General.ResetIo();
                        General.Wait(3000);
                        //USB-シリアル変換機に接続
                        if (!General.電源投入時の処理()) goto case 500;
                        General.Wait(5000);
                        //ターゲットにコマンド送信
                        Target.sendData(Target.PortName.P1_232C, "SDCCheck");

                        //100秒以内に受信バッファ内の受信データバイト数が1以上になるか判定する
                        bool res = false;
                        for (int i = 0; i < 100; i++)
                        {
                            General.Wait(1000);
                            if (General.checkStopButton()) goto case 500;

                            if (Target.GetByteData(Target.PortName.P1_232C) > 0)
                            {
                                res = true;
                                break;
                            }
                        }
                        if (!res) goto case 500;

                        General.Wait(1000);
                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 2000)) goto case 500;

                        if (Target.RecieveData.IndexOf("SDC_" + State.SdMemory + "_OK") < 0) goto case 500;
                        retryCnt = 0;
                        break;

                    case 110:    //リアルタイムクロックセット（これ以降の検査ではPort1_RS232CでTargetと通信を行う）

                        if (checkBoxTestCase.Checked || Flags.RetryFlag)
                        {
                            if (!General.電源投入時の処理()) goto case 500;

                        }

                        General.Wait(2000);
                        Target.sendData(Target.PortName.P1_232C, "SetTime" + Constants.DefaultTime); // "/13/01/01/01/01/00"

                        sw.Start();//ストップウォッチ計測開始

                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) goto case 500;

                        if (Target.RecieveData.IndexOf("SetTimeOK") < 0) goto case 500;
                        retryCnt = 0;
                        break;

                    case 120:    //SW1チェック

                        if (checkBoxTestCase.Checked)
                        {
                            if (!General.電源投入時の処理()) goto case 500;
                        }

                        //照明ON
                        General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, EPX64S.OUT.H);

                        labelMessage.Text = "SW1を時計方向に回してください！";
                        //音鳴らして終了
                        General.PlaySound(Constants.S1Sound);

                        if (!TEST_スイッチ.CheckSw1_2(SwName.SW1, SetSwDisplay, SetCommLog)) goto case 500;

                        General.Wait(1000);
                        labelMessage.Text = "SW1を F にセットしてください！";
                        //音鳴らして終了
                        General.PlaySound2(Constants.S1FSound);
                        if (!TEST_スイッチ.InitializeSw1_2(SwName.SW1, 15, SetSwDisplay, SetCommLog)) goto case 500;
                        retryCnt = 0;
                        break;

                    case 130:    //SW2チェック

                        if (checkBoxTestCase.Checked)
                        {
                            if (!General.電源投入時の処理()) goto case 500;
                        }

                        //照明ON
                        General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, EPX64S.OUT.H);

                        labelMessage.Text = "SW2を時計方向に回してください！";
                        //音鳴らして終了
                        General.PlaySound(Constants.S2Sound);

                        if (!TEST_スイッチ.CheckSw1_2(SwName.SW2, SetSwDisplay, SetCommLog)) goto case 500;

                        General.Wait(1000);
                        labelMessage.Text = "SW2を F にセットしてください！";
                        //音鳴らして終了
                        General.PlaySound2(Constants.S2FSound);
                        if (!TEST_スイッチ.InitializeSw1_2(SwName.SW2, 15, SetSwDisplay, SetCommLog)) goto case 500;
                        retryCnt = 0;
                        break;

                    case 140://リアルタイムクロックチェック

                        if (checkBoxTestCase.Checked || Flags.RetryFlag)
                        {
                            if (!General.電源投入時の処理()) goto case 500;

                        }

                        Target.sendData(Target.PortName.P1_232C, "ReqTime");
                        sw.Stop();//ストップウォッチ停止

                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 10000)) goto case 500;

                        if (Target.RecieveData.IndexOf("Time") < 0) goto case 500;

                        sBuff = sw.Elapsed.ToString();
                        double hostTime = Double.Parse(sBuff.Substring(3, 2)) * 60;
                        hostTime += Double.Parse(sBuff.Substring(6));

                        sBuff = Target.RecieveData.Substring(5);// Targetから受信した時刻データから　"/yy/MM/dd/HH/mm/ss" を抽出

                        double buffTarget1;
                        double buffTarget2;
                        if (!General.ConvertTime(sBuff, out buffTarget1) || !General.ConvertTime(Constants.DefaultTime, out buffTarget2))
                        {
                            labelRtc.Text = "---";
                            labelRtc.ForeColor = Color.Red;
                            goto case 500;
                        }

                        double targetTime = buffTarget1 - buffTarget2;

                        dBuff = hostTime - targetTime;
                        if (dBuff < 0)
                        {
                            dBuff *= (-1);

                        }
                        labelRtc.Text = dBuff.ToString();

                        if (dBuff > State.TestSpec.RtcMax)
                        {
                            labelRtc.ForeColor = Color.Red;
                            goto case 500;
                        }
                        retryCnt = 0;
                        break;

                    case 150:    //S1チェック

                        if (checkBoxTestCase.Checked)
                        {
                            if (!General.電源投入時の処理()) goto case 500;
                        }

                        //S1奇数設定になっているかのチェック
                        if (!TEST_スイッチ.CheckS1(SwMode.奇数On, SetSwDisplay, SetCommLog))
                        {
                            General.ResetIo();
                            General.Wait(500);
                            labelMessage.Text = "プレス治具を開けてください！";
                            labelMessage.Refresh();
                            General.PlaySound2(Constants.NoticeSound);//

                            //プレスがあくまで待つ
                            for (;;)
                            {
                                Application.DoEvents();
                                if (General.checkStopButton()) goto case 500;
                                if (!General.CheckPress()) break;
                            }
                            General.StopSound();
                            labelMessage.Text = "S1の奇数番号をONして、プレスを閉めてください！";
                            labelMessage.Refresh();

                            //プレスが閉まるまで待つ
                            for (;;)
                            {
                                Application.DoEvents();
                                if (General.checkStopButton()) goto case 500;
                                if (General.CheckPress()) break;
                            }

                            General.Wait(500);
                            if (!General.電源投入時の処理()) goto case 500;
                            if (!TEST_スイッチ.CheckS1(SwMode.奇数On, SetSwDisplay, SetCommLog)) goto case 500;
                        }

                        //S1すべてONになっているかのチェック
                        General.ResetIo();
                        General.Wait(500);
                        labelMessage.Text = "プレス治具を開けてください！";
                        labelMessage.Refresh();
                        General.PlaySound2(Constants.NoticeSound);

                        //プレスがあくまで待つ
                        for (;;)
                        {
                            Application.DoEvents();
                            if (General.checkStopButton()) goto case 500;
                            if (!General.CheckPress()) break;
                        }
                        General.StopSound();
                        labelMessage.Text = "S1をすべてONして、プレスを閉めてください！";
                        labelMessage.Refresh();

                        //プレスが閉まるまで待つ
                        for (;;)
                        {
                            Application.DoEvents();
                            if (General.checkStopButton()) goto case 500;
                            if (General.CheckPress()) break;
                        }

                        General.Wait(500);
                        if (!General.電源投入時の処理()) goto case 500;

                        if (!TEST_スイッチ.CheckS1(SwMode.AllOn, SetSwDisplay, SetCommLog)) goto case 500;

                        //S1OFF設定になっているかのチェック
                        General.ResetIo();
                        General.Wait(500);
                        labelMessage.Text = "プレス治具を開けてください！";
                        labelMessage.Refresh();
                        General.PlaySound2(Constants.NoticeSound);

                        //プレスがあくまで待つ
                        for (;;)
                        {
                            Application.DoEvents();
                            if (General.checkStopButton()) goto case 500;
                            if (!General.CheckPress()) break;
                        }
                        General.StopSound();
                        labelMessage.Text = "S1をすべてOFFして、プレスを閉めてください！";
                        labelMessage.Refresh();

                        //プレスが閉まるまで待つ
                        for (;;)
                        {
                            Application.DoEvents();
                            if (General.checkStopButton()) goto case 500;
                            if (General.CheckPress()) break;
                        }

                        General.Wait(500);
                        if (!General.電源投入時の処理()) goto case 500;

                        if (!TEST_スイッチ.CheckS1(SwMode.AllOff, SetSwDisplay, SetCommLog)) goto case 500;

                        retryCnt = 0;
                        break;



                    case 160:    //スイッチ出荷設定の確認(SW1の出荷設定確認"F")


                        if (checkBoxTestCase.Checked || Flags.RetryFlag)
                        {
                            if (!General.電源投入時の処理()) goto case 500;

                        }


                        Target.sendData(Target.PortName.P1_232C, "ReadSw1_2");
                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) goto case 500;

                        sBuff = Target.RecieveData.Substring(8, 1);
                        labelSw1.Text = "SW1 [" + sBuff.ToUpper() + "]";
                        ShowSw1Picture(Convert.ToInt32(sBuff, 16));

                        if (sBuff.ToUpper() != "F") goto case 500;

                        break;

                    case 161:    //スイッチ出荷設定の確認(SW2の出荷設定確認"F")

                        Target.sendData(Target.PortName.P1_232C, "ReadSw1_2");
                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) goto case 500;

                        sBuff = Target.RecieveData.Substring(7, 1);
                        labelSw2.Text = "SW2 [" + sBuff.ToUpper() + "]";
                        ShowSw2Picture(Convert.ToInt32(sBuff, 16));
                        if (sBuff.ToUpper() != "F") goto case 500;

                        break;

                    case 162:    //スイッチ出荷設定の確認(S1の出荷設定確認"00")

                        //S1の出荷設定確認"00"
                        Target.sendData(Target.PortName.P1_232C, "ReadS1");
                        if (!Target.ReadRecieveData(Target.PortName.P1_232C, 1000)) goto case 500;

                        sBuff = Target.RecieveData.Substring(4);
                        iBuff = Convert.ToInt32(sBuff, 16);//
                        labelS1.Text = "S1 [" + sBuff.ToUpper() + "]";
                        ShowS1Picture(iBuff);
                        if (sBuff != "00") goto case 500;
                        retryCnt = 0;
                        break;

                    case 170:    //製品ソフト書き込み

                        General.ResetIo();
                        General.Wait(3000);

                        //E1を接続する
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                        General.Wait(3000);

                        if (!FlashDevelopmentToolkit.WriteFirmware(FlashDevelopmentToolkit.WriteMode.ProductMode)) goto case 500;
                        General.Wait(1500);
                        General.ResetIo();
                        retryCnt = 0;
                        break;

                    case 180:    //最終通電チェック

                        //USB-シリアル変換機に接続
                        General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                        General.Wait(1000);

                        if (!Target.GetInitData() || Target.RecieveData.IndexOf("@00RD2120000354") < 0)
                        {
                            if (retryCnt == 3)
                            {
                                //最終通電チェックで3回リトライしてＮＧの場合、試験ソフトを書き込みして終了する（最初の重複試験を通すため）
                                General.ResetIo();
                                General.Wait(3000);
                                //E1を接続する
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                                General.Wait(3000);

                                FlashDevelopmentToolkit.WriteFirmware(FlashDevelopmentToolkit.WriteMode.TestMode);
                                General.Wait(1500);
                                //E1を切断する
                                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                                General.Wait(500);
                            }
                            goto case 500;
                        }

                        General.Wait(1000);
                        break;

                    case 500:    //リトライ確認用

                        //検査ログにＮＧを書き込む
                        WriteNg();

                        //IOを初期化する処理（出力をすべてＬに落とす）
                        //リアルタイムクロック設定～チェック間のステップでＮＧリトライになった場合は電源をＯＦＦしない！（Targetの時刻データが消えるため）
                        if (State.StepNo != 120 && State.StepNo != 130)
                        {
                            General.ResetIo();
                        }

                        //重複試験チェック、ＳＤカードチェック、リアルタイムクロックでＮＧ、停止ボタンを押した場合はリトライしない
                        if (State.StepNo == 0 || State.StepNo == 20 || State.StepNo == 170 ||
                            State.StepNo == 100 || State.StepNo == 150 || Flags.StopFlag)
                        {
                            goto TEST_FAIL;
                        }

                        if (++retryCnt < Constants.RetryCount)
                        {
                            iBuff = State.StepNo % 10;
                            index -= iBuff;

                            Flags.RetryFlag = true;
                            goto ReTest;
                        }

                        retryCnt = 0;
                        //音鳴らして終了
                        General.PlaySound2(Constants.FailSound);

                        if (State.StepNo != 180)//step180でリトライ3回以上の場合は即テスト終了
                        {
                            result = MessageBox.Show("この項目はＮＧですがリトライしますか？", "作業者への確認", MessageBoxButtons.YesNo);
                            //何が選択されたか調べる
                            if (result == DialogResult.Yes)
                            {
                                iBuff = State.StepNo % 10;
                                index -= iBuff;

                                Flags.RetryFlag = true;
                                goto ReTest;
                            }
                            else
                            {
                                goto TEST_FAIL;
                            }
                        }
                        else
                        {
                            goto TEST_FAIL;
                        }

                    default: break;

                }　//switchの終わり

                //ステップ正常時の処理
                WriteOk();

                if (++index > MaxIndex) break;

            }//whileの終わり

            //試験合格時の処理（↑のwhile文を無事に抜けた場合のみ）

            //IOを初期化する処理（出力をすべてＬに落とす）
            General.ResetIo();

            if (!checkBoxTestCase.Checked)//全試験合格時は、検査データを保存し、合格スタンプを押す
            {
                var Data = new List<string>()
                {
                    textBoxOpeCode.Text,
                    comboBoxOperatorName.SelectedItem.ToString(),
                    System.DateTime.Now.ToString("yyyy年MM月dd日 HH時mm分ss秒"),
                    labelFirmVer.Text,
                    labelFirmSum.Text,
                    labelVccVol.Text,
                    labelVee.Text,
                    labelCurrent.Text,
                    labelRtc.Text,
                    labelTX1rgb.Text,
                    labelTX2rgb.Text,
                    labelTX3rgb.Text,
                    labelTX4rgb.Text,
                    labelRX1rgb.Text,
                    labelRX2rgb.Text,
                    labelRX3rgb.Text,
                    labelRX4rgb.Text,
                    labelCPUrgb.Text,
                    labelRUNrgb.Text,
                    labelVCCrgb.Text,
                    labelDIrgb.Text,
                    labelDOrgb.Text,
                    State.TestSpec.VccMin.ToString("F2") + "～" + State.TestSpec.VccMax.ToString("F2"),
                    State.TestSpec.VeeMin.ToString("F2") + "～" + State.TestSpec.VeeMax.ToString("F2"),
                    State.TestSpec.CurrMin.ToString("F2") + "～" + State.TestSpec.CurrMax.ToString("F2"),
                    State.TestSpec.RtcMax.ToString("F2")
                };

                if (!General.SaveData(Data))
                {
                    MessageBox.Show("検査データ生成に失敗しました\r\n" + "アプリケーションを強制終了します", "警告");
                    Environment.Exit(0);
                }

                //合格印押し
                General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, EPX64S.OUT.H);
                Thread.Sleep(500);
                General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, EPX64S.OUT.L);

            }

            //音鳴らして終了
            General.PlaySound2(Constants.PassSound);

            labelDecision.Text = "PASS";
            labelDecision.ForeColor = Color.LightSeaGreen;
            labelMessage.Text = Constants.MessRemove;


            //スタートボタンの切り替え
            buttonStart.Text = "確認";
            buttonStart.Enabled = true;
            timerLbMessage.Enabled = true;

            return;


            TEST_FAIL://試験不合格時の処理

            //音鳴らして終了
            General.PlaySound2(Constants.FailSound);

            labelDecision.Text = "FAIL";
            labelDecision.ForeColor = Color.Red;
            ShowErrorInfo();
            labelMessage.Text = Constants.MessRemove;


            //スタートボタンの切り替え
            buttonStart.Text = "確認";
            buttonStart.Enabled = true;
            timerLbMessage.Enabled = true;



            return;
        }


        private void ShowWarning()
        {
            this.Enabled = false;
            Flags.WarningFlag = true;

            //アイテム追加フォームの作成
            var warForm = new FormWarning();

            // Form をモーダルで表示する
            warForm.ShowDialog();

            // 不要になった時点で破棄する (正しくは オブジェクトの破棄を保証する を参照)
            warForm.Dispose();
            this.Refresh();
            this.Enabled = true;
        }



    }

}
