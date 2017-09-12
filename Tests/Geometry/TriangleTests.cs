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
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfDefinedWithDifferentVectorInstances()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2a, t2b, t2c); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void EqualTrianglesShouldHaveTheSameHashCode()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2a, t2b, t2c); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfDefinedWithDifferentDirection()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2c, t2b, t2a); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfNoneOfThePointsAreMatchingInPair()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2b, t2c, t2a); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }
           
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldBeEqualEvenIfNoneOfThePointsAreMatchingInPairAndOrderIsDifferent()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2 = new Triangle(t2b, t2a, t2c); 
            
            t1.Equals(t2).Should().BeTrue();
            t2.Equals(t1).Should().BeTrue();
        }

        [Test]
        public void TwoTrianglesShouldNotBeEqualIfAnyOfTheirPointsAreDifferent()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.1);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var t1ToT2EqualityCheckResult = t1.Equals(t2);
            
            t1ToT2EqualityCheckResult.Should().BeFalse();
        }
        
        [Test]
        public void TwoTrianglesLyingBetweenSamePointsShouldHaveTheSameHashCodeEvenIfDefinedWithDifferentVectorInstances()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var t1HashCode = t1.GetHashCode();
            var t2HashCode = t2.GetHashCode();
            
            t1HashCode.ShouldBeEquivalentTo(t2HashCode);
        }

        [Test]
        public void GettingCommonEdgesShouldThrowExceptionIfTheTrianglesHaveNoCommonEdge()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.1);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            Action gettingCommonEdge = () => t1.GetCommonEdge(t2);

            gettingCommonEdge.ShouldThrow<ArgumentException>();
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
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

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
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2b, t2c, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrder()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrderAndAreShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfFirstTwoVerticesAreTheSameButArePresentInDifferentOrderAndAreShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(5.0, 5.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(2.0, 2.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSame()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

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
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrder()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrderAndShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
                
        [Test]
        public void TriangleShouldReturnCommonEdgeIfSecondAndThirdVerticesAreTheSameButTrianglesAreDefinedInDifferentOrderAndShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(0.0, 2.0);
            var t2b = new Vector(2.0, 2.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(2.0, 2.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSame()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2a, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButShiftedTwice()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2b, t2c);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
        
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrder()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

            commonEdge.ShouldBeEquivalentTo(expectedCommonEdge);
        }
                
        [Test]
        public void TriangleShouldReturnCommonEdgeIfThirdAndFirstVerticesAreTheSameButAreInDifferentOrderAndShiftedOnce()
        {
            var t1a = new Vector(3.0, 1.0);
            var t1b = new Vector(2.0, 2.0);
            var t1c = new Vector(1.0, 1.0);
            var t2a = new Vector(3.0, 1.0);
            var t2b = new Vector(2.0, 0.0);
            var t2c = new Vector(1.0, 1.0);
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2a, t2c, t2b);

            var commonEdge = t1.GetCommonEdge(t2);

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
            var expectedCommonEdge = new Edge(new Vector(3.0, 1.0), new Vector(1.0, 1.0));
            ITriangle t1 = new Triangle(t1a, t1b, t1c);
            ITriangle t2 = new Triangle(t2c, t2b, t2a);

            var commonEdge = t1.GetCommonEdge(t2);

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
            Triangle t1 = new Triangle(t1a, t1b, t1c);
            Triangle t2 = new Triangle(t2a, t2c, t2b);
            
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
            var t2 = new Triangle(t2a, t2b, t2c);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c);
            
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
            
            var t1 = new Triangle(t1a, t1b, t1c);
            var neighbours = t1.Neighbours;

            neighbours.Should().NotBeNull();
            neighbours.Count().Should().Be(0);
        }
    }
}