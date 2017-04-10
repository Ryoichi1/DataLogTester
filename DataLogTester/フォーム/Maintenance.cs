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
    public partial class Maintenance : Form
    {
        // ---------------- DLL Imports (USBカメラの設定画面表示用)--------------------
        [DllImport("olepro32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int OleCreatePropertyFrame(
        IntPtr hwndOwner, int x, int y,
        string lpszCaption, int cObjects,
        [In, MarshalAs(UnmanagedType.Interface)] ref object ppUnk,
        int cPages, IntPtr pPageClsID, int lcid, int dwReserved, IntPtr pvReserved);

        
       
        public Maintenance()
        {
            InitializeComponent();
        }

        //S2押し
        private void buttonSolenoid1_Click(object sender, EventArgs e)
        {
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.H);
            General.Wait(500);
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b0, EPX64S.OUT.L);
        }
        
        //合格印押し
        private void buttonSolenoid2_Click(object sender, EventArgs e)
        {
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, EPX64S.OUT.H);
            General.Wait(500);
            General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b1, EPX64S.OUT.L);


        }

        private void Maintenance_Load(object sender, EventArgs e)
        {
            buttonRs232c.Text = "RS232C 接続";
            buttonRs422.Text = "RS422 接続";
            buttonPower.Text = "電源 ON";
            buttonLight.Text = "照明 ON";
            buttonE1emulator.Text = "E1エミュレータ 接続";
        }


        private void Maintenance_FormClosed(object sender, FormClosedEventArgs e)
        {
            General.ResetIo();
        }


        private void buttonCameraProperty_Click(object sender, EventArgs e)
        {
            ISpecifyPropertyPages specifyPropertyPages;//USBカメラの画像補正系設定ページ

            //キャプチャフィルタからspecifyPropertyPageを取得
            specifyPropertyPages = (ISpecifyPropertyPages)General.cam.capFilter;

            DsCAUUID cauuid = new DsCAUUID();

            int handle = specifyPropertyPages.GetPages(out cauuid);
            if (handle != 0) Marshal.ThrowExceptionForHR(handle);

            object o = specifyPropertyPages;
            handle = OleCreatePropertyFrame(this.Handle, 30, 30, null, 1,
                  ref o, cauuid.cElems, cauuid.pElems, 0, 0, IntPtr.Zero);


        }

        private void buttonRs422_Click(object sender, EventArgs e)
        {
            if (buttonRs422.Text == "RS422 接続")
            {
                //ｺﾐﾆｯﾁ接続(K13-K20<P10,P11>) + RS422ジャンパ接続(K21-K24<P12>) 
                General.io.OutByte(EPX64S.PORT.P1, 0x07);
                buttonRs422.Text = "RS422 切断";
            }
            else
            {
                General.io.OutByte(EPX64S.PORT.P1, 0x00);
                buttonRs422.Text = "RS422 接続";
            }
        }

        private void buttonRs232c_Click(object sender, EventArgs e)
        {
            if (buttonRs232c.Text == "RS232C 接続")
            {
                //USB-シリアル変換機に接続
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.H);
                buttonRs232c.Text = "RS232C 切断";
            }
            else
            {
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b5, EPX64S.OUT.L);
                buttonRs232c.Text = "RS232C 接続";
            }
        }

        private void buttonPower_Click(object sender, EventArgs e)
        {
            if (buttonPower.Text == "電源 ON")
            {
                General.PowerOn();
                buttonPower.Text = "電源 OFF";
            }
            else
            {
                General.PowerOff();
                buttonPower.Text = "電源 ON";
            }


        }

        private void buttonLight_Click(object sender, EventArgs e)
        {
            if (buttonLight.Text == "照明 ON")
            {
                General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, EPX64S.OUT.H);
                buttonLight.Text = "照明 OFF";
            }
            else
            {
                General.io.OutBit(EPX64S.PORT.P2, EPX64S.BIT.b2, EPX64S.OUT.L);
                buttonLight.Text = "照明 ON";
            }
        }

        private void buttonE1emulator_Click(object sender, EventArgs e)
        {
            if (buttonE1emulator.Text == "E1エミュレータ 接続")
            {
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.H);
                buttonE1emulator.Text = "E1エミュレータ 切断";
            }
            else
            {
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b4, EPX64S.OUT.L);
                buttonE1emulator.Text = "E1エミュレータ 接続";
            }


        }

        private void buttonLedOn_Click(object sender, EventArgs e)
        {
            if (buttonLedOn.Text == "LEDを全点灯させる")
            {
                General.ResetIo();
                General.Wait(500);

                //ｺﾐﾆｯﾁ接続(K13-K20<P10,P11>) + RS422ジャンパ接続(K21-K24<P12>) 
                General.io.OutByte(EPX64S.PORT.P1, 0x07);
                General.Wait(500);

                //電源ON
                General.PowerOn();
                General.Wait(3000);

                //LED全点灯にするため、K4をONする
                General.io.OutBit(EPX64S.PORT.P0, EPX64S.BIT.b3, EPX64S.OUT.H);
                General.Wait(500);

                //ターゲットにコマンド送信
                Target.sendData(Target.PortName.P1_422, "LedCheck",false);//このコマンドのみ返信なし
                General.Wait(2000);//正常ならここで全点してるはず

                buttonLedOn.Text = "LEDを消灯させる";
            }
            else
            {
                General.ResetIo();
                buttonLedOn.Text = "LEDを全点灯させる";
            
            }
            
        }






   
    
    }
}
