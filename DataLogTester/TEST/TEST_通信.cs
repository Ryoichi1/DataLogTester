using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLogTester
{
    public static class TEST_通信
    {
        public static bool Check422(Target.PortName pName)
        {
            string port = "";
            switch (pName)
            {
                case Target.PortName.P1_422:
                    port = "Port1@";
                    break;
                case Target.PortName.P2_422:
                    port = "Port2@";
                    break;
                case Target.PortName.P3_422:
                    port = "Port3@";
                    break;
                case Target.PortName.P4_422:
                    port = "Port4@";
                    break;
            }

            Target.sendData(pName, "ComCheck");
            if (!Target.ReadRecieveData(pName, 3000))
            {
                goto CommErr;
            }
            if (Target.RecieveData.IndexOf(port + "1234567890") < 0)
            {
                goto CommErr;
            }

            Target.sendData(pName, "ABCDEFGHIJ");
            if (!Target.ReadRecieveData(pName, 3000))
            {
                goto CommErr;
            }
            if (Target.RecieveData.IndexOf(port + "KLMNOPQRST") < 0)
            {
                goto CommErr;
            }
            return true;

        CommErr:
            return false;
        }

        public static bool Check232c(Target.PortName pName)
        {
            string port = "";
            switch (pName)
            {
                case Target.PortName.P1_232C:
                    port = "Port1@";
                    break;
                case Target.PortName.P2_232C:
                    port = "Port2@";
                    break;
                case Target.PortName.P3_232C:
                    port = "Port3@";
                    break;
                case Target.PortName.P4_232C:
                    port = "Port4@";
                    break;
            }

            Target.sendData(pName, "ComCheck");
            if (!Target.ReadRecieveData(pName, 3000))
            {
                goto CommErr;
            }
            if (Target.RecieveData.IndexOf(port + "1234567890") < 0)
            {
                goto CommErr;
            }

            Target.sendData(pName, "ABCDEFGHIJ");
            if (!Target.ReadRecieveData(pName, 3000))
            {
                goto CommErr;
            }
            if (Target.RecieveData.IndexOf(port + "KLMNOPQRST") < 0)
            {
                goto CommErr;
            }
            return true;

        CommErr:
            return false;
        }

    }
}
