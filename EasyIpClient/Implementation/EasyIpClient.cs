using System.Net.EasyIp.Common;
using System.Net.EasyIp.Enums;
using System.Net.EasyIp.Extensions;
using System.Net.EasyIp.Helpers;
using System.Net.EasyIp.Interfaces;
using System.Runtime.InteropServices;

namespace System.Net.EasyIp
{
    /// <summary>
    /// Encapsulate EasyIP protocol
    /// </summary>
    public sealed class EasyIpClient: Disposable, IEasyIpClient
    {
        private IChannel _channel;
        public EasyIpClient(IChannel channel)
        {
            _channel = channel;
        }

        /// <summary>
        /// Read data block from PLC
        /// </summary>
        /// <typeparam name="T">Any value type, e.g. short, int, double, etc.</typeparam>
        /// <param name="point">Data offset, start address in PLC</param>
        /// <param name="dataType">Type of requested data, e.g. Flags, Registers, etc.</param>
        /// <param name="length">Count of elements.
        /// Maximum depends from desired data type. 
        /// Result data size can't be great than 256 words</param>
        /// <returns>Array of T</returns>
        public T[] BlockRead<T>(short point, DataTypeEnum dataType, byte length)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            byte count = Convert.ToByte(length * typeSize / Constants.SHORT_SIZE);
            var packet = PacketFactory.GetReadPacket(point, dataType, count);
            byte[] recvBuffer = _channel.Execute(packet.ToByteArray());
            int dataLen = recvBuffer.Length - Constants.EASYIP_HEADERSIZE;
            int retLen = dataLen / typeSize;
            T[] ret = new T[retLen];
            Buffer.BlockCopy(recvBuffer, Constants.EASYIP_HEADERSIZE, ret, 0, dataLen);
            return ret;
        }

        /// <summary>
        /// Write data block to PLC
        /// </summary>
        /// <typeparam name="T">Any value type, e.g. short, int, double, etc.</typeparam>
        /// <param name="point">Data offset, start address in PLC</param>
        /// <param name="val">Array of T</param>
        /// <param name="dataType">Type of requested data, e.g. Flags, Registers, etc.</param>
        public void BlockWrite<T>(short point, T[] val, DataTypeEnum dataType)
        {
            int typeSize = Marshal.SizeOf(typeof(T));
            byte count = Convert.ToByte(val.Length * typeSize / Constants.SHORT_SIZE);
            var packet = PacketFactory.GetWritePacket(point, dataType, count);
            var sendBuffer = packet.ToByteArray();
            Buffer.BlockCopy(val, 0, sendBuffer, Constants.EASYIP_HEADERSIZE, packet.SendDataSize * Constants.SHORT_SIZE);
            var recvBuffer = _channel.Execute(sendBuffer);
        }

        ~EasyIpClient()
        {
            Dispose(false);
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            IsDisposed = true;
            if (disposing)
            {
                if (_channel != null)
                {
                    _channel.Dispose();
                }
                GC.SuppressFinalize(this);
            }

            base.Dispose(disposing);
            // free unmanaged resources here
        }
    }
}
