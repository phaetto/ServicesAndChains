namespace Chains.Play.Security.Provider
{
    using System;

    [Serializable]
    public sealed class ProviderAuthenticationGateData : SerializableSpecification
    {
        public string LoginUrl;

        public LoginUrlMethod LoginUrlMethod = LoginUrlMethod.NotSupported;

        public string[] LoginUrlDataToSend;

        public string LoginTypeUrlZip;

        public string LoginType;

        public string LoginDllPath;

        public object[] LoginTypeArguments;

        public override int DataStructureVersionNumber
        {
            get
            {
                return 1;
            }
        }
    }
}
