using CommonTools.Geometry;
using FluentAssertions;
using NUnit.Framework;

namespace TPAStar.Tests
{
    [TestFixture]
    public class TriangleTests
    {
        [Test]
        public void TwoTriangleLyingOnSamePointsShouldBeEqualEvenIfDefinedWithDifferentVectorInstances()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2a, t2b, t2c); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
        
    }
}