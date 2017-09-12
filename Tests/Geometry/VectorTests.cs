﻿using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class VectorTests
    {
        private const double Precision = 0.0001;
        
        [Test]
        public void DistanceOfTwoVectorsShouldBeTheirCartesianDistanceInTwoDimension()
        {
            IVector v1 = new Vector(1.0, 1.0);
            IVector v2 = new Vector(2.0, 2.0);
            var squareRootTwo = 1.41421;

            var distance = v1.DistanceFrom(v2);

            distance.Should().BeApproximately(squareRootTwo, Precision);
        }

        [Test]
        public void DistanceOfTwoVectorsShouldBeIndependentFromOrder()
        {
            IVector v1 = new Vector(1.0, 1.0);
            IVector v2 = new Vector(2.0, 2.0);
            var squareRootTwo = 1.41421;

            var distance = v2.DistanceFrom(v1);

            distance.Should().BeApproximately(squareRootTwo, Precision);
        }

        [Test]
        public void DistanceFromItselfShouldBeZero()
        {
            IVector v2 = new Vector(2.0, 2.0);

            var distance = v2.DistanceFrom(v2);

            distance.Should().BeApproximately(0.0, Precision);
        }

        // values come from examples on www.onlinemathlearning.com
        [Test]
        public void
            SubtractingAnotherVectorShouldResultInNewVectorContainingCoordinatesSubtractedTheSecondOneFromTheFirst()
        {
            IVector v1 = new Vector(2.0, 1.0);
            IVector v2 = new Vector(3.0, -2.0);

            var result = v1.Minus(v2);

            v1.X.Should().BeApproximately(2.0, Precision);
            v1.Y.Should().BeApproximately(1.0, Precision);
            v2.X.Should().BeApproximately(3.0, Precision);
            v2.Y.Should().BeApproximately(-2.0, Precision);
            result.X.Should().BeApproximately(-1.0, Precision);
            result.Y.Should().BeApproximately(3.0, Precision);
        }

        // values come from examples on www.onlinemathlearning.com
        [Test]
        public void AddingAnotherVectorShouldResultInNewVectorContaingSumOfCorrespondingCoordinateValues()
        {
            IVector v1 = new Vector(2.0, 3.0);
            IVector v2 = new Vector(2.0, -2.0);

            var result = v1.Plus(v2);
            
            v1.X.Should().BeApproximately(2.0, Precision);
            v1.Y.Should().BeApproximately(3.0, Precision);
            v2.X.Should().BeApproximately(2.0, Precision);
            v2.Y.Should().BeApproximately(-2.0, Precision);
            result.X.Should().BeApproximately(4.0, Precision);
            result.Y.Should().BeApproximately(1.0, Precision);
        }
        
        [Test]
        public void UnitVectorInClockwiseDirectionShouldBeDeterminedAsClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(1.0, 0.0);

            var result = v.IsInClockWiseDirectionFrom(u);

            result.Should().BeTrue();
        }
        
        [Test]
        public void UnitVectorInCounterClockwiseDirectionShouldBeDeterminedAsCounterClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(-1.0, 0.0);

            var result = v.IsInCounterClockWiseDirectionFrom(u);

            result.Should().BeTrue();
        }
        
        [Test]
        public void UnitVectorInCounterClockwiseDirectionShouldNotBeDeterminedAsClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(-1.0, 0.0);

            var result = v.IsInClockWiseDirectionFrom(u);

            result.Should().BeFalse();
        }
        
        [Test]
        public void UnitVectorInClockwiseDirectionShouldNotBeDeterminedAsCounterClockwise()
        {
            var u = new Vector(0.0, 1.0);
            var v = new Vector(1.0, 0.0);

            var result = v.IsInCounterClockWiseDirectionFrom(u);

            result.Should().BeFalse();
        }
        
        [Test]
        public void ParallelVectorsShouldNotBeDetectedClockwise()
        {
            var p1 = new Vector(1.0, 1.0);
            var p2 = new Vector(3.0, 1.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            var result = pmToP1.IsInCounterClockWiseDirectionFrom(pmToP2);

            result.Should().BeFalse();
        }
        
        [Test]
        public void ParallelVectorsShouldNotBeDeterminedCounterClockwise()
        {
            var pm = new Vector(2.0, 1.0);
            var p1 = new Vector(2.0, 2.0);
            var p2 = new Vector(3.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);

            var result = pmToP1.IsInClockWiseDirectionFrom(pmToP2);

            result.Should().BeFalse();
        }
        
        [Test]
        public void VectorsInCounterClockwiseOrientationShouldBeDeterminedAsCounterClockwise()
        {
            var p2 = new Vector(3.0, 1.0);
            var p1 = new Vector(2.0, 2.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);
            
            var result = pmToP2.IsInClockWiseDirectionFrom(pmToP1);

            result.Should().BeTrue();
        }
        
        [Test]
        public void VectorsInClockwiseOrientationShouldBeDeterminedAsClockwise()
        {
            var p2 = new Vector(3.0, 1.0);
            var p1 = new Vector(2.0, 2.0);
            var pm = new Vector(2.0, 1.0);
            var pmToP1 = p1.Minus(pm);
            var pmToP2 = p2.Minus(pm);
            
            var result = pmToP1.IsInCounterClockWiseDirectionFrom(pmToP2);

            result.Should().BeTrue();
        }

        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqual()
        {
            IVector u = new Vector(2.5, 3.6);
            IVector v = new Vector(2.5, 3.6);

            var equalityCheckResult = u.Equals(v);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqualIndependentlyFromComparisonOrder()
        {
            IVector u = new Vector(2.5, 3.6);
            IVector v = new Vector(2.5, 3.6);

            var equalityCheckResult = v.Equals(u);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void TwoVectorsRepresentingExactlyTheSamePointShouldBeEqualEvenIfTheyAreBoxed()
        {
            object u = new Vector(2.5, 3.2);
            object v = new Vector(2.5, 3.2);

            var equalityCheckResult = u.Equals(v);

            equalityCheckResult.Should().BeTrue();
        }
        
        [Test]
        public void VectorShouldBeEqualToItself()
        {
            IVector u = new Vector(2.5, 3.5);

            var equalityCheckResult = u.Equals(u);

            equalityCheckResult.Should().BeTrue();
        }

        [Test]
        public void VectorsDifferingInOneCoordinateWithOneWholeIntegerShouldNotBeEqual()
        {
            object u = new Vector(2.5, 3.0);
            object v = new Vector(3.5, 3.0);

            var equalityCheckResult = u.Equals(v);

            equalityCheckResult.Should().BeFalse();
        }
    }
}