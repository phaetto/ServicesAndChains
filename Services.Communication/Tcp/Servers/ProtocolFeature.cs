namespace Services.Communication.Tcp.Servers
{
    using System;

    [Flags]
    public enum ProtocolFeature
    {
        None,

        KeepConnectionOpen,
    }
}
