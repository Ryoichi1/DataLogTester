using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLogTester
{
    //マルチメータを34401AとR6441Bの両方に対応するためのラッパクラス
    public static class 電流電圧チェック
    {
        public static double GetDcV()
        {
            switch (State.ItemMultimeter)
            { 
                case Multimeter.AGI34401A:
                        Agilent34401A.GetDcVoltage();
                        return Agilent34401A.VoltData;

                case Multimeter.R6441b:
                        R6441b.SetDcV();
                        R6441b.GetDcV();
                        return R6441b.VoltData;

                default: return 0;
            }
        }

        public static double GetDcA()
        {
            switch (State.ItemMultimeter)
            {
                case Multimeter.AGI34401A:
                    Agilent34401A.GetDcCurrent();
                    return Agilent34401A.CurrData * 1000;

                case Multimeter.R6441b:
                    R6441b.SetDcA();
                    R6441b.GetDcA();
                    return R6441b.AmpData * 1000;

                default: return 0;
            }
        }






    }
}
