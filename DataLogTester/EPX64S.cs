using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// EPX-64S API library definitions
/// </summary>

namespace DataLogTester
{

    public class EPX64S
    {
        // Function definitions
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_GetNumberOfDevices(ref int Number);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_GetSerialNumber(int Index, ref int SerialNumber);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_Open(ref System.IntPtr Handle);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_OpenBySerialNumber(int SerialNumber, ref System.IntPtr Handle);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_Close(System.IntPtr Handle);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_SetDirection(System.IntPtr Handle, byte Direction);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_GetDirection(System.IntPtr Handle, ref byte Direction);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_OutputPort(System.IntPtr Handle, byte Port, byte Value);
        [DllImport("EPX64S.dll")]
        public static extern int EPX64S_InputPort(System.IntPtr Handle, byte Port, ref byte Value);


        // Device status (Return codes)
        public const int EPX64S_OK = 0;
        public const int EPX64S_INVALID_HANDLE = 1;
        public const int EPX64S_DEVICE_NOT_FOUND = 2;
        public const int EPX64S_DEVICE_NOT_OPENED = 3;
        public const int EPX64S_OTHER_ERROR = 4;
        public const int EPX64S_COMMUNICATION_ERROR = 5;
        public const int EPX64S_INVALID_PARAMETER = 6;

        // Constants
        public const byte EPX64S_PORT0 = 0x01;
        public const byte EPX64S_PORT1 = 0x02;
        public const byte EPX64S_PORT2 = 0x04;
        public const byte EPX64S_PORT3 = 0x08;
        public const byte EPX64S_PORT4 = 0x10;
        public const byte EPX64S_PORT5 = 0x20;
        public const byte EPX64S_PORT6 = 0x40;
        public const byte EPX64S_PORT7 = 0x80;


        //列挙型
        public enum PORT
        {
            P0, P1, P2, P3, P4, P5, P6, P7,
        }

        public enum BIT
        {
            b0, b1, b2, b3, b4, b5, b6, b7,
        }

        public enum OUT
        {
            H, L,
        }


        //プロパティ

        //インスタンス変数宣言（生成するインスタンス固有の値）
        private System.IntPtr hDevice;   //Device Handle（ポインタへのポインタ）
        public int Status { get; private set; }   // Device Status (Return Code)
        private int Number;              // Devices Number
        private byte Direction;          //各ポート 入力or出力の設定用パラメータ
        private byte Port;               //入出力のポート指定用パラメータ
        private byte InputValue;         //指定ポートから読み込んだデータ

        public byte P0InputData { get; private set; } //ポート0の入力データバッファ
        public byte P1InputData { get; private set; } //ポート1の入力データバッファ
        public byte P2InputData { get; private set; } //ポート2の入力データバッファ
        public byte P3InputData { get; private set; } //ポート3の入力データバッファ
        public byte P4InputData { get; private set; } //ポート4の入力データバッファ
        public byte P5InputData { get; private set; } //ポート5の入力データバッファ
        public byte P6InputData { get; private set; } //ポート6の入力データバッファ
        public byte P7InputData { get; private set; } //ポート7の入力データバッファ

        public byte p0Outdata { get; private set; } //現在のポート０に出力されているデータ
        public byte p1Outdata { get; private set; } //現在のポート１に出力されているデータ
        public byte p2Outdata { get; private set; } //現在のポート２に出力されているデータ
        public byte p3Outdata { get; private set; } //現在のポート３に出力されているデータ
        public byte p4Outdata { get; private set; } //現在のポート４に出力されているデータ
        public byte p5Outdata { get; private set; } //現在のポート５に出力されているデータ
        public byte p6Outdata { get; private set; }  //現在のポート６に出力されているデータ
        public byte p7Outdata { get; private set; }  //現在のポート７に出力されているデータ

        //インスタンスコンストラクタ
        public EPX64S()
        {
            //インスタンス変数の初期化
            hDevice = System.IntPtr.Zero;   // Device Handle
            Status = 1; // 初期化時は0以外としておく
            Number = 0; // Devices Number
            Direction = 0;
            Port = 0;
            InputValue = 0;

            P0InputData = 0; //現在のポート０に出力されているデータ
            P1InputData = 0; //現在のポート１に出力されているデータ
            P2InputData = 0; //現在のポート２に出力されているデータ
            P3InputData = 0; //現在のポート３に出力されているデータ
            P4InputData = 0; //現在のポート４に出力されているデータ
            P5InputData = 0; //現在のポート５に出力されているデータ
            P6InputData = 0; //現在のポート６に出力されているデータ
            P7InputData = 0; //現在のポート７に出力されているデータ

            p0Outdata = 0; //現在のポート０に出力されているデータ
            p1Outdata = 0; //現在のポート１に出力されているデータ
            p2Outdata = 0; //現在のポート２に出力されているデータ
            p3Outdata = 0; //現在のポート３に出力されているデータ
            p4Outdata = 0; //現在のポート４に出力されているデータ
            p5Outdata = 0; //現在のポート５に出力されているデータ
            p6Outdata = 0; //現在のポート６に出力されているデータ
            p7Outdata = 0; //現在のポート７に出力されているデータ
        }

