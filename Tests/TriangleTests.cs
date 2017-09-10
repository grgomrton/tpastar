using System;
using CommonTools.Geometry;
using FluentAssertions;
using NUnit.Framework;

namespace TPAStar.Tests
{
    [TestFixture]
    public class TriangleTests
    {
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfDefinedWithDifferentVectorInstances()
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
        
        [Test]
        public void EqualTrianglesShouldHaveTheSameHashCode()
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
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfDefinedWithDifferentDirection()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2c, t2b, t2a); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfNoneOfThePointsAreMatchingInPair()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2b, t2c, t2a); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
           
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfNoneOfThePointsAreMatchingInPairAndOrderIsDifferent()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2b, t2a, t2c); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldNotBeEqualIfAnyOfTheirPointsAreDifferent()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.1);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var t1ToT2EqualityCheckResult = t1.Equals(t2);
            
            t1ToT2EqualityCheckResult.Should().BeFalse();
        }
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldHaveTheSameHashCodeEvenIfDefinedWithDifferentVectorInstances()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var t1HashCode = t1.GetHashCode();
            var t2HashCode = t2.GetHashCode();
            
            t1HashCode.ShouldBeEquivalentTo(t2HashCode);
        }

        [Test]
        public void GettingCommonEdgesShouldThrowExceptionIfTheTrianglesHaveNoCommonEdge()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.1);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            Action gettingCommonEdge = () => t1.GetCommonEdge(t2);

            gettingCommonEdge.ShouldThrow<ArgumentException>();
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSame()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2b, t2c, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrder()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrderAndAreShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrderAndAreShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSame()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrder()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrderAndShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
                
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrderAndShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(0.0, 2.0);
            var t2b = new Vector3(2.0, 2.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(2.0, 2.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSame()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrder()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
                
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrderAndShiftedOnce()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrderAndShiftedTwice()
        {
            var t1a = new Vector3(3.0, 1.0);
            var t1b = new Vector3(2.0, 2.0);
            var t1c = new Vector3(1.0, 1.0);
            var t2a = new Vector3(3.0, 1.0);
            var t2b = new Vector3(2.0, 0.0);
            var t2c = new Vector3(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector3(3.0, 1.0), new Vector3(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
    }
}