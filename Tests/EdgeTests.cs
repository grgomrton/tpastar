using System;
using CommonTools.Geometry;
using FluentAssertions;
using NUnit.Framework;

namespace TPAStar.Tests
{
    [TestFixture]
    public class EdgeTests
    {
        private static double Precision = 0.00001;
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnLeftEdgeEndpoint()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(2.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(0.0, Precision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnRightEdgeEndpoint()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(4.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(0.0, Precision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnEdge()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(3.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(0.0, Precision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceOnYAxisIfPointFallsBetweenEndpointsOnXAxis()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(3.0, 2.5);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(1.5, Precision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromLeftEndpointIfPointFallsLeftFromLeftEndpoint()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(1.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootTwo = 1.41421;

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(squareRootTwo, Precision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromRightEndpointIfPointFallsRightFromRightEndpoint()
        {
            var leftEndpoint = new Vector3(2.0, 1.0);
            var rightEndpoint = new Vector3(4.0, 1.0);
            var pointToCheck = new Vector3(6.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootFive = 2.23607;

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(squareRootFive, Precision);
        }
        
        [Test]
        public void DistanceOfPointFromDiagonalEdgeShouldBeTheLenghtOfARightSegmentBetweenEdgeAndPoint()
        {
            var leftEndpoint = new Vector3(1.0, 1.0);
            var rightEndpoint = new Vector3(3.0, 3.0);
            var pointToCheck = new Vector3(1.0, 3.0);
            var squareRootTwo = 1.41421;
            var edge = new Edge(leftEndpoint, rightEndpoint);

            var distance = edge.DistanceFromPoint(pointToCheck);

            distance.Should().BeApproximately(squareRootTwo, Precision);
        }

        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpoints()
        {
            var leftEndpointOfFirstEdge = new Vector3(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector3(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector3(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector3(4.0, 3.0);
            IEdge firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            IEdge secondEdge = new Edge(leftEndpointOfSecondEdge, rightEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpointsIndependentlyFromEndpointOrder()
        {
            var leftEndpointOfFirstEdge = new Vector3(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector3(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector3(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector3(4.0, 3.0);
            IEdge firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            IEdge secondEdge = new Edge(rightEndpointOfSecondEdge, leftEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void EdgesShouldBeEqualIfTheyLieBetweenSameEndpointsEvenIfTheyAreBoxedIndependentlyFromEndpointOrder()
        {
            var leftEndpointOfFirstEdge = new Vector3(1.0, 3.0);
            var leftEndpointOfSecondEdge = new Vector3(1.0, 3.0);
            var rightEndpointOfFirstEdge = new Vector3(4.0, 3.0);
            var rightEndpointOfSecondEdge = new Vector3(4.0, 3.0);
            object firstEdge = new Edge(leftEndpointOfFirstEdge, rightEndpointOfFirstEdge);
            object secondEdge = new Edge(rightEndpointOfSecondEdge, leftEndpointOfSecondEdge);

            var equalityCheckResult = firstEdge.Equals(secondEdge);

            equalityCheckResult.Should().BeTrue();
        }
    }
}