using System.Net.EasyIp.Enums;

namespace System.Net.EasyIp.Helpers
{
    public static class PacketFactory
    { 
        public static EasyIpPacket GetReadPacket(short point, DataTypeEnum dataType, byte count)
        {
            return new EasyIpPacket
            {
                Flags = 0,
                Error = 0,
                Counter = 0, // Must increment in client
                SendDataType = 0,
                SendDataSize = 0,
                SendDataOffset = point,
                ReqDataType = dataType,
                ReqDataSize = count,
                ReqDataOffsetServer = point,
                ReqDataOffsetClient = 0
            };
        }

        public static EasyIpPacket GetWritePacket(short point, DataTypeEnum dataType, byte count)
        {
            return new EasyIpPacket
            {
                Flags = 0,
                Error = 0,
                Counter = 0, // Must increment in client
                SendDataSize = count,
                SendDataOffset = point,
                SendDataType = dataType,
                ReqDataSize = 0,
                ReqDataOffsetServer = point,
                ReqDataOffsetClient = 0
            };
        }
    }
}
