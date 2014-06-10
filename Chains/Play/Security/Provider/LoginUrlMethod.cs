namespace Chains.Play.Security.Provider
{
    using System;

    [Flags]
    public enum LoginUrlMethod
    {
        NotSupported = 0x0,
        BrowseUrl = 0x1,
        GetRequest = 0x2,
        PostRequest = 0x4,
        DataOnHeaderGetRequest = 0x8,
        DataOnHeaderPostRequest = 0x10,
    }
}
