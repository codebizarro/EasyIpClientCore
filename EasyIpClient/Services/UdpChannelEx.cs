using System.Net.EasyIp.Common;
using System.Net.EasyIp.Interfaces;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System.Net.EasyIp
{
    /// <summary>
    /// Encapsulate channel layer for communication via UDP protocol
    /// </summary>
    [Obsolete]
    public sealed class UdpChannelEx : Disposable, IChannel
    {
        private Socket _socket;
        private IPEndPoint _endPoint;

        public UdpChannelEx(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(endPoint);
        }

        public UdpChannelEx(string host, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(_endPoint);
        }

        public byte[] Execute(byte[] buffer)
        {
            _socket.SendTo(buffer, buffer.Length, SocketFlags.None, _endPoint);
            var recvBuffer = new byte[1024];
            var endPoint = (EndPoint)_endPoint;
            var recvLength = _socket.ReceiveFrom(recvBuffer, SocketFlags.None, ref endPoint);
            Array.Resize<byte>(ref recvBuffer, recvLength);
            return recvBuffer;
        }

        public Task<byte[]> ExecuteAsync(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public int SendTimeout
        {
            get
            {
                return int.Parse(_socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout).ToString());
            }
            set
            {
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, value);

            }
        }

        public int ReceiveTimeout
        {
            get
            {
                return int.Parse(_socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout).ToString());
            }
            set
            {
                _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, value);
            }
        }

        ~UdpChannelEx()
        {
            Dispose(false);
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            if (disposing)
            {
                if (_socket != null)
                {
                    _socket.Close();
                    _socket = null;
                }
                GC.SuppressFinalize(this);
            }

            // Free any unmanaged objects here.
            //
            IsDisposed = true;

            base.Dispose(disposing);
        }
    }
}
