using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using PathFinder.TPAStar;
using TriangulatedPolygonAStar.Geometry;

namespace TPAStar.Tests
{
    [TestFixture]
    public class TPAPathTests
    {
        [Test]
        public void afterSteppingIntoATriangleExplorableTrianglesShouldNotContainThePreviousOne()
        {
            var t2a = new Vector3(10.0, 7.5);
            var t2b = new Vector3(10.0, 12.5);
            var t2c = new Vector3(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c);
            var t3a = new Vector3(5.0, 10.0);
            var t3b = new Vector3(10.0, 12.5);
            var t3c = new Vector3(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2);
            var startPoint = new Vector3();
            var path = new TPAPath(startPoint);
            
            path.StepTo(t2, new IVector[]{ new Vector3() });
            path.StepTo(t3, new IVector[]{ new Vector3() });
            var explorableNeighbours = path.ExplorableTriangles;

            explorableNeighbours.Contains(t2).Should().BeFalse();
        }

        [Test]
        public void initialPathShouldContainEveryNeighbourOfStartTriangle()
        {
            var t2a = new Vector3(10.0, 7.5);
            var t2b = new Vector3(10.0, 12.5);
            var t2c = new Vector3(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c);
            var t3a = new Vector3(5.0, 10.0);
            var t3b = new Vector3(10.0, 12.5);
            var t3c = new Vector3(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c);
            var t4a = new Vector3(10.0, 12.5);
            var t4b = new Vector3(12.5, 15.0);
            var t4c = new Vector3(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            var s = new Vector3(9.0, 11.5);
            var path = new TPAPath(s);

            path.StepTo(t3, new IVector[] {});
            
            path.ExplorableTriangles.Contains(t2).Should().BeTrue();
            path.ExplorableTriangles.Contains(t4).Should().BeTrue();
        }
        
        [Test]
        public void afterSteppingIntoATriangleExplorableTrianglesShouldContainTheOneWeAreNotComingFrom()
        {
            var t2a = new Vector3(10.0, 7.5);
            var t2b = new Vector3(10.0, 12.5);
            var t2c = new Vector3(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c);
            var t3a = new Vector3(5.0, 10.0);
            var t3b = new Vector3(10.0, 12.5);
            var t3c = new Vector3(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c);
            var t4a = new Vector3(10.0, 12.5);
            var t4b = new Vector3(12.5, 15.0);
            var t4c = new Vector3(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c);
            var t5a = new Vector3(15.0, 12.5);
            var t5b = new Vector3(12.5, 15.0);
            var t5c = new Vector3(10.0, 12.5);
            var t5 = new Triangle(t5a, t5b, t5c);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            t4.SetNeighbours(t3, t5);
            t5.SetNeighbours(t4);
            var s = new Vector3(9.0, 11.5);
            var path = new TPAPath(s);
            
            path.StepTo(t2, new IVector[] { new Vector3() });
            path.StepTo(t3, new IVector[] { new Vector3() });
            var explorableNeighbours = path.ExplorableTriangles;

            explorableNeighbours.Contains(t4).Should().BeTrue();
            explorableNeighbours.Contains(t2).Should().BeFalse();
        }
        
    }
}