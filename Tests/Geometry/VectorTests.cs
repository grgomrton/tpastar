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
    public class VectorTests
    {
        private static double AssertionPrecision = 0.0001;

        [SetUp]
        public void BeforeEachTest()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }
        
        [Test]
        public void DistanceOfTwoVectorsShouldBeTheirCartesianDistanceInTwoDimension()
        {
            IVector v1 = new Vector(1.0, 1.0);
            IVector v2 = new Vector(2.0, 2.0);
            var squareRootTwo = 1.41421;

            v1.DistanceFrom(v2).Should().BeApproximately(squareRootTwo, AssertionPrecision);
        }

        [Test]
        public void DistanceOfTwoVectorsShouldBeIndependentFromOrder()
        {
            IVector v1 = new Vector(1.0, 1.0);
            IVector v2 = new Vector(2.0, 2.0);
            var squareRootTwo = 1.41421;

            v2.DistanceFrom(v1).Should().BeApproximately(squareRootTwo, AssertionPrecision);
        }

        [Test]
        public void DistanceFromItselfShouldBeZero()
        {
            IVector v2 = new Vector(2.0, 2.0);

            v2.DistanceFrom(v2).Should().BeApproximately(0.0, AssertionPrecision);
        }

        [Test]
        public void SubtractionShouldNotChangeInputParameters()
        {
            IVector v1 = new Vector(2.0, 1.0);
            IVector v2 = new Vector(3.0, -2.0);

            v1.Minus(v2);

            v1.X.Should().BeApproximately(2.0, AssertionPrecision);
            v1.Y.Should().BeApproximately(1.0, AssertionPrecision);
            v2.X.Should().BeApproximately(3.0, AssertionPrecision);
            v2.Y.Should().BeApproximately(-2.0, AssertionPrecision);  
        }
        
        // values come from examples on www.onlinemathlearning.com
        [Test]
        public void SubtractingAnotherVectorShouldResultInNewVectorContainingCoordinatesSubtractedTheSecondOneFromTheFirst()
        {
            IVector v1 = new Vector(2.0, 1.0);
            IVector v2 = new Vector(3.0, -2.0);

            var result = v1.Minus(v2);

            result.X.Should().BeApproximately(-1.0, AssertionPrecision);
            result.Y.Should().BeApproximately(3.0, AssertionPrecision);
        }

        [Test]
        public void AdditionShouldNotChangeInputParameters()
        {
            IVector v1 = new Vector(2.0, 3.0);
            IVector v2 = new Vector(2.0, -2.0);

            v1.Plus(v2);
            
            v1.X.Should().BeApproximately(2.0, AssertionPrecision);
            v1.Y.Should().BeApproximately(3.0, AssertionPrecision);
            v2.X.Should().BeApproximately(2.0, AssertionPrecision);
            v2.Y.Should().BeApproximately(-2.0, AssertionPrecision);
        }

        // values come from examples on www.onlinemathlearning.com
        [Test]
        public void AddingAnotherVectorShouldResultInNewVectorContaingSumOfCorrespondingCoordinateValues()
        {
            IVector v1 = new Vector(2.0, 3.0);
            IVector v2 = new Vector(2.0, -2.0);

            var result = v1.Plus(v2);
            
            result.X.Should().BeApproximately(4.0, AssertionPrecision);
            result.Y.Should().BeApproximately(1.0, AssertionPrecision);
        }
        
        [Test]
        public void UnitVectorInClockwiseDirectionShouldBeDeterminedAsClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(1.0, 0.0);

            v.IsInClockWiseDirectionFrom(u).Should().BeTrue();
        }
        
        [Test]
        public void UnitVectorInCounterClockwiseDirectionShouldBeDeterminedAsCounterClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(-1.0, 0.0);

            v.IsInCounterClockWiseDirectionFrom(u).Should().BeTrue();
        }
        
        [Test]
        public void UnitVectorInCounterClockwiseDirectionShouldNotBeDeterminedAsClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(-1.0, 0.0);

            v.IsInClockWiseDirectionFrom(u).Should().BeFalse();
        }
        
        [Test]
        public void UnitVectorInClockwiseDirectionShouldNotBeDeterminedAsCounterClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(1.0, 0.0);

            v.IsInCounterClockWiseDirectionFrom(u).Should().BeFalse();
        }
        
        [Test]
        public void ParallelVectorsShouldBeDeterminedClockwise()
        {
            var p1 = new Vector(1.0, 1.0);
            var p2 = new Vector(3.0, 1.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            pmToP1.IsInClockWiseDirectionFrom(pmToP2).Should().BeTrue();
        }
        
        [Test]
        public void ParallelVectorsShouldBeDeterminedCounterClockwise()
        {
            var p1 = new Vector(1.0, 1.0);
            var p2 = new Vector(3.0, 1.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            pmToP1.IsInCounterClockWiseDirectionFrom(pmToP2).Should().BeTrue();
        }
        
        [Test]
        public void VectorsInCounterClockwiseOrientationShouldBeDeterminedAsCounterClockwise()
        {
            var p2 = new Vector(3.0, 1.0);
            var p1 = new Vector(2.0, 2.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            pmToP2.IsInClockWiseDirectionFrom(pmToP1).Should().BeTrue();
        }
        
        [Test]
        public void VectorsInClockwiseOrientationShouldBeDeterminedAsClockwise()
        {
            var p2 = new Vector(3.0, 1.0);
            var p1 = new Vector(2.0, 2.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            pmToP1.IsInCounterClockWiseDirectionFrom(pmToP2).Should().BeTrue();
        }

        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqual()
        {
            IVector u = new Vector(2.5, 3.6);
            IVector v = new Vector(2.5, 3.6);

            u.Equals(v).Should().BeTrue();
        }
        
        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqualIndependentlyFromComparisonOrder()
        {
            IVector u = new Vector(2.5, 3.6);
            IVector v = new Vector(2.5, 3.6);

            v.Equals(u).Should().BeTrue();
        }
        
        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqualEvenIfTheyAreBoxed()
        {
            object u = new Vector(2.5, 3.2);
            object v = new Vector(2.5, 3.2);

            u.Equals(v).Should().BeTrue();
        }
        
        [Test]
        public void VectorShouldBeEqualToItself()
        {
            IVector u = new Vector(2.5, 3.5);

            u.Equals(u).Should().BeTrue();
        }

        [Test]
        public void VectorsDifferingExactlyTheSameAmountAsTheToleranceShouldNotBeEqual()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var u = new Vector(1.0, 1.0);
            var v = new Vector(1.01, 1.0);

            u.Equals(v).Should().BeFalse();
        }
        
        [Test]
        public void VectorsDifferingLessThanTheToleranceShouldBeEqual()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var u = new Vector(1.0, 1.0);
            var v = new Vector(1.005, 1.0);

            u.Equals(v).Should().BeTrue();
        }

        
        [Test]
        public void VectorsDifferingMoreThanTheToleranceShouldNotBeEqual()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var u = new Vector(1.01, 1.01);
            var v = new Vector(1.0, 1.0);

            u.Equals(v).Should().BeFalse();
        }
        
        [Test]
        public void VectorsDifferingInBothCoordinatesButAreCloserThanToleranceShouldBeEqual()
        {
            VectorEqualityCheck.Tolerance = 0.01;
            var u = new Vector(1.001, 1.0);
            var v = new Vector(1.0, 1.001);

            u.Equals(v).Should().BeTrue();
        }
        
        [Test]
        public void EqualsShouldWorkWithNullParameter()
        {
            var vector = new Vector(1.0, 0.0);

            Action gettingEqualsWithNull = () => vector.Equals(null);
            
            gettingEqualsWithNull.ShouldNotThrow();
        }
        
    }
}