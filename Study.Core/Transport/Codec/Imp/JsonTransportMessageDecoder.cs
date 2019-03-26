using System;
using System.Text;
using Newtonsoft.Json;
using Study.Core.Message;

namespace Study.Core.Transport.Codec.Imp
{
  public  class JsonTransportMessageDecoder:ITransportMessageDecoder
    {
        public TransportMessage Decoder(byte[] data)
        {
            try
            {
                var content = Encoding.UTF8.GetString(data);
                var message = JsonConvert.DeserializeObject<TransportMessage>(content);
                if (message.IsInvokeMessage())
                {
                    message.Content = JsonConvert.DeserializeObject<RemoteInvokeMessage>(message.Content.ToString());
                }
                if (message.IsInvokeResultMessage())
                {
                    message.Content = JsonConvert.DeserializeObject<RemoteInvokeResultMessage>(message.Content.ToString());
                }
                return message;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public TransportMessage Decoder(string data)
        {
            var message = JsonConvert.DeserializeObject<TransportMessage>(data);
            if (message.IsInvokeMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeMessage>(message.Content.ToString());
            }
            if (message.IsInvokeResultMessage())
            {
                message.Content = JsonConvert.DeserializeObject<RemoteInvokeResultMessage>(message.Content.ToString());
            }
            return message;
        }
    }
}
