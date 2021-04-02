using System.Threading.Tasks;

namespace System.Net.EasyIp.Interfaces
{
    public interface IChannel: IDisposable
    {
        byte[] Execute(byte[] buffer);

        Task<byte[]> ExecuteAsync(byte[] buffer);

        int SendTimeout
        {
            get;
            set;
        }

        int ReceiveTimeout
        {
            get;
            set;
        }
    }
}
