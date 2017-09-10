using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.Geometry;

namespace TPAStar.Tests
{
    [TestFixture]
    public class GeometryUtilTests
    {
        [Test]
        public void parallelVectorsShouldNotBeDetectedClockwise()
        {
            var p1 = new Vector3(1.0, 1.0);
            var p2 = new Vector3(3.0, 1.0);
            var pm = new Vector3(2.0, 1.0);
            var pmToP1 = p1 - pm;
            var pmToP2 = p2 - pm;

            bool result = OrientationUtil.ClockWise(pmToP1, pmToP2);

            result.Should().BeFalse();
        }
        
        [Test]
        public void parallelVectorsShouldNotBeDeterminedCounterClockwise()
        {
            var pm = new Vector3(2.0, 1.0);
            var p1 = new Vector3(2.0, 2.0);
            var p2 = new Vector3(3.0, 1.0);
            var pmToP1 = p1 - pm;
            var pmToP2 = p2 - pm;

            bool result = OrientationUtil.CounterClockWise(pmToP1, pmToP2);

            result.Should().BeFalse();
        }
        
        [Test]
        public void vectorsInCounterClockwiseOrientationShouldBeDeterminedAsCounterClockwise()
        {
            var p2 = new Vector3(3.0, 1.0);
            var p1 = new Vector3(2.0, 2.0);
            var pm = new Vector3(2.0, 1.0);
            var pmToP1 = p1 - pm;
            var pmToP2 = p2 - pm;
            
            bool result = OrientationUtil.CounterClockWise(pmToP2, pmToP1);

            result.Should().BeTrue();
        }
        
        [Test]
        public void vectorsInClockwiseOrientationShouldBeDeterminedAsClockwise()
        {
            var p2 = new Vector3(3.0, 1.0);
            var p1 = new Vector3(2.0, 2.0);
            var pm = new Vector3(2.0, 1.0);
            var pmToP1 = p1 - pm;
            var pmToP2 = p2 - pm;
            
            bool result = OrientationUtil.ClockWise(pmToP1, pmToP2);

            result.Should().BeTrue();
        }
    }
}