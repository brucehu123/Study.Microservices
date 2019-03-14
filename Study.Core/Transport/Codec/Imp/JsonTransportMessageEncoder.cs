using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Study.Core.Transport.Codec.Imp
{
   public class JsonTransportMessageEncoder:ITransportMessageEncoder
    {
        public byte[] Encode(TransportMessage message)
        {
            var content = JsonConvert.SerializeObject(message);
            return Encoding.UTF8.GetBytes(content);
        }
    }
}
