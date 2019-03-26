namespace Study.Core.Transport.Codec.Imp
{
    public class DefaultTransportMessageCodecFactory : ITransportMessageCodecFactory
    {
        private readonly ITransportMessageEncoder _encoder;
        private readonly ITransportMessageDecoder _decoder;

        public DefaultTransportMessageCodecFactory()
        {
            _encoder = new JsonTransportMessageEncoder();
            _decoder = new JsonTransportMessageDecoder();
        }

        public ITransportMessageDecoder CreateDecoder()
        {
            return _decoder;
        }

        public ITransportMessageEncoder CreateEncoder()
        {
            return _encoder;
        }
    }
}
