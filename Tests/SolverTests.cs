using System;
using CommonTools.Geometry;
using FluentAssertions;
using NUnit.Framework;
using PathFinder.TPAStar;

namespace TPAStar.Tests
{
    [TestFixture]
    public class SolverTests
    {
        private static double Precision = 0.00001;
        
        [Test]
        public void pathBetweenStartAndGoalInTheSameTriangleShouldBeAStraightLine()
        {
            var a = new Vector3(5.0, 5.0);
            var b = new Vector3(5.0, 10.0);
            var c = new Vector3(10.0, 7.5);
            var t1 = new Triangle(a, b, c);
            var s = new Vector3(5.5, 7.5);
            var g = new Vector3(7.0, 7.5);
            var solver = new TPAStarSolver();
            
            var path = solver.FindPath(s, t1, new [] { g });
            
            path.Count.Should().Be(2);
            path.Length.Should().BeApproximately(1.5, Precision);
            var pathNodes = path.ToList();
            pathNodes[0].X.Should().BeApproximately(5.5, Precision);
            pathNodes[0].Y.Should().BeApproximately(7.5, Precision);
            pathNodes[1].X.Should().BeApproximately(7.0, Precision);
            pathNodes[1].Y.Should().BeApproximately(7.5, Precision);
        }
        
        [Test]
        public void pathBetweenTwoPointsWithoutObstacleShouldBeAStraightLine()
        {
            var t1a = new Vector3(10.0, 7.5);
            var t1b = new Vector3(5.0, 10.0);
            var t1c = new Vector3(5.0, 5.0);   
            var t1 = new Triangle(t1a, t1b, t1c);
            var t2a = new Vector3(10.0, 7.5);
            var t2b = new Vector3(10.0, 12.5);
            var t2c = new Vector3(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);
            var s = new Vector3(7.5, 7.5);
            var g = new Vector3(7.5, 10.0);
            var solver = new TPAStarSolver();
            
            var path = solver.FindPath(s, t1, new [] { g });
            
            path.Count.Should().Be(2);
            path.Length.Should().BeApproximately(2.5, Precision);
            var resultPath = path.ToList();
            resultPath[0].X.Should().BeApproximately(7.5, Precision);
            resultPath[0].Y.Should().BeApproximately(7.5, Precision);
            resultPath[1].X.Should().BeApproximately(7.5, Precision);
            resultPath[1].Y.Should().BeApproximately(10.0, Precision);
        }

        [Test]
        public void pathWithACornerBetweenTwoPointsShouldPassByTheCorner()
        {
            double sqrt2 = 1.41421;
            double sqrt5 = 2.23607;
            double sqrt2PlusSqrt5 = 3.65028;
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
            var g = new Vector3(12.0, 13.5);
            var solver = new TPAStarSolver();
            
            var path = solver.FindPath(s, t2, new [] { g });
            
            path.Count.Should().Be(3);
            path.Length.Should().BeApproximately(sqrt2PlusSqrt5, Precision);
            var pathNodes = path.ToList();
            pathNodes[1].X.Should().BeApproximately(10.0, Precision);
            pathNodes[1].Y.Should().BeApproximately(12.5, Precision);
        }
        
        [Test]
        public void betweenMultipleGoalsWhenACloserPointIsPresentStraightAheadPathShouldLeadToThatOne()
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
            var g1 = new Vector3(12.0, 13.5);
            var g2 = new Vector3(9.0, 14.5);
            var solver = new TPAStarSolver();
            
            var path = solver.FindPath(s, t2, new [] { g1, g2 });
            
            path.Count.Should().Be(2);
            path.Length.Should().BeApproximately(3.0, Precision);
            var pathNodes = path.ToList();
            pathNodes[0].X.Should().BeApproximately(9.0, Precision);
            pathNodes[0].Y.Should().BeApproximately(11.5, Precision);
            pathNodes[1].X.Should().BeApproximately(9.0, Precision);
            pathNodes[1].Y.Should().BeApproximately(14.5, Precision);
        }
        
        // TODO add test for not entering the same triangle due to exceeding edge threshold
    }
}