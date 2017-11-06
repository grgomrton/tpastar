using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class TPAPathTests
    {
        [OneTimeSetUp]
        public void BeforeTheseTestCases()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }
        
        [Test]
        public void afterSteppingIntoATriangleExplorableTrianglesShouldNotContainThePreviousOne()
        {
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 1);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2);
            var startPoint = new Vector(7.5, 10.0);
            var goalPoints = new [] { new Vector(100.0, 100.0) };
            var path = new TPAPath(startPoint, t2);

            var pathsAfterSteppingIntoT2 = path.ExploreNeighbourTriangle(goalPoints);
            var theOnlyPathThatShouldBeCreated = pathsAfterSteppingIntoT2.First();
            var pathsAfterSteppingIntoT3 = theOnlyPathThatShouldBeCreated.ExploreNeighbourTriangles(goalPoints);

            pathsAfterSteppingIntoT2.Count().Should().Be(1);
            theOnlyPathThatShouldBeCreated.CurrentTriangle.ShouldBeEquivalentTo(t3);
            pathsAfterSteppingIntoT3.Count().Should().Be(0);
        }

        [Test]
        public void initialPathShouldContainEveryNeighbourOfStartTriangle()
        {
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 1);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c, 2);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            var s = new Vector(9.0, 11.5);
            var goalPoints = new [] { new Vector(100.0, 100.0) };
            var initialPath = new TPAPath(s, t3);

            var pathsAfterSteppingIntoT3 = initialPath.ExploreNeighbourTriangle(goalPoints);

            pathsAfterSteppingIntoT3.Count().Should().Be(2);
            pathsAfterSteppingIntoT3.Should().Contain(path => path.CurrentTriangle.Equals(t2));
            pathsAfterSteppingIntoT3.Should().Contain(path => path.CurrentTriangle.Equals(t4));
        }
        
        [Test]
        public void afterSteppingIntoATriangleExplorableTrianglesShouldContainTheOneWeAreNotComingFrom()
        {
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 1);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c, 2);
            var t5a = new Vector(15.0, 12.5);
            var t5b = new Vector(12.5, 15.0);
            var t5c = new Vector(10.0, 12.5);
            var t5 = new Triangle(t5a, t5b, t5c, 3);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            t4.SetNeighbours(t3, t5);
            t5.SetNeighbours(t4);
            var s = new Vector(9.0, 11.5);
            var goalPoints = new [] { new Vector(100.0, 100.0) };
            var initialPath = new TPAPath(s, t2);
            
            var pathsAfterSteppingIntoT2 = initialPath.ExploreNeighbourTriangle(goalPoints);
            var theOnlyPathThatShouldBeCreated = pathsAfterSteppingIntoT2.First();
            var pathsAfterSteppingIntoT3 = theOnlyPathThatShouldBeCreated.ExploreNeighbourTriangles(goalPoints);

            pathsAfterSteppingIntoT2.Count().Should().Be(1);
            pathsAfterSteppingIntoT3.Should().Contain(path => path.CurrentTriangle.Equals(t4));
            pathsAfterSteppingIntoT3.Should().NotContain(path => path.CurrentTriangle.Equals(t2));
        }
        
    }
}