        //**************************************************************************
        //EPX64の初期化
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public bool InitEpx64S(byte DirectionData)
        {
            try
            {
                // Device Number
                EPX64S_GetNumberOfDevices(ref this.Number);
                if (this.Number == 0) return false;

                // Device Open
                this.Status = EPX64S_Open(ref this.hDevice);
                if (this.Status != EPX64S_OK) return false;

                // Direction 0=入力 1=出力
                this.Direction = DirectionData;//例0x07→P7:0 P6:0 P5:0 P4:0 P3:0 P2:1 P1:1 P0:1
                this.Status = EPX64S_SetDirection(this.hDevice, this.Direction);
                return (this.Status == EPX64S_OK);
            }
            catch
            {
                return false;
            }

        }

        //****************************************************************************
        //メソッド名：ReadInputData（指定ポートのデータを取り込む）
        //引数：ポート名（"P0"、"P1"・・・"P7"）
        //戻り値：取り込んだデータ正常０、異常１
        //****************************************************************************
        public bool ReadInputData(PORT pName)
        {
            try
            {
                switch (pName)
                {
                    case PORT.P0:
                        this.Port = EPX64S_PORT0; // PORT0
                        break;
                    case PORT.P1:
                        this.Port = EPX64S_PORT1; // PORT1
                        break;
                    case PORT.P2:
                        this.Port = EPX64S_PORT2; // PORT2                   
                        break;
                    case PORT.P3:
                        this.Port = EPX64S_PORT3; // PORT3                   
                        break;
                    case PORT.P4:
                        this.Port = EPX64S_PORT4; // PORT4                   
                        break;
                    case PORT.P5:
                        this.Port = EPX64S_PORT5; // PORT5                  
                        break;
                    case PORT.P6:
                        this.Port = EPX64S_PORT6; // PORT6                   
                        break;
                    case PORT.P7:
                        this.Port = EPX64S_PORT7; // PORT7                     
                        break;
                    default:
                        break;
                }

                // Input
                foreach (var i in Enumerable.Range(1, 2))
                {
                    this.Status = EPX64S_InputPort(this.hDevice, this.Port, ref this.InputValue);
                    if (Status == EPX64S_OK)
                        break;
                    if (i == 2)
                    {
                        return false;
                    }                
                }

                switch (pName)
                {
                    case PORT.P0:
                        this.P0InputData = this.InputValue;
                        break;
                    case PORT.P1:
                        this.P1InputData = this.InputValue;
                        break;
                    case PORT.P2:
                        this.P2InputData = this.InputValue;
                        break;
                    case PORT.P3:
                        this.P3InputData = this.InputValue;
                        break;
                    case PORT.P4:
                        this.P4InputData = this.InputValue;
                        break;
                    case PORT.P5:
                        this.P5InputData = this.InputValue;
                        break;
                    case PORT.P6:
                        this.P6InputData = this.InputValue;
                        break;
                    case PORT.P7:
                        this.P7InputData = this.InputValue;
                        break;
                }
                return true;
            }
            catch
            {
                this.Status = EPX64S.EPX64S_COMMUNICATION_ERROR;
                return false;
            }


        }



        //****************************************************************************
        //メソッド名：OutByte（指定ポートにバイト単位での出力）
        //引数：引数①　ポート名（"P0"、"P1"・・・"P7"）引数②出力値（0x00～0xFF）
        //戻り値：正常０、異常１
        //****************************************************************************
        public bool OutByte(PORT pName, byte Data)
        {
            try
            {
                //ポートの特定と出力データバッファの更新
                switch (pName)
                {
                    case PORT.P0:
                        this.Port = EPX64S_PORT0;
                        this.p0Outdata = Data;
                        break;
                    case PORT.P1:
                        this.Port = EPX64S_PORT1;
                        this.p1Outdata = Data;
                        break;
                    case PORT.P2:
                        this.Port = EPX64S_PORT2;
                        this.p2Outdata = Data;
                        break;
                    case PORT.P3:
                        this.Port = EPX64S_PORT3;
                        this.p3Outdata = Data;
                        break;
                    case PORT.P4:
                        this.Port = EPX64S_PORT4;
                        this.p4Outdata = Data;
                        break;
                    case PORT.P5:
                        this.Port = EPX64S_PORT5;
                        this.p5Outdata = Data;
                        break;
                    case PORT.P6:
                        this.Port = EPX64S_PORT6;
                        this.p6Outdata = Data;
                        break;
                    case PORT.P7:
                        this.Port = EPX64S_PORT7;
                        this.p7Outdata = Data;
                        break;
                    default:
                        break;
                }

                //データの出力
                this.Status = EPX64S_OutputPort(this.hDevice, this.Port, Data);
                return (Status == EPX64S_OK) ? true : false;

            }
            catch
            {
                this.Status = EPX64S.EPX64S_COMMUNICATION_ERROR;
                return false;

            }
        }

