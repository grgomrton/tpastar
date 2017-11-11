/**
 * Copyright 2017 Márton Gergó
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
        
        [SetUp]
        public void BeforeEachTest()
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

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnRightEdgeEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(4.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceFromPointShouldBeZeroIfPointFallsOnEdge()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(3.0, 1.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(0.0, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceOnYAxisIfPointFallsBetweenEndpointsOnXAxis()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(3.0, 2.5);
            var edge = new Edge(leftEndpoint, rightEndpoint);

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(1.5, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromLeftEndpointIfPointFallsLeftFromLeftEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(1.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootTwo = 1.41421;

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(squareRootTwo, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromHorizontalEdgeShouldBeDistanceFromRightEndpointIfPointFallsRightFromRightEndpoint()
        {
            var leftEndpoint = new Vector(2.0, 1.0);
            var rightEndpoint = new Vector(4.0, 1.0);
            var pointToCheck = new Vector(6.0, 2.0);
            var edge = new Edge(leftEndpoint, rightEndpoint);
            var squareRootFive = 2.23607;

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(squareRootFive, AssertionPrecision);
        }
        
        [Test]
        public void DistanceOfPointFromDiagonalEdgeShouldBeTheLenghtOfARightSegmentBetweenEdgeAndPoint()
        {
            var leftEndpoint = new Vector(1.0, 1.0);
            var rightEndpoint = new Vector(3.0, 3.0);
            var pointToCheck = new Vector(1.0, 3.0);
            var squareRootTwo = 1.41421;
            var edge = new Edge(leftEndpoint, rightEndpoint);

            edge.DistanceFrom(pointToCheck).Should().BeApproximately(squareRootTwo, AssertionPrecision);
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

            firstEdge.Equals(secondEdge).Should().BeTrue();
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

            firstEdge.Equals(secondEdge).Should().BeTrue();
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

            firstEdge.Equals(secondEdge).Should().BeTrue();
        }

        [Test]
        public void PointsThatAreCloserToTheEdgeThanVectorToleranceShouldFallOnEdge()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var edge = new Edge(a, b);
            var pointToCheck = new Vector(0.5, 0.05);

            edge.PointLiesOnEdge(pointToCheck).Should().BeTrue();
        }
        
        [Test]
        public void PointsThatAreExactlyInVectorToleranceDistanceShouldNotFallOnEdge()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var edge = new Edge(a, b);
            var pointToCheck = new Vector(0.5, 0.1);

            edge.PointLiesOnEdge(pointToCheck).Should().BeFalse();
        }

        [Test]
        public void EdgesBetweenTrianglesWithIdHigherThanIntegerRepresentationBoundaryShouldHaveValidHashes()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(1.0, 0.0);
            var t1 = new Triangle(a, b, c, Int32.MaxValue);
            var d = new Vector(1.0, 1.0);
            var t2 = new Triangle(b, c, d, 10);
            t1.SetNeighbours(new[]{ t2 });

            var sharedEdge = t1.GetCommonEdgeWith(t2);
            var hashCode = sharedEdge.GetHashCode();

            hashCode.Should().BeLessThan(0);
        }

        [Test]
        public void EqualsShouldWorkWithNullParameter()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var edge = new Edge(a, b);

            Action gettingEqualsWithNull = () => edge.Equals(null);
            
            gettingEqualsWithNull.ShouldNotThrow();
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