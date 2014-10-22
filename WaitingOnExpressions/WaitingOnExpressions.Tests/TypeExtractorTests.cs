using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaitingOnExpressions.Logic;
using FluentAssertions;

namespace WaitingOnExpressions.Tests
{
    [TestClass]
    public class TypeExtractorTests
    {
        [TestMethod]
        public void TypeExtractOnConst_ReturnsConst()
        { 
            var visitor = TypeExtractor<int>.Extract(() => 1);
            visitor.ExtractedItems.Should().Contain(1);
        }

        [TestMethod]
        public void TypeExtractOnBinaryOp_ReturnsTwoConsts2()
        {
            var visitor = TypeExtractor<int>.Extract(() => 1 + 2);
            visitor.ExtractedItems.Should().Contain(1);
            visitor.ExtractedItems.Should().Contain(2);
        }

        [TestMethod]
        public void TypeExtractOnUnaryOp_ReturnsConst()
        {
            var visitor = TypeExtractor<int>.Extract(() => -(1));
            visitor.ExtractedItems.Should().Contain(-1);
        }

        [TestMethod]
        public void TypeExtractOnLocal_ReturnsValue()
        {
            var x = 1;
            var visitor = TypeExtractor<int>.Extract(() => x);
            visitor.ExtractedItems.Should().Contain(1);
        }
    }
}
