using System;
using Xunit;

namespace DemoApp.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void PassingTest()
        {
            Assert.True(1==1);
        }

        [Fact(Skip="Intentionally fails")]
        public void FailingTest()
        {
            Assert.False(1==1);
        }
    }
}