        //****************************************************************************
        //メソッド名：OutBit（指定ポートにビット単位での出力）
        //引数：引数①　ポート名（"P00"、"P01"・・・"P07"）引数②出力値（H：0/ L: 1）
        //戻り値：bool
        //****************************************************************************
        public bool OutBit(PORT pName, BIT bName, OUT data)
        {

            byte buffOutData = 0; //現時点で出力しているデータ

            try
            {

                //ポートの特定
                switch (pName)
                {
                    case PORT.P0:
                        this.Port = EPX64S_PORT0;
                        buffOutData = this.p0Outdata;
                        break;
                    case PORT.P1:
                        this.Port = EPX64S_PORT1;
                        buffOutData = this.p1Outdata;
                        break;
                    case PORT.P2:
                        this.Port = EPX64S_PORT2;
                        buffOutData = this.p2Outdata;
                        break;
                    case PORT.P3:
                        this.Port = EPX64S_PORT3;
                        buffOutData = this.p3Outdata;
                        break;
                    case PORT.P4:
                        this.Port = EPX64S_PORT4;
                        buffOutData = this.p4Outdata;
                        break;
                    case PORT.P5:
                        this.Port = EPX64S_PORT5;
                        buffOutData = this.p5Outdata;
                        break;
                    case PORT.P6:
                        this.Port = EPX64S_PORT6;
                        buffOutData = this.p6Outdata;
                        break;
                    case PORT.P7:
                        this.Port = EPX64S_PORT7;
                        buffOutData = this.p7Outdata;
                        break;
                    default:
                        break;
                }

                //ビットの特定
                byte Temp = 0;
                int Num = 0;

                switch (bName)
                {
                    case BIT.b0:
                        Num = 0; Temp = 0xFE;
                        break;
                    case BIT.b1:
                        Num = 1; Temp = 0xFD;
                        break;
                    case BIT.b2:
                        Num = 2; Temp = 0xFB;
                        break;
                    case BIT.b3:
                        Num = 3; Temp = 0xF7;
                        break;
                    case BIT.b4:
                        Num = 4; Temp = 0xEF;
                        break;
                    case BIT.b5:
                        Num = 5; Temp = 0xDF;
                        break;
                    case BIT.b6:
                        Num = 6; Temp = 0xBF;
                        break;
                    case BIT.b7:
                        Num = 7; Temp = 0x7F;
                        break;
                }


                //データの出力
                byte Data = 0;
                switch (data)
                {
                    case OUT.H:
                        Data = 1;
                        break;

                    case OUT.L:
                        Data = 0;
                        break;
                }

                //指定ビット以外はそのままにして出力データを決定する
                byte OutputValue = (byte)((buffOutData & Temp) | (Data << Num));//byteでｷｬｽﾄしないと怒られる

                //出力データバッファの更新
                switch (pName)
                {
                    case PORT.P0:
                        this.p0Outdata = OutputValue;
                        break;
                    case PORT.P1:
                        this.p1Outdata = OutputValue;
                        break;
                    case PORT.P2:
                        this.p2Outdata = OutputValue;
                        break;
                    case PORT.P3:
                        this.p3Outdata = OutputValue;
                        break;
                    case PORT.P4:
                        this.p4Outdata = OutputValue;
                        break;
                    case PORT.P5:
                        this.p5Outdata = OutputValue;
                        break;
                    case PORT.P6:
                        this.p6Outdata = OutputValue;
                        break;
                    case PORT.P7:
                        this.p7Outdata = OutputValue;
                        break;
                }

                //データ出力
                this.Status = EPX64S_OutputPort(this.hDevice, Port, OutputValue);
                return (Status == EPX64S_OK) ? true : false;

            }
            catch
            {
                this.Status = EPX64S.EPX64S_COMMUNICATION_ERROR;
                return false;
                
            }
        }


        //****************************************************************************
        //メソッド名：Close（ポートを閉じる処理）
        //引数：なし
        //戻り値：bool
        //****************************************************************************
        public bool Close()
        {
            try
            {
                EPX64S_Close(this.hDevice);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
