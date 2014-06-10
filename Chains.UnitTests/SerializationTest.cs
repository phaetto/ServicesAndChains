namespace Chains.UnitTests
{
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
                               stringArray = new[]
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
    }
}
