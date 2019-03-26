namespace Study.Core.Transport.Codec
{
    public  interface  ITransportMessageCodecFactory
    {
        ITransportMessageDecoder CreateDecoder();
        ITransportMessageEncoder CreateEncoder();
    }
}
