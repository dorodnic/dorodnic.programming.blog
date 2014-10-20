using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GenericExtensions.Tests
{
    [TestClass]
    public class TestIfIsNull
    {
        [TestMethod]
        public void IfNull_ValueIsNull_ActionNotNull_ActionIsCalled()
        {
            var ok = false;
            Action action = () =>
            {
                ok = true;
            };

            A a = null;
            a.If().IsNull(action);

            Assert.IsTrue(ok);
        }

        [TestMethod]
        public void IfNull_ValueIsNull_ActionNull_ActionIsNotCalled()
        {
            Action action = null;

            A a = null;
            a.If().IsNull(action);
        }

        [TestMethod]
        public void IfNull_ValueIsNotNull_ActionNotNull_ActionIsNotCalled()
        {
            var ok = true;
            Action action = () =>
            {
                ok = false;
            };

            var a = new A();
            a.If().IsNull(action);

            Assert.IsTrue(ok);
        }
    }
}
