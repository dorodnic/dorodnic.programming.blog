using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericExtensions.Tests
{
    [TestClass]
    public class TestIfElse
    {
        [TestMethod]
        public void IfElse_NoMatch_ActionNotNull_ElseIsCalled()
        {
            var ok = false;
            Action action = () =>
            {
                ok = true;
            };

            A a = new B();
            a.If().Is<C>(c => Assert.Fail()).Else(action);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void IfElse_NoMatch_ActionNull_ElseIsNotCalled()
        {
            Action action = null;

            A a = new B();
            a.If().Is<C>(c => Assert.Fail()).Else(action);
        }

        [TestMethod]
        public void IfElse_Match_ActionNotNull_ElseIsNotCalled()
        {
            var ok = false;
            Action action = () =>
            {
                ok = false;
            };

            A a = new B();
            a.If().Is<B>(b => ok = true).Else(action);

            Assert.IsTrue(ok);
        }
    }
}
