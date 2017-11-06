using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    // in order to have timeout support the assert and act sections are switched
    [TestFixture]
    public class TPAStarTests
    {
        private static double AssertionPrecision = 0.00001;
        private static int TimeOutInMillseconds = 1000;
        
        [OneTimeSetUp]
        public void BeforeTheseTestCases()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }

        [Test]
        public void PathFinderShouldThrowExceptionIfStartTriangleDoesNotContainStartPoint()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(1.0, 0.0);
            var triangle = new Triangle(a, b, c, 0);
            var start = new Vector(-1.0, -1.0);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                pathFindingOutcome.Exception.Should().NotBeNull();
                var innerExceptions = pathFindingOutcome.Exception.InnerExceptions;
                innerExceptions.Should().Contain(exception => exception.GetType() == typeof(ArgumentException));
                var argumentException = innerExceptions.First(exception => exception.GetType() == typeof(ArgumentException));
                argumentException.Message.Should().Contain("fall");
            };
            
            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, triangle, Enumerable.Empty<IVector>()), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);
        }
        
        [Test]
        public void PathBetweenStartAndGoalInTheSameTriangleShouldBeAStraightLine()
        {
            var a = new Vector(5.0, 5.0);
            var b = new Vector(5.0, 10.0);
            var c = new Vector(10.0, 7.5);
            var t1 = new Triangle(a, b, c, 0);
            var s = new Vector(5.5, 7.5);
            var g = new Vector(7.0, 7.5);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;

            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(2);
                var pathLength = path.Zip(path.Skip(1), (v1, v2) => v1.DistanceFrom(v2)).Sum();
                pathLength.Should().BeApproximately(1.5, AssertionPrecision);
                var pathNodes = path.ToList();
                pathNodes[0].X.Should().BeApproximately(5.5, AssertionPrecision);
                pathNodes[0].Y.Should().BeApproximately(7.5, AssertionPrecision);
                pathNodes[1].X.Should().BeApproximately(7.0, AssertionPrecision);
                pathNodes[1].Y.Should().BeApproximately(7.5, AssertionPrecision);
            };
            
            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(s, t1, new[]{ g }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);
        }
        
        [Test]
        public void PathBetweenTwoPointsWithoutObstacleShouldBeAStraightLine()
        {
            var t1a = new Vector(10.0, 7.5);
            var t1b = new Vector(5.0, 10.0);
            var t1c = new Vector(5.0, 5.0);   
            var t1 = new Triangle(t1a, t1b, t1c, 0);
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);
            var s = new Vector(7.5, 7.5);
            var g = new Vector(7.5, 10.0);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;

            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(2);
                var pathLength = path.Zip(path.Skip(1), (v1, v2) => v1.DistanceFrom(v2)).Sum();
                pathLength.Should().BeApproximately(2.5, AssertionPrecision);
                var resultPath = path.ToList();
                resultPath[0].X.Should().BeApproximately(7.5, AssertionPrecision);
                resultPath[0].Y.Should().BeApproximately(7.5, AssertionPrecision);
                resultPath[1].X.Should().BeApproximately(7.5, AssertionPrecision);
                resultPath[1].Y.Should().BeApproximately(10.0, AssertionPrecision);
            };
            
            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(s, t1, new IVector[] { g }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);
        }

        [Test]
        public void PathWithACornerBetweenTwoPointsShouldPassByTheCorner()
        {
            double sqrt2PlusSqrt5 = 3.65028;
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 1);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c, 2);
            var t5a = new Vector(15.0, 12.5);
            var t5b = new Vector(12.5, 15.0);
            var t5c = new Vector(10.0, 12.5);
            var t5 = new Triangle(t5a, t5b, t5c, 3);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            t4.SetNeighbours(t3, t5);
            t5.SetNeighbours(t4);
            var s = new Vector(9.0, 11.5);
            var g = new Vector(12.0, 13.5);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;

            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(3);
                var pathLength = path.Zip(path.Skip(1), (v1, v2) => v1.DistanceFrom(v2)).Sum();
                pathLength.Should().BeApproximately(sqrt2PlusSqrt5, AssertionPrecision);
                var pathNodes = path.ToList();
                pathNodes[1].X.Should().BeApproximately(10.0, AssertionPrecision);
                pathNodes[1].Y.Should().BeApproximately(12.5, AssertionPrecision);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(s, t2, new IVector[] {g}), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);
        }
        
        [Test]
        public void BetweenMultipleGoalsWhenACloserPointIsPresentStraightAheadPathShouldLeadToThatOne()
        {
            var t2a = new Vector(10.0, 7.5);
            var t2b = new Vector(10.0, 12.5);
            var t2c = new Vector(5.0, 10.0);
            var t2 = new Triangle(t2a, t2b, t2c, 0);
            var t3a = new Vector(5.0, 10.0);
            var t3b = new Vector(10.0, 12.5);
            var t3c = new Vector(5.0, 15.0);
            var t3 = new Triangle(t3a, t3b, t3c, 1);
            var t4a = new Vector(10.0, 12.5);
            var t4b = new Vector(12.5, 15.0);
            var t4c = new Vector(5.0, 15.0);
            var t4 = new Triangle(t4a, t4b, t4c, 2);
            var t5a = new Vector(15.0, 12.5);
            var t5b = new Vector(12.5, 15.0);
            var t5c = new Vector(10.0, 12.5);
            var t5 = new Triangle(t5a, t5b, t5c, 3);
            t2.SetNeighbours(t3);
            t3.SetNeighbours(t2, t4);
            t4.SetNeighbours(t3, t5);
            t5.SetNeighbours(t4);
            var s = new Vector(9.0, 11.5);
            var g1 = new Vector(12.0, 13.5);
            var g2 = new Vector(9.0, 14.5);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;

            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(2);
                var pathLength = path.Zip(path.Skip(1), (v1, v2) => v1.DistanceFrom(v2)).Sum();
                pathLength.Should().BeApproximately(3.0, AssertionPrecision);
                var pathNodes = path.ToList();
                pathNodes[0].X.Should().BeApproximately(9.0, AssertionPrecision);
                pathNodes[0].Y.Should().BeApproximately(11.5, AssertionPrecision);
                pathNodes[1].X.Should().BeApproximately(9.0, AssertionPrecision);
                pathNodes[1].Y.Should().BeApproximately(14.5, AssertionPrecision);                
            };
            
            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(s, t2, new IVector[]{ g1, g2 }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);
        }

        [Test]
        public void PathFinderShouldStopInPolygonWithHoleIfNoGoalCanBeReached()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(3.0, 0.0);
            var dp = new Vector(2.0, 1.0);
            var dn = new Vector(2.0, -1.0);
            var ep = new Vector(2.0, 2.0);
            var en = new Vector(2.0, -2.0);
            var f = new Vector(4.0, 0.0);
            var start = new Vector(1.0, 0.5);
            var unreachableGoal = new Vector(2.0, 0.0);
            var t1 = new Triangle(a,b,dp, 1);
            var t2 = new Triangle(a,b,dn, 2);
            var t3 = new Triangle(a,dp, ep, 3);
            var t4 = new Triangle(a,dn,en, 4);
            var t5 = new Triangle(ep,dp,c, 5);
            var t6 = new Triangle(en,dn,c, 6);
            var t7 = new Triangle(ep,f,c, 7);
            var t8 = new Triangle(en,f,c, 8);
            var triangles = new[] {t1, t2, t3, t4, t5, t6, t7, t8};
            foreach (var triangle in triangles)
            {
                var neighbours = triangles.Where(other => triangle.GetCommonVerticesWith(other).Count() == 2).ToArray();
                triangle.SetNeighbours(neighbours);
            }
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(1);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, t1, new IVector[]{ unreachableGoal }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);   
        }
        
        [Test]
        public void PathFinderShouldFindShortestPathInDiamondShapedPolygonWithMultipleGoals()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(3.0, 0.0);
            var dp = new Vector(2.0, 1.0);
            var dn = new Vector(2.0, -1.0);
            var ep = new Vector(2.0, 2.0);
            var en = new Vector(2.0, -2.0);
            var f = new Vector(4.0, 0.0);
            var start = new Vector(0.5, 0.0);
            var goalOnExactlyTheOppositeSide = new Vector(3.5, 0.0);
            var goalOnPositivePartOfTriangle = new Vector(3.4, 0.1);
            var t1 = new Triangle(a,b,dp, 1);
            var t2 = new Triangle(a,b,dn, 2);
            var t3 = new Triangle(a,dp, ep, 3);
            var t4 = new Triangle(a,dn,en, 4);
            var t5 = new Triangle(ep,dp,c, 5);
            var t6 = new Triangle(en,dn,c, 6);
            var t7 = new Triangle(ep,f,c, 7);
            var t8 = new Triangle(en,f,c, 8);
            var triangles = new[] {t1, t2, t3, t4, t5, t6, t7, t8};
            foreach (var triangle in triangles)
            {
                var neighbours = triangles.Where(other => triangle.GetCommonVerticesWith(other).Count() == 2).ToArray();
                triangle.SetNeighbours(neighbours);
            }
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(3);
                path[0].Should().Be(start);
                path[2].Should().Be(goalOnPositivePartOfTriangle);
                path[1].Should().Be(dp);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, t1, new IVector[]{ goalOnExactlyTheOppositeSide, goalOnPositivePartOfTriangle }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);   
        }
        
        [Test]
        public void PathFinderShouldFindPathInPolygonWithHoleAndBeAbleToChooseFromTwoPathsWithIdenticalLength()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(3.0, 0.0);
            var dp = new Vector(2.0, 1.0);
            var dn = new Vector(2.0, -1.0);
            var ep = new Vector(2.0, 2.0);
            var en = new Vector(2.0, -2.0);
            var f = new Vector(4.0, 0.0);
            var start = new Vector(0.5, 0.0);
            var goalOnExactlyTheOppositeSide = new Vector(3.5, 0.0);
            var t1 = new Triangle(a,b,dp, 1);
            var t2 = new Triangle(a,b,dn, 2);
            var t3 = new Triangle(a,dp, ep, 3);
            var t4 = new Triangle(a,dn,en, 4);
            var t5 = new Triangle(ep,dp,c, 5);
            var t6 = new Triangle(en,dn,c, 6);
            var t7 = new Triangle(ep,f,c, 7);
            var t8 = new Triangle(en,f,c, 8);
            var triangles = new[] {t1, t2, t3, t4, t5, t6, t7, t8};
            foreach (var triangle in triangles)
            {
                var neighbours = triangles.Where(other => triangle.GetCommonVerticesWith(other).Count() == 2).ToArray();
                triangle.SetNeighbours(neighbours);
            }
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(3);
                path[0].Should().Be(start);
                path[2].Should().Be(goalOnExactlyTheOppositeSide);
                (dp.Equals(path[1]) || dn.Equals(path[1])).Should().BeTrue();
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, t1, new IVector[]{ goalOnExactlyTheOppositeSide }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);   
        }

        [Test]
        public void PathFinderShouldBeExecutableFromTriangleHavingThreeNeighbours()
        {
            var a = new Vector(-1.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(0.0, 1.0);
            var t1 = new Triangle(a, b, c, 1);
            var d = new Vector(-1.0, 2.0);
            var t2 = new Triangle(d, a, c, 2);
            t2.SetNeighbours(t1);
            var e = new Vector(1.0, 2.0);
            var t3 = new Triangle(b, c, e, 3);
            t3.SetNeighbours(t1);
            var f = new Vector(0.0, -2.0);
            var t4 = new Triangle(a, b, f, 4);
            t4.SetNeighbours(t1);
            t1.SetNeighbours(t2, t3, t4);
            var start = new Vector(0.0, 0.5);
            var goalInT2CornerPoint = new Vector(-1.0, 1.0);
            var goalInT3 = new Vector(0.8, 0.8);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(2);
                path[0].Should().Be(start);
                path[1].Should().Be(goalInT3);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, t1, new IVector[]{ goalInT2CornerPoint, goalInT3 }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);   
        }

        [Test]
        public void IfStartTriangleContainsGoalPointButThereIsAnotherGoalInAnAdjacentTriangleCloserToStartPathFinderShouldFindPathToThatOne()
        {
            var a = new Vector(0.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(0.0, 1.0);
            var d = new Vector(-1.0, 0.0);
            var t1 = new Triangle(a, b, c, 0);
            var t2 = new Triangle(a, d, c, 1);
            t1.SetNeighbours(t2);
            t2.SetNeighbours(t1);
            var start = new Vector(0.1, 0.1);
            var goalInT1 = new Vector(0.5, 0.1);
            var goalInT2 = new Vector(-0.1, 0.1);
            var goals = new[] {goalInT1, goalInT2};
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(2);
                path[0].Should().Be(start);
                path[1].Should().Be(goalInT2);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(start, t1, goals), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds); 
        }
        
        [Test]
        public void PathFinderShouldBeExecutableInPolygonContainingTriangleWithThreeNeighbours()
        {
            var a = new Vector(-1.0, 0.0);
            var b = new Vector(1.0, 0.0);
            var c = new Vector(0.0, 1.0);
            var t1 = new Triangle(a, b, c, 1);
            var d = new Vector(-1.0, 2.0);
            var t2 = new Triangle(d, a, c, 2);
            t2.SetNeighbours(t1);
            var e = new Vector(1.0, 2.0);
            var t3 = new Triangle(b, c, e, 3);
            t3.SetNeighbours(t1);
            var f = new Vector(0.0, -2.0);
            var t4 = new Triangle(a, b, f, 4);
            t4.SetNeighbours(t1);
            t1.SetNeighbours(t2, t3, t4);
            var startInT3 = new Vector(0.8, 1.8);
            var goalInT2CornerPoint = new Vector(-1.0, 2.0);
            var pathFinder = new TPAStarPathFinder();
            var cancelAfterTimeout = new CancellationTokenSource(TimeOutInMillseconds).Token;
            
            Action<Task<IEnumerable<IVector>>> pathAssertion = pathFindingOutcome =>
            {
                var path = pathFindingOutcome.Result.ToList();
                path.Count.Should().Be(3);
                path[0].Should().Be(startInT3);
                path[2].Should().Be(goalInT2CornerPoint);
                path[1].Should().Be(c);
            };

            Task<IEnumerable<IVector>>.Factory
                .StartNew(() => pathFinder.FindPath(startInT3, t3, new IVector[]{ goalInT2CornerPoint }), cancelAfterTimeout)
                .ContinueWith(pathAssertion, cancelAfterTimeout)
                .Wait(2 * TimeOutInMillseconds);   
        }
        
    }
}