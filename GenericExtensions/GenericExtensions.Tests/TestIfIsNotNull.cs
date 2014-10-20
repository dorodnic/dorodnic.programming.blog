using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericExtensions.Tests
{
    [TestClass]
    public class TestIfIsNotNull
    {
        [TestMethod]
        public void IfNotNull_ValueIsNull_ActionNotNull_ActionIsNotCalled()
        {
            var ok = true;
            Action<A> action = x =>
            {
                ok = false;
            };

            A a = null;
            a.If().IsNotNull(action);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void IfNotNull_ValueIsNull_ActionNull_ActionIsNotCalled()
        {
            Action<A> action = null;

            A a = null;
            a.If().IsNotNull(action);
        }

        [TestMethod]
        public void IfNotNull_ValueIsNotNull_ActionNotNull_ActionIsCalled()
        {
            var ok = false;
            Action<A> action = x =>
            {
                ok = true;
            };

            var a = new A();
            a.If().IsNotNull(action);

            Assert.IsTrue(ok);
        }
    }
}
