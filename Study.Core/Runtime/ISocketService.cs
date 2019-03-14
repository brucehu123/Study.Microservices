using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Runtime
{
    public interface ISocketService
    {
        void OnConnected();
        void OnDisconnected();
        void OnReceive();

        void OnException();
    }
}
