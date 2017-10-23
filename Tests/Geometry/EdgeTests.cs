using System;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class EdgeTests
    {
        private static double AssertionPrecision = 0.00001;
        
        [OneTimeSetUp]
        public void SetupVectorLibrary()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnLeftEdgeEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(2.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnRightEdgeEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(4.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnEdge()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(3.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceOnYAxisIfPointFallsBetweenEndpointsOnXAxis()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(3.0, 2.5);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(1.5, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromLeftEndpointIfPointFallsLeftFromLeftEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(1.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootTwo = 1.41421;

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(squareRootTwo, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromRightEndpointIfPointFallsRightFromRightEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(6.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootFive = 2.23607;

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(squareRootFive, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromDiagonalEdgeShouldBeTheLenghtOfARightSegmentBetweenEdgeAndPoint()
        {
            var leftEndpoint = new Vector(1.0, 1.0);
            var rightEndpoint = new Vector(3.0, 3.0);
            var pointToCheck = new Vector(1.0, 3.0);
            var squareRootTwo = 1.41421;
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFrom(pointToCheck);

            distance.Should().BeApproximately(squareRootTwo, AssertionPrecision);
        }

        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpoints()
        {
            var leftEndpointOfFirstEdge = new Vector(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector(4.0, 3.0);
            IEdge firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            IEdge secondEdge = new Edge(leftEndpointOfSecondEdge, rightEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpointsIndependentlyFromEndpointOrder()
        {
            var leftEndpointOfFirstEdge = new Vector(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector(4.0, 3.0);
            IEdge firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            IEdge secondEdge = new Edge(rightEndpointOfSecondEdge, leftEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpointsEvenIfTheyAreBoxedIndependentlyFromEndpointOrder()
        {
            var leftEndpointOfFirstEdge = new Vector(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector(4.0, 3.0);
            object firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            object secondEdge = new Edge(rightEndpointOfSecondEdge, leftEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void DistortedEdgeShouldNotBeCreated() 
        {
            var a = new Vector(0.0, 1.0);
            var b = new Vector(0.0, 1.0);

            Action edgeInstantiation = () => new Edge(a, b);

            edgeInstantiation.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain("equal");
        }
    }
}