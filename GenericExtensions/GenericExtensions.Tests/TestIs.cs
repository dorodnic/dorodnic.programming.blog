using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericExtensions.Tests
{
    [TestClass]
    public class TestIs
    {
        [TestMethod]
        public void IsNot_ClassesUnrelated_ReturnsTrue()
        {
            A b = new B();
            Assert.IsTrue(b.Is().Not<C>());
        }

        [TestMethod]
        public void IsNot_ClassesMatch_ReturnsFalse()
        {
            A b = new B();
            Assert.IsFalse(b.Is().Not<B>());
        }

        [TestMethod]
        public void IsNot_SameClass_ReturnsFalse()
        {
            A b = new B();
            Assert.IsFalse(b.Is().Not<A>());
        }

        [TestMethod]
        public void IsNull_OnNull_ReturnsTrue()
        {
            B b = null;
            Assert.IsTrue(b.Is().Null());
        }

        [TestMethod]
        public void IsNull_OnNotNull_ReturnsFalse()
        {
            B b = new B();
            Assert.IsFalse(b.Is().Null());
        }

        [TestMethod]
        public void IsNotNull_OnNull_ReturnsFalse()
        {
            B b = null;
            Assert.IsFalse(b.Is().NotNull());
        }

        [TestMethod]
        public void IsNotNull_OnNotNull_ReturnsTrue()
        {
            B b = new B();
            Assert.IsTrue(b.Is().NotNull());
        }
    }
}
