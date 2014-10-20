using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericExtensions.Tests
{
    [TestClass]
    public class TestIfIs
    {
        [TestMethod]
        public void IfIs_Match_ActionNotNull_ActionIsCalled()
        {
            var ok = false;
            Action<B> action = b =>
            {
                ok = true;
            };

            A a = new B();
            a.If().Is<B>(action);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void IfIs_Match_ActionNull_ActionIsNotCalled()
        {
            Action<B> action = null;

            A a = new B();
            a.If().Is<B>(action);
        }

        [TestMethod]
        public void IfIs_NoMatch_ActionNotNull_ActionIsNotCalled()
        {
            var ok = true;
            Action<B> action = b =>
            {
                ok = false;
            };

            A a = new C();
            a.If().Is<B>(action);

            Assert.IsTrue(ok);
        }
    }
}
