namespace Study.Core.Transport.Codec
{
    public  interface ITransportMessageDecoder
    {
        TransportMessage Decoder(byte[] data);
        TransportMessage Decoder(string data);
    }
}
