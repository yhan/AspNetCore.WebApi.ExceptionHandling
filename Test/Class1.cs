using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Should_when_Condition()
        {
            var counter = 0;
            try
            {
                throw new Exception("dqsfqsdf");
            }
            finally
            {
                counter++;
            }

            TestContext.WriteLine($"Counter = {counter}");
        }
    }
}
