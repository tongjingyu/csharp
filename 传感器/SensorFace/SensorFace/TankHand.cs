using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SensorFace
{
    public partial class TankHand : Form
    {
        private int KeyFlag = 0;
        SensorFace.ACFF Mode = new SensorFace.ACFF();
        private const int KFCC_Start = 1, KFCC_Stop = 2;
        MB TxModBus = new MB();
        MB RxModBus = new MB();
        public TankHand()
        {
            InitializeComponent();
        }

        private void button29_Click(object sender, EventArgs e)
        {
        
        }
        private void FillValue()
        {
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref TxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref RxModBus);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadRxModBusMsg);
            ModBusClass.ModBusCoppyCreate(ref ModBusClass.DefMoBus, ref Usart.ThreadTxModBusMsg);

        }

        private void TankHand_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
            byte[] Data = new byte[100];
            switch (KeyFlag)
            {
                case KFCC_Start:
                    Mode = ACFF.SCFF_SetCTRBIT;
                    break;
                case KFCC_Stop:
                    break;
                default:break;
            }
            Sensor.SendReadSensor(Mode, Data, 1);
            // string Msg = SendReadSensor(Mode, Data, 1);
        }
    }
}
