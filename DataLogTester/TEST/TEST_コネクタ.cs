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
    public static class TEST_コネクタ
    {
        //各コネクタの合格フラグ
        public static bool Port1Flag { get; private set; }
        public static bool Port2Flag { get; private set; }
        public static bool Port3Flag { get; private set; }
        public static bool Port4Flag { get; private set; }
        public static bool Cn1Flag { get; private set; }
        public static bool Cn3Flag { get; private set; }
                
        
        
        //**************************************************************************
        //コネクター実装チェック
        //引数：なし
        //戻値：なし
        //**************************************************************************
        public static bool CheckConnector()
        {
            Port1Flag = false;
            Port2Flag = false;
            Port3Flag = false;
            Port4Flag = false;
            Cn1Flag = false;
            Cn3Flag = false;

            if (!General.io.ReadInputData(EPX64S.PORT.P3) )
            {
                Flags.Epx64Stutas = false;
                return false;
            }

            byte buff;

            //Port1チェック
            buff = General.io.P3InputData;
            if ((buff & 0x02) == 0x00)
            {
                Port1Flag = true;
            }

            //Port2チェック
            buff = General.io.P3InputData;
            if ((buff & 0x04) == 0x00)
            {
                Port2Flag = true;
            }

            //Port3チェック
            buff = General.io.P3InputData;
            if ((buff & 0x08) == 0x00)
            {
                Port3Flag = true;
            }

            //Port4チェック
            buff = General.io.P3InputData;
            if ((buff & 0x10) == 0x00)
            {
                Port4Flag = true;
            }

            //CN1チェック
            buff = General.io.P3InputData;
            if ((buff & 0x20) == 0x00)
            {
                Cn1Flag = true;
            }

            //CN3チェック
            buff = General.io.P3InputData;
            if ((buff & 0x40) == 0x00)
            {
                Cn3Flag = true;
            }

            if (Port1Flag && Port2Flag && Port3Flag && Port4Flag && Cn1Flag && Cn3Flag)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    
    
    
    
    
    
    }
}
