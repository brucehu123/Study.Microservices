using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Study.Core.Message;
using Study.Core.Transport;

namespace Study.Core.Runtime.Client
{
    public  interface IRpcClient
    {
        TaskCompletionSource<TransportMessage> CallBack { get; }
        Task SendAsync(TransportMessage message);
    }
  
}
