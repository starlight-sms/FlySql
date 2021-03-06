﻿using Xunit;

namespace sdmap.unittest.VisitorTest
{
    public class InnerTravelVisitorTest : VisitorTestBase
    {
        [Fact]
        public void CannotTravelInner()
        {
            var root = GetParseTree("namespace ns{sql sql{}}");
            var visitor = new Mocks.InnerNoTravelVisitor();
            visitor.Visit(root);

            Assert.False(visitor.VisitedNamedSql);
        }

        [Fact]
        public void CanTravelInner()
        {
            var root = GetParseTree("namespace ns{sql sql{}}");
            var visitor = new Mocks.InnerTravelVisitor();
            visitor.Visit(root);

            Assert.True(visitor.VisitedNamedSql);
        }
    }
}
