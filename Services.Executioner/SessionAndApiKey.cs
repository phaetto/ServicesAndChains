namespace Services.Executioner
{
    using Chains.Play;
    using Chains.Play.Security;

    public class SessionAndApiKey : SerializableSpecification, IAuthorizableAction, IApplicationAuthorizableAction
    {
        public string Session { get; set; }

        public string ApiKey { get; set; }

        public override int DataStructureVersionNumber => 1;
    }
}
