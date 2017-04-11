using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLogTester
{
    public class TestSpec
    {
        public string TestSpecVer { get; set; }

        //①製品ソフトウェア情報
        public  string FirmVer { get;  set; }
        public  string FirmSum { get;  set; }
        public string FirmName { get;  set; }

        //②試験スペック
        public  double VccMax { get;  set; }     //電源電圧5V上限
        public  double VccMin { get;  set; }     //電源電圧5V下限
        public  double VeeMax { get;  set; }     //電源電圧3.3V上限
        public  double VeeMin { get;  set; }      //電源電圧3.3V下限
        public  double CurrMax { get;  set; }     //消費電流上限
        public  double CurrMin { get;  set; }  //消費電流下限
        public  double RtcMax { get;  set; }      //リアルタイムクロック誤差上限
        public  float Led_H_Max { get;  set; }
        public  float Led_H_Min { get;  set; }

        public List<string> SdInfo { get; set; }//各アイテムのSDカード容量

    }
}
