using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chains.UnitTests
{
    using Chains.Play;

    [TestClass]
    public class CreateObjectTests
    {
        [TestMethod]
        public void CreateObjectWithParameters_WhenInjectionIsUsed_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = ExecutionChain.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).FullName,
                new object[]
                {
                    1, "one"
                },
                new object[]
                {
                    new InjectedClass()
                }) as WantedClass;

            Assert.IsNotNull(createdObject);
            Assert.IsInstanceOfType(createdObject, typeof(WantedClass));
            Assert.IsInstanceOfType(createdObject.InjectedClass, typeof(InjectedClass));
        }

        [TestMethod]
        public void CreateObjectWithParameters_WhenInjectionIsUsedWithWrongOrder_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = ExecutionChain.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).FullName,
                new object[]
                {
                    1, "one"
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.IsNotNull(createdObject);
            Assert.IsInstanceOfType(createdObject, typeof(WantedClass));
            Assert.IsInstanceOfType(createdObject.InjectedClass, typeof(InjectedClass));
            Assert.IsInstanceOfType(createdObject.InjectedClass2, typeof(InjectedClass2));
        }

        [TestMethod]
        public void CreateObjectWithParameters_WhenArrayObjectIsUsed_ThenItemIsBeenSetCorrectly()
        {
            var createdObject = ExecutionChain.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).FullName,
                new object[]
                {
                    1, new[] { "one" }
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.IsNotNull(createdObject);
            Assert.IsInstanceOfType(createdObject, typeof(WantedClass));
            Assert.IsInstanceOfType(createdObject.InjectedClass, typeof(InjectedClass));
        }

        [TestMethod]
        public void CreateObjectWithParameters_WhenDefaultOptionIsUsed_ThenParameterIsBeenSetCorrectly()
        {
            var createdObject = ExecutionChain.CreateObjectWithParametersAndInjection(
                typeof(WantedClass).FullName,
                new object[]
                {
                    1
                },
                new object[]
                {
                    new InjectedClass2(),
                    new InjectedClass()
                }) as WantedClass;

            Assert.IsNotNull(createdObject);
            Assert.AreEqual("default", createdObject.Astring);
            Assert.IsInstanceOfType(createdObject, typeof(WantedClass));
            Assert.IsNull(createdObject.InjectedClass);
        }

        public class InjectedClass
        {
        }

        public class InjectedClass2
        {
        }

        public class WantedClass
        {
            public readonly bool Abool;
            public readonly string Astring;
            public readonly string[] Astrings;
            public readonly InjectedClass InjectedClass;
            public readonly InjectedClass2 InjectedClass2;

            public WantedClass(bool abool, string[] astrings, InjectedClass injectedClass)
            {
                this.Abool = abool;
                this.Astrings = astrings;
                InjectedClass = injectedClass;
            }

            public WantedClass(bool abool, string astring = "default")
            {
                this.Abool = abool;
                this.Astring = astring;
            }

            public WantedClass(bool abool, string astring, InjectedClass injectedClass)
            {
                this.Abool = abool;
                this.Astring = astring;
                InjectedClass = injectedClass;
            }

            public WantedClass(bool abool, string astring, InjectedClass injectedClass = null, InjectedClass2 injectedClass2 = null)
            {
                this.Abool = abool;
                this.Astring = astring;
                InjectedClass = injectedClass;
                this.InjectedClass2 = injectedClass2;
            }
        }
    }
}
