using System.Net.EasyIp.Common;
using System.Net.EasyIp.Interfaces;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace System.Net.EasyIp
{
    /// <summary>
    /// Encapsulate channel layer for communication via UDP protocol
    /// </summary>
    public sealed class UdpChannel : Disposable, IChannel
    {
        private UdpClient _client;
        private IPEndPoint _endPoint;

        public UdpChannel(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _client = new UdpClient();
            _client.Connect(endPoint);
        }

        public UdpChannel(string host, int port)
        {
            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
            _client = new UdpClient();
            _client.Connect(_endPoint);
        }

        public byte[] Execute(byte[] buffer)
        {
            var buff = new byte[256];
            _client.Send(buffer, buffer.Length);
            do
            {
                buff = _client.Receive(ref _endPoint);
            } while (_client.Available > 0);
            return buff;
        }

        public async Task<byte[]> ExecuteAsync(byte[] buffer)
        {
            await _client.SendAsync(buffer, buffer.Length);
            var result = await _client.ReceiveAsync();
            return result.Buffer;
        }

        public int SendTimeout
        {
            get
            {
                return _client.Client.SendTimeout;
            }
            set
            {
                _client.Client.SendTimeout = value;
            }
        }

        public int ReceiveTimeout
        {
            get
            {
                return _client.Client.ReceiveTimeout;
            }
            set
            {
                _client.Client.ReceiveTimeout = value;
            }
        }

        ~UdpChannel()
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
                if (_client != null)
                {
                    _client.Close();
                    _client = null;
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
