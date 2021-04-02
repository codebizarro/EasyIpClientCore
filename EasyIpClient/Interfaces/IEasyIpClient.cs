using System.Net.EasyIp.Enums;

namespace System.Net.EasyIp.Interfaces
{
    public interface IEasyIpClient : IDisposable
    {
        T[] BlockRead<T>(short point, DataTypeEnum dataType, byte length);
        void BlockWrite<T>(short point, T[] val, DataTypeEnum dataType);
    }
}
