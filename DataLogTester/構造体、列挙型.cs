using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLogTester
{
    //*******************************************************************************
    //構造体の宣言
    //******************************************************************************* 

    //検査スペック用構造体
    public struct TestProperty
    {
        public int stepNo;          //ステップ№
        public string testCase;     //検査項目
        public string errorMessage; //検査項目に対するエラーメッセージ
    }

    //製品型番用構造体
    public struct Model
    {
        public string name;         //製品型式
        public string memory;       //製品型式に対するSDカードの容量
    }


    //*******************************************************************************
    //列挙型の宣言
    //******************************************************************************* 
    public enum LedName
    {
        TX1, TX2, TX3, TX4,
        RX1, RX2, RX3, RX4,
        VCC, RUN, CPU, DI, DO,
    }

    public enum BitNo
    {
        bit0, bit1, bit2, bit3, bit4, bit5, bit6, bit7, bit8, bit9,
        bitA, bitB, bitC, bitD, bitE, bitF,
    }

    public enum SwName
    {
        SW1, SW2, S1,
    }

    public enum ComNumber
    {
        COM1, COM2, COM3, COM4, COM5, COM6, COM7, COM8, COM9, COM10,
    }

    public enum SwMode
    { 奇数On, AllOn, AllOff}

    public enum Multimeter
    { AGI34401A, R6441b}





}
