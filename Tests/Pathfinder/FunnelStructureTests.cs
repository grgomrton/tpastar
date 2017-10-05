using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class FunnelStructureTests
    {
        [Test]
        public void funnelShouldNotAddVertexIfFirstEdgeLiesOnApex()
        {
            var v1 = new Vector(1.0, 1.0);
            var v2 = new Vector(3.0, 1.0);
            var edge = new Edge(v1, v2);
            var apex = new Vector(2.0, 1.0);
            var funnel = new FunnelStructure(apex);
            
            funnel.StepOver(edge);

            var funnelPoints = funnel.Apex.List;
            funnelPoints.Count.Should().Be(1);
            funnelPoints.First().ShouldBeEquivalentTo(apex);
        }

        [Test]
        public void funnelShouldBeATriangleAfterFirstEdgeIsSteppedOverUnlessApexIsOnEdge()
        {
            var v1 = new Vector(1.0, 1.0);
            var v2 = new Vector(3.0, 1.0);
            var edge = new Edge(v1, v2);
            var apex = new Vector(2.0, 0.0);
            var funnel = new FunnelStructure(apex);
            
            funnel.StepOver(edge);

            var funnelVertices = funnel.Apex.List.ToList();
            funnelVertices.Count.Should().Be(3);
            funnelVertices[0].ShouldBeEquivalentTo(v1);
            funnelVertices[1].ShouldBeEquivalentTo(apex);
            funnelVertices[2].ShouldBeEquivalentTo(v2);
        }

        [Test]
        public void funnelShouldContainVerticesInCounterClockwiseOrderFromLeftToRight()
        {
            var v1 = new Vector(0.0, 1.5);
            var v2 = new Vector(1.0, 1.0);
            var apex = new Vector(2.0, 0.0);
            var v3 = new Vector(3.0, 1.0);
            var v4 = new Vector(5.0, 1.5);
            var edge1 = new Edge(v2, v3);
            var edge2 = new Edge(v3, v1);
            var edge3 = new Edge(v1, v4);
            var funnel = new FunnelStructure(apex);
            
            funnel.StepOver(edge1);
            funnel.StepOver(edge2);
            funnel.StepOver(edge3);

            var funnelVertices = funnel.Apex.List.ToList();
            funnelVertices.Count.Should().Be(5);
            funnelVertices[0].ShouldBeEquivalentTo(v1);
            funnelVertices[1].ShouldBeEquivalentTo(v2);
            funnelVertices[2].ShouldBeEquivalentTo(apex);
            funnelVertices[3].ShouldBeEquivalentTo(v3);
            funnelVertices[4].ShouldBeEquivalentTo(v4);
        }
    }
}