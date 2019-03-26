namespace Study.Core.Runtime.Server
{
    public  interface  IServiceEntryLocator
    {
        ServerEntry Locate(string serviceId);
    }
}
