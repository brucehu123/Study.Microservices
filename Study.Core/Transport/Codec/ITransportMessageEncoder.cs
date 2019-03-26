namespace Study.Core.Transport.Codec
{
    public interface ITransportMessageEncoder
    {
        byte[] Encode(TransportMessage message);
    }
}
