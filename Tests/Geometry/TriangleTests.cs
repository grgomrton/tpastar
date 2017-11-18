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
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class TriangleTests
    {
        [SetUp]
        public void BeforeEachTest()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPoints()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfThreePointsCloserThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, -0.001);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, 0.02);
            var f = new Vector(0.0, 0.01);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfTwoIdenticalAndOnePointCloserThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.01);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfOneIdenticalAndTwoPointsCloserThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.02);
            var f = new Vector(0.0, 0.01);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldNotBeEqualIfAnyOfTheirPointsAreInHigherDistanceThanVectorTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.1);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c, 0);
            ITriangle t2 = new Triangle(t2a, t2b, t2c, 1);

            var t1ToT2EqualityCheckResult = t1.Equals(t2);
            
            t1ToT2EqualityCheckResult.Should().BeFalse();
        }
        
        [Test]
        public void SettingNeighboursShouldThrowExceptionIfTrianglesHaveNoCommonEdge()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.1);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2a, t2b, t2c, 1);

            Action settingNeighbours = () => t1.SetNeighbours(t2);

            settingNeighbours.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain("not adjacent");
        }

        [Test]
        public void GettingCommonEdgeShouldThrowExceptionIfOtherIsNotAmoungNeighbours()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2a, t2b, t2c, 1);

            Action gettingCommonEdge = () => t1.GetCommonEdgeWith(t2);

            gettingCommonEdge.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain("neighbour");
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSame()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2a, t2b, t2c, 1);
            t1.SetNeighbours(t2);
            
            var commonEdge = t1.GetCommonEdgeWith(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2c, t2a, t2b, 1);
            t1.SetNeighbours(t2);

            var commonEdge = t1.GetCommonEdgeWith(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2c, t2a, t2b, 1);
            t1.SetNeighbours(t2);

            var commonEdge = t1.GetCommonEdgeWith(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrderAndShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2c, t2b, t2a, 1);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            t1.SetNeighbours(t2);
            
            var commonEdge = t1.GetCommonEdgeWith(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }

        [Test]
        public void TriangleShouldStoreNeighboursAfterSettingThem()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            Triangle t1 = new Triangle(t1a, t1b, t1c, 0);
            Triangle t2 = new Triangle(t2a, t2c, t2b, 1);
            
            t1.SetNeighbours(t2);
            var neighbours = t1.Neighbours;

            neighbours.Count().Should().Be(1);
            neighbours.Should().Contain(t2);
        }

        [Test]
        public void TriangleShouldBeAbleToStoreMultipleNeighbours()
        {
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 0);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c, 0);
            
            t3.SetNeighbours(t2, t4);
            var neighbours = t3.Neighbours;

            neighbours.Count().Should().Be(2);
            neighbours.Should().Contain(t2);
            neighbours.Should().Contain(t4);
        }

        [Test]
        public void TriangleNeighboursShouldBeEmptySetByDefault()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c, 0);
            var neighbours = t1.Neighbours;

            neighbours.Should().NotBeNull();
            neighbours.Count().Should().Be(0);
        }

        [Test]
        public void PointThatFallsBehindABEdgeButIsCloserThanVectorDistanceToleranceShouldBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(-0.005, 0.5);

            t.ContainsPoint(p).Should().BeTrue();
        }
        
        [Test]
        public void PointThatFallsBehindABEdgeInDistanceThatEqualsVectorDistanceToleranceShouldNotBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(-0.01, 0.5);

            t.ContainsPoint(p).Should().BeFalse();
        }
        
        [Test]
        public void PointThatFallsBehindACEdgeButIsCloserThanVectorDistanceToleranceShouldBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.0, -0.005);

            t.ContainsPoint(p).Should().BeTrue();
        }
        
        [Test]
        public void PointThatFallsBehindACEdgeInDistanceThatEqualsVectorDistanceToleranceShouldNotBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.0, -0.01);

            t.ContainsPoint(p).Should().BeFalse();
        }
       
        [Test]
        public void PointThatFallsExactlyOnBCEdgeShouldBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.0, 0.5);

            t.ContainsPoint(p).Should().BeTrue();
        }
        
        [Test]
        public void PointThatFallsBehindBCEdgeButIsCloserThanVectorDistanceToleranceShouldBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.0, 0.501);

            t.ContainsPoint(p).Should().BeTrue();
        }
        
        [Test]
        public void PointThatFallsBehindBCEdgeInADistanceEqualsVectorDistanceToleranceShouldNotBeContainedByTriangle()
        {
            var vectorDistanceTolerance = 0.01;
            var floatingPointEqualityPrecision = 0.00001;
            VectorEqualityCheck.Tolerance = vectorDistanceTolerance;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.00447213595, 0.50894427191);
            var bcEdge = new Edge(b, c);

            var bcEdgeDistanceFromPoint = bcEdge.DistanceFrom(p);
            var pointInTriangleTestResult = t.ContainsPoint(p);
            
            bcEdgeDistanceFromPoint.Should().BeApproximately(vectorDistanceTolerance, floatingPointEqualityPrecision);
            pointInTriangleTestResult.Should().BeFalse();
        }
        
        [Test]
        public void PointThatFallsBehindBCEdgeAndIsInHigherDistanceThanVectorDistanceToleranceShouldNotBeContainedByTriangle()
        {
            var vectorDistanceTolerance = 0.01;
            VectorEqualityCheck.Tolerance = vectorDistanceTolerance;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(1.0, 0.52);
            var bcEdge = new Edge(b, c);
            
            var bcEdgeDistanceFromPoint = bcEdge.DistanceFrom(p);
            var pointInTriangleTestResult = t.ContainsPoint(p);

            bcEdgeDistanceFromPoint.Should().BeGreaterThan(vectorDistanceTolerance);
            pointInTriangleTestResult.Should().BeFalse();
        }
        
        [Test]
        public void PointThatFallsBehindFirstEndpointButIsCloserThanVectorDistanceToleranceShouldBeContainedByTriangle()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);
            var t = new Triangle(a, b, c, 0);
            var p = new Vector(-0.001, -0.001);

            t.ContainsPoint(p).Should().BeTrue();
        }

        [Test]
        public void CommonEdgeShouldEqualWithOtherTriangleEdgeIfOneSharedPointDiffersLessThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdge = t1.GetCommonEdgeWith(t2);

            var endpointsOfCommonEdge = new[]{ commonEdge.A, commonEdge.B };
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(b));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(c));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(d));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(e));
        }
        
        [Test]
        public void CommonEdgeShouldEqualWithOtherTriangleEdgeIfOneSharedPointDiffersLessThanToleranceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdge = t2.GetCommonEdgeWith(t1);

            var endpointsOfCommonEdge = new[]{ commonEdge.A, commonEdge.B };
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(b));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(c));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(d));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(e));
        }

        [Test]
        public void CommonEdgeShouldEqualWithOtherTriangleEdgeIfNothSharedPointDiffersButLessThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.001);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, -0.001);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdge = t1.GetCommonEdgeWith(t2);

            var endpointsOfCommonEdge = new[]{ commonEdge.A, commonEdge.B };
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(b));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(c));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(d));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(e));
        }
        
        [Test]
        public void CommonEdgeShouldEqualWithOtherTriangleEdgeIfBothSharedPointDiffersButLessThanToleranceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.001);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, -0.001);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdge = t2.GetCommonEdgeWith(t1);

            var endpointsOfCommonEdge = new[]{ commonEdge.A, commonEdge.B };
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(b));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(c));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(d));
            endpointsOfCommonEdge.Should().Contain(point => point.Equals(e));
        }

        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdges()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT1.Equals(commonEdgeQueriedFromT2);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdgesIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT2.Equals(commonEdgeQueriedFromT1);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdgesEvenIfOneEndpointDiffersLessThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT1.Equals(commonEdgeQueriedFromT2);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdgesEvenIfOneEndpointDiffersLessThanToleranceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT2.Equals(commonEdgeQueriedFromT1);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdgesEvenIfBothEndpointsDiffersLessThanTolerance()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.001);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, -0.001);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT1.Equals(commonEdgeQueriedFromT2);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void AcquiringCommonEdgeOfAdjacentTrianglesShouldProduceEqualEdgesEvenIfBothEndpointsDiffersLessThanToleranceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.001);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, -0.001);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var commonEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var commonEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckResult = commonEdgeQueriedFromT2.Equals(commonEdgeQueriedFromT1);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);

            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedOnce()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(f, d, e, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedOnceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(f, d, e, 1);

            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedTwice()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(e, f, d, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedTwiceIndependentlyFromQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(e, f, d, 1);

            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsIndependentlyFromOrderOfPoints()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(f, e, d, 1);

            t1.Equals(t2).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsIndependentlyFromOrderOfPointsAndQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(f, e, d, 1);

            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedOnceIndependentlyFromOrderOfPoints()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, f, e, 1);

            t1.Equals(t2).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedOnceIndependentlyFromOrderOfPointsAndQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, f, e, 1);

            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedTwiceIndependentlyFromOrderOfPoints()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(e, d, f, 1);

            t1.Equals(t2).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfIdenticalPointsShiftedTwiceIndependentlyFromOrderOfPointsAndQueryOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(0.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(e, d, f, 1);

            t2.Equals(t1).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfThreePointsCloserThanToleranceShiftedOnce()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, -0.001);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, 0.02);
            var f = new Vector(0.0, 0.01);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(f, d, e, 1);

            t1.Equals(t2).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesShouldBeEqualIfTheyConsistOfThreePointsCloserThanToleranceShiftedOnceIndependentlyFromOrder()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, -0.001);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.001);
            var d = new Vector(1.0, 0.999);
            var e = new Vector(1.0, 0.02);
            var f = new Vector(0.0, 0.01);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, f, e, 1);

            t1.Equals(t2).Should().BeTrue();
        }

        [Test]
        public void HashCodesOfCommonEdgesOfNeighbourTrianglesShouldBeIdentical()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);
            
            var sharedEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var sharedEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var hashCodeOfEdge1 = sharedEdgeQueriedFromT1.GetHashCode();
            var hashCodeOfEdge2 = sharedEdgeQueriedFromT2.GetHashCode();

            hashCodeOfEdge1.Should().Be(hashCodeOfEdge2);
            hashCodeOfEdge1.Should().NotBe(0);
        }
        
        [Test]
        public void HashCodesOfCommonEdgesOfNeighbourTrianglesShouldBeIdenticalEvenIfBothSharedPointDiffers()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, -1.01);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, -0.99);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);
            
            var sharedEdgeQueriedFromT1 = t1.GetCommonEdgeWith(t2);
            var sharedEdgeQueriedFromT2 = t2.GetCommonEdgeWith(t1);
            var hashCodeOfEdge1 = sharedEdgeQueriedFromT1.GetHashCode();
            var hashCodeOfEdge2 = sharedEdgeQueriedFromT2.GetHashCode();

            hashCodeOfEdge1.Should().Be(hashCodeOfEdge2);
            hashCodeOfEdge1.Should().NotBe(0);
        }
        
        [Test]
        public void EdgesThatAreSharedByTrianglesMeanwhileTheirEndpointsDifferLessThanToleranceShouldHaveIdenticalHashCodes()
        {
            VectorEqualityCheck.Tolerance = 0.1;
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 0.999);
            var d = new Vector(1.0, 1.001);
            var e = new Vector(1.0, 0.0);
            var f = new Vector(2.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(d, e, f, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);

            var distortedEndpointEqualityCheckResult = c.Equals(d);
            var sharedEdgeFromT1 = t1.GetCommonEdgeWith(t2);
            var sharedEdgeFromT2 = t2.GetCommonEdgeWith(t1);
            var equalityCheckT1ToT2 = sharedEdgeFromT1.Equals(sharedEdgeFromT2);
            var equalityCheckT2ToT1 = sharedEdgeFromT2.Equals(sharedEdgeFromT1);
            var hashCodeOfSharedEdgeFromT1 = sharedEdgeFromT1.GetHashCode();
            var hashCodeOfSharedEdgeFromT2 = sharedEdgeFromT2.GetHashCode();

            distortedEndpointEqualityCheckResult.Should().BeTrue();
            equalityCheckT1ToT2.Should().BeTrue();
            equalityCheckT2ToT1.Should().BeTrue();
            var endpointsOfSharedEdgeFromT1 = new[]{sharedEdgeFromT1.A, sharedEdgeFromT1.B };
            var endpointsOfSharedEdgeFromT2 = new[]{sharedEdgeFromT2.A, sharedEdgeFromT2.B };
            endpointsOfSharedEdgeFromT1.Should().Contain(point => point.Equals(b));
            endpointsOfSharedEdgeFromT1.Should().Contain(point => point.Equals(c));
            endpointsOfSharedEdgeFromT1.Should().Contain(point => point.Equals(d));
            endpointsOfSharedEdgeFromT1.Should().Contain(point => point.Equals(e));
            endpointsOfSharedEdgeFromT2.Should().Contain(point => point.Equals(b));
            endpointsOfSharedEdgeFromT2.Should().Contain(point => point.Equals(c));
            endpointsOfSharedEdgeFromT2.Should().Contain(point => point.Equals(d));
            endpointsOfSharedEdgeFromT2.Should().Contain(point => point.Equals(e));
            hashCodeOfSharedEdgeFromT1.Should().Be(hashCodeOfSharedEdgeFromT2);
            hashCodeOfSharedEdgeFromT1.Should().NotBe(0);
        }
        
        [Test]
        public void EqualsShouldWorkWithNullParameter()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(1.0, 1.0);
            var triangle = new Triangle(a, b, c, 0);

            Action gettingEqualsWithNull = () => triangle.Equals(null);
            
            gettingEqualsWithNull.ShouldNotThrow();
        }
        
        [Test]
        public void DistortedTriangleShouldNotBeCreated() 
        {
            var a = new Vector(0.0, 1.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(2.0, 0.0);

            Action triangleInstantiation = () => new Triangle(a, b, c, 0);

            triangleInstantiation.ShouldThrow<ArgumentException>()
                .And.Message.Should().Contain("overlap");
        }
    }
}