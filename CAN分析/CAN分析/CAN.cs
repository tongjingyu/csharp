using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAN分析
{
    //    struct
    //{
    //     uint32 StdId;  /*!< Specifies the standard identifier.
    //                        This parameter can be a value between 0 to 0x7FF. */

    //    uint32_t ExtId;  /*!< Specifies the extended identifier.
    //                        This parameter can be a value between 0 to 0x1FFFFFFF. */

    //    uint8_t IDE;     /*!< Specifies the type of identifier for the message that will be received.
    //                        This parameter can be a value of @ref CAN_identifier_type */

    //    uint8_t RTR;     /*!< Specifies the type of frame for the received message.
    //                        This parameter can be a value of @ref CAN_remote_transmission_request */

    //    uint8_t DLC;     /*!< Specifies the length of the frame that will be received.
    //                        This parameter can be a value between 0 to 8 */

    //    uint8_t Data[8]; /*!< Contains the data to be received. It ranges from 0 to 0xFF. */

    //    uint8_t FMI;     /*!< Specifies the index of the filter the message stored in the mailbox passes through.
    //                        This parameter can be a value between 0 to 0xFF */
    //}
    //CanRxMsg;

    struct CanRxMsg
    {
        public UInt32 StdId;
        public UInt32 ExtId;
        public byte IDE;
        public byte RTR;
        public byte DLC;
        public byte[] Data;
        public byte FMI;
    }
    class CAN
    {
        
         public static CanRxMsg CAN_Export(byte[] Buf, UInt32 OffSet)
        {
            CanRxMsg Msg=new CanRxMsg();
            Msg.StdId = Tools.ByteToU32(Buf, OffSet, 0); OffSet += 4;
            Msg.ExtId = Tools.ByteToU32(Buf, OffSet, 0); OffSet += 4;
            Msg.IDE = Buf[OffSet++];
            Msg.RTR = Buf[OffSet++]; 
            Msg.DLC = Buf[OffSet++];
            Msg.Data = new byte[Msg.DLC];
            for (int i = 0; i < Msg.DLC; i++) Msg.Data[i] = Buf[OffSet++];
            return Msg;
        }
        public static void CAN_Create(ref byte[] Buf, CanRxMsg Msg)
        {

        }

    }
}
