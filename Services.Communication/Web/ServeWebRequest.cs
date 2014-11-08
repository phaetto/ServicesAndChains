namespace Services.Communication.Web
{
    using System;
    using System.IO;
    using Chains;
    using Chains.Play;
    using Chains.Play.Web;
    using Services.Communication.Protocol;

    public sealed class ServeWebRequest : IChainableAction<HttpContextInfo, HttpResultContext>
    {
        private readonly object contextObject;
        private readonly string contextTypeName;
        private readonly Func<ExecutableActionSpecification[], bool> onBeforeExecute;
        private readonly Action<dynamic> onAfterExecute;

        public ServeWebRequest(
            object contextObject,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null)
        {
            this.contextObject = contextObject;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
        }

        public ServeWebRequest(
            string contextTypeName,
            Func<ExecutableActionSpecification[], bool> onBeforeExecute = null,
            Action<dynamic> onAfterExecute = null)
        {
            this.contextTypeName = contextTypeName;
            this.onBeforeExecute = onBeforeExecute;
            this.onAfterExecute = onAfterExecute;
        }

        public HttpResultContext Act(HttpContextInfo context)
        {
            if (context.HttpMethod.ToLowerInvariant() != "post")
            {
                throw new InvalidOperationException("Only post is supported from this chain server.");
            }

            var postJson = string.Empty;
            using (var streamReader = new StreamReader(context.InputStream))
            {
                postJson = streamReader.ReadToEnd();
            }

            if (string.IsNullOrWhiteSpace(postJson))
            {
                throw new InvalidOperationException("Post data are empty, this is not allowed.");
            }

            var protocolServerLogic = contextObject != null
                ? new ProtocolServerLogic(contextObject, onBeforeExecute, onAfterExecute)
                : new ProtocolServerLogic(contextTypeName, onBeforeExecute, onAfterExecute);

            return new HttpResultContext(protocolServerLogic.ReadFromStreamAndPlay(postJson), "text/json");
        }
    }
}
