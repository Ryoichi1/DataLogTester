using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DataLogTester
{
    public static class Flags
    {
        //各種フラグ

        public static bool Epx64Stutas;   //EPX64のステータス
        public static bool StopFlag;      //停止ボタンが押されたかどうかのフラグ(押されたらTrue)
        public static bool PowerFlag;     //製品に電源供給されているかどうかのフラグ（供給時True）
        public static bool RetryFlag;     //試験ＮＧの際にリトライした場合true
        public static bool WarningFlag;     //警告画面表示用
        


    
    
    }

}
