namespace Chains.UnitTests
{
    using System.Collections.Generic;
    using Chains.Play;
    using Chains.UnitTests.Classes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SerializationTest
    {
        [TestMethod]
        public void Chain_WhenObjectIsSerializedToBase64AndDeserialized_ThenTheObjectIsTheSame()
        {
            var data = new ReproducibleTestData
            {
                ChangeToValue = "value",
                DomainName = "domain"
            };

            var serializableData = data.SerializeToBase64String();

            var deserializedData =
                DeserializableSpecification<ReproducibleTestData>.DeserializeFromBase64String(serializableData);

            Assert.AreEqual("value", deserializedData.ChangeToValue);
            Assert.AreEqual("domain", deserializedData.DomainName);
        }

        [TestMethod]
        public void Chain_WhenObjectIsSerializedToJsonAndDeserialized_ThenTheObjectIsTheSame()
        {
            var data = new ReproducibleTestData
                       {
                           ChangeToValue = "value",
                           DomainName = "domain"
                       };

            var serializableData = data.SerializeToJson();

            var deserializedData =
                DeserializableSpecification<ReproducibleTestData>.DeserializeFromJson(serializableData);

            Assert.AreEqual("value", deserializedData.ChangeToValue);
            Assert.AreEqual("domain", deserializedData.DomainName);
        }

        [TestMethod]
        public void Chain_WhenManyObjectsAreSerializedToJsonAndDeserialized_ThenObjectsAreTheSame()
        {
            var data = new[]
                       {
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 1",
                               DomainName = "domain 1",
                               StringArray = new[]
                                             {
                                                 "stuff 1",
                                                 "stuff 2",
                                             }
                           },
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 2",
                               DomainName = "domain 2"
                           },
                           new ReproducibleTestData
                           {
                               ChangeToValue = "value 3",
                               DomainName = "domain 3"
                           }
                       };

            var serializableData = SerializableSpecification.SerializeManyToJson(data);

            var deserializedData =
                DeserializableSpecification<ReproducibleTestData>.DeserializeManyFromJson(serializableData);

            Assert.AreEqual("value 1", deserializedData[0].ChangeToValue);
            Assert.AreEqual("domain 1", deserializedData[0].DomainName);
            Assert.AreEqual("value 2", deserializedData[1].ChangeToValue);
            Assert.AreEqual("domain 2", deserializedData[1].DomainName);
            Assert.AreEqual("value 3", deserializedData[2].ChangeToValue);
            Assert.AreEqual("domain 3", deserializedData[2].DomainName);
        }

        [TestMethod]
        public void DeserializableSpecification_WhenDeserializingComplexExceptionData_ThenItSucceeds()
        {
            var jsonData =
                "{\"DataType\":\"System.Collections.Generic.KeyNotFoundException\",\"Data\":{\"ClassName\":\"System.Collections.Generic.KeyNotFoundException\",\"Message\":\"The given key was not present in the dictionary.\",\"InnerException\":null,\"HelpURL\":null,\"StackTraceString\":\"at System.Collections.Generic.Dictionary`2 < string, string>.get_Item(string) < 0x001b8 >\nat Services.Communication.DataStructures.NameValue.GetKeyValue.Act(Services.Communication.DataStructures.NameValue.HashContext) < 0x0004f >\nat Chains.Chain`1 < Services.Communication.DataStructures.NameValue.HashContext >.InvokeAct<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.IChainableAction`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x0005f >\nat Chains.Chain`1 < Services.Communication.DataStructures.NameValue.HashContext >.Do<Services.Communication.DataStructures.NameValue.KeyValueData>(Chains.IChainableAction`2 < Services.Communication.DataStructures.NameValue.HashContext, Services.Communication.DataStructures.NameValue.KeyValueData >) < 0x00063 >\nat(wrapper dynamic - method) object.CallSite.Target(System.Runtime.CompilerServices.Closure, System.Runtime.CompilerServices.CallSite, object, object) < 0x0010f >\nat Chains.Play.ExecuteActionAndGetResult.Act(Chains.Play.ExecutionChain) < 0x005a3 >\nat Chains.Chain`1 < Chains.Play.ExecutionChain >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.IChainableAction`2 < Chains.Play.ExecutionChain, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Chain`1 < Chains.Play.ExecutionChain >.Do<Chains.Play.ExecutionResultContext>(Chains.IChainableAction`2 < Chains.Play.ExecutionChain, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Chains.Play.ExecuteActionFromSpecification.Act(Chains.Play.ExecutionChain) < 0x0005f >\nat Chains.Chain`1 < Chains.Play.ExecutionChain >.InvokeAct<Chains.Play.ExecutionResultContext>(Chains.IChainableAction`2 < Chains.Play.ExecutionChain, Chains.Play.ExecutionResultContext >) < 0x0005f >\nat Chains.Chain`1 < Chains.Play.ExecutionChain >.Do<Chains.Play.ExecutionResultContext>(Chains.IChainableAction`2 < Chains.Play.ExecutionChain, Chains.Play.ExecutionResultContext >) < 0x00063 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataOnExecutionChain(Chains.Play.ExecutionChain, Chains.Play.ExecutableActionSpecification[]) < 0x00083 >\nat Services.Communication.Protocol.ProtocolServerLogic.ApplyDataAndReturn(Chains.Play.ExecutionChain, Chains.Play.ExecutableActionSpecification[], bool) < 0x000e3 >\n\",\"RemoteStackTraceString\":null,\"RemoteStackIndex\":0,\"HResult\":-2146233087,\"Source\":\"mscorlib\",\"ExceptionMethod\":null,\"Data\":null},\"DataStructureVersionNumber\":1}";

            var deserializedData =
                DeserializableSpecification<ExecutableActionSpecification>.DeserializeFromJson(jsonData);

            Assert.IsNotNull(deserializedData);
            Assert.IsInstanceOfType(deserializedData.Data, typeof(KeyNotFoundException));
        }
    }
}
