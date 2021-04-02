using System.IO;
using System.Net.EasyIp.Common;

namespace System.Net.EasyIp.Extensions
{
    public static class PacketExtensions
    {
        public static byte[] ToByteArray(this EasyIpPacket packet)
        {
            var _buffer = new byte[Constants.EASYIP_HEADERSIZE + packet.SendDataSize * Constants.SHORT_SIZE];
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(packet.Flags);
                    writer.Write(packet.Error);
                    writer.Write(packet.Counter);
                    writer.Write(packet.Spare1);
                    writer.Write((byte)packet.SendDataType);
                    writer.Write(packet.SendDataSize);
                    writer.Write(packet.SendDataOffset);
                    writer.Write(packet.Spare2);
                    writer.Write((byte)packet.ReqDataType);
                    writer.Write(packet.ReqDataSize);
                    writer.Write(packet.ReqDataOffsetServer);
                    writer.Write(packet.ReqDataOffsetClient);
                }
                Buffer.BlockCopy(stream.ToArray(), 0, _buffer, 0, Constants.EASYIP_HEADERSIZE);
            }

            if (packet.SendDataSize > 0)
            {
                Buffer.BlockCopy(packet.Data, 0, _buffer, Constants.EASYIP_HEADERSIZE, packet.SendDataSize * Constants.SHORT_SIZE);
            }
            return _buffer;
        }
    }
}
