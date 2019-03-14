using System;
using System.Collections.Generic;
using System.Text;

namespace Study.Core.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}
