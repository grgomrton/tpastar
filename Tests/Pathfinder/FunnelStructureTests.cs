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

using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TriangulatedPolygonAStar.BasicGeometry;

namespace TriangulatedPolygonAStar.Tests
{
    [TestFixture]
    public class FunnelStructureTests
    {
        [OneTimeSetUp]
        public void BeforeTheseTestCases()
        {
            VectorEqualityCheck.Tolerance = 0.001;
        }

        [Test]
        public void FunnelAndFunnelPathShouldContainStartPointAfterInit()
        {
            var startPoint = new Vector(1.0, 2.0);
            
            var funnel = new FunnelStructure(startPoint);

            funnel.Apex.List.Count.Should().Be(1);
            funnel.Apex.Value.Should().Be(startPoint);
            funnel.Path.Count.Should().Be(1);
            funnel.Path.First.Value.Should().Be(startPoint);
        }
        
        [Test]
        public void FunnelShouldContainVerticesInCounterClockwiseOrder()
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
            funnelVertices[0].Should().Be(v1);
            funnelVertices[1].Should().Be(v2);
            funnelVertices[2].Should().Be(apex);
            funnelVertices[3].Should().Be(v3);
            funnelVertices[4].Should().Be(v4);
        }
        
        [Test]
        public void FunnelShouldBeATriangleAfterFirstEdgeIsSteppedOver()
        {
            var a = new Vector(1.0, 1.0);
            var b = new Vector(3.0, 1.0);
            var edge = new Edge(a, b);
            var apex = new Vector(2.0, 0.0);
            var funnel = new FunnelStructure(apex);
            
            funnel.StepOver(edge);

            var funnelVertices = funnel.Apex.List.ToList();
            funnelVertices.Count.Should().Be(3);
            funnelVertices[0].Should().Be(a);
            funnelVertices[1].Should().Be(apex);
            funnelVertices[2].Should().Be(b);
        }
        
        [Test]
        public void FunnelShouldSkipFirstEdgeIfApexLiesOnIt()
        {
            var v1 = new Vector(1.0, 1.0);
            var v2 = new Vector(3.0, 1.0);
            var edge = new Edge(v1, v2);
            var apex = new Vector(2.0, 1.0);
            var funnel = new FunnelStructure(apex);
            
            funnel.StepOver(edge);

            var funnelPoints = funnel.Apex.List;
            funnelPoints.Count.Should().Be(1);
            funnelPoints.First.Value.Should().Be(apex);
        }

        [Test]
        public void FunnelShouldRemovePointThatCreatesABump()
        {
            var origin = new Vector(0.0, 0.0);
            var a = new Vector(1.0, 1.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(1.0, 2.0);
            var firstEdge = new Edge(a, b);
            var secondEdge = new Edge(a, c);
            var expectedApexAfterSecondEdgeIsAdded = new Vector(0.0, 0.0);
            var expectedLeftEndpointAfterSecondEdgeIsAdded = new Vector(1.0, 2.0);
            var expectedRightEndpointAfterSecondEdgeIsAdded = new Vector(1.0, 1.0);
            var funnel = new FunnelStructure(origin);
            
            funnel.StepOver(firstEdge);
            var funnelContainsPointBAfterFirstEdgeIsAdded = funnel.Apex.List.Contains(b);
            funnel.StepOver(secondEdge);
            var funnelContainsPointBAfterSecondEdgeIsAdded = funnel.Apex.List.Contains(b);

            funnel.Apex.List.Count.Should().Be(3);
            funnelContainsPointBAfterFirstEdgeIsAdded.Should().BeTrue();
            funnelContainsPointBAfterSecondEdgeIsAdded.Should().BeFalse();
            funnel.Apex.Value.Should().Be(expectedApexAfterSecondEdgeIsAdded);
            funnel.Apex.List.First.Value.Should().Be(expectedLeftEndpointAfterSecondEdgeIsAdded);
            funnel.Apex.List.Last.Value.Should().Be(expectedRightEndpointAfterSecondEdgeIsAdded);
            funnel.Path.Count.Should().Be(1);
            funnel.Path.First.Value.Should().Be(origin);
        }

        [Test]
        public void FunnelShouldRemovePointThatCreatesABumpOnTheRightFunnelSide()
        {
            var origin = new Vector(0.0, 0.0);
            var a = new Vector(-1.0, 1.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(-1.0, 2.0);
            var firstEdge = new Edge(a, b);
            var secondEdge = new Edge(a, c);
            var expectedApexAfterSecondEdgeIsAdded = new Vector(0.0, 0.0);
            var expectedLeftEndpointAfterSecondEdgeIsAdded = new Vector(-1.0, 1.0);
            var expectedRightEndpointAfterSecondEdgeIsAdded = new Vector(-1.0, 2.0);
            var funnel = new FunnelStructure(origin);
            
            funnel.StepOver(firstEdge);
            var funnelContainsPointBAfterFirstEdgeIsAdded = funnel.Apex.List.Contains(b);
            funnel.StepOver(secondEdge);
            var funnelContainsPointBAfterSecondEdgeIsAdded = funnel.Apex.List.Contains(b);

            funnel.Apex.List.Count.Should().Be(3);
            funnelContainsPointBAfterFirstEdgeIsAdded.Should().BeTrue();
            funnelContainsPointBAfterSecondEdgeIsAdded.Should().BeFalse();
            funnel.Apex.Value.Should().Be(expectedApexAfterSecondEdgeIsAdded);
            funnel.Apex.List.First.Value.Should().Be(expectedLeftEndpointAfterSecondEdgeIsAdded);
            funnel.Apex.List.Last.Value.Should().Be(expectedRightEndpointAfterSecondEdgeIsAdded);
            funnel.Path.Count.Should().Be(1);
            funnel.Path.First.Value.Should().Be(origin);
        }

        
        [Test]
        public void FunnelShouldPopApexOnceAKnotIsCreated()
        {
            var origin = new Vector(0.0, 0.0);
            var a = new Vector(1.0, 1.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(1.0, 2.0);
            var d = new Vector(2.0, 0.0);
            var firstEdge = new Edge(a, b);
            var secondEdge = new Edge(a, c);
            var thirdEdge = new Edge(d, a);
            var expectedApexAfterThirdEdgeIsAdded = new Vector(1.0, 1.0);
            var expectedLeftEndpointAfterThirdEdgeIsAdded = new Vector(2.0, 0.0);
            var expectedRightEndpointAfterThirdEdgeIsAdded = expectedApexAfterThirdEdgeIsAdded;
            
            var funnel = new FunnelStructure(origin);
            funnel.StepOver(firstEdge);
            funnel.StepOver(secondEdge);
            funnel.StepOver(thirdEdge);

            funnel.Apex.List.Count.Should().Be(2);
            funnel.Apex.Value.Should().Be(expectedApexAfterThirdEdgeIsAdded);
            funnel.Apex.List.First.Value.Should().Be(expectedLeftEndpointAfterThirdEdgeIsAdded);
            funnel.Apex.List.Last.Value.Should().Be(expectedRightEndpointAfterThirdEdgeIsAdded);
            funnel.Path.Count.Should().Be(2);
            funnel.Path.First.Value.Should().Be(origin);
            funnel.Path.First.Next.Value.Should().Be(a);
        }

        
        [Test]
        public void FunnelShouldPopMultiplePointsIfAKnotInvalidatesThem()
        {
            var origin = new Vector(0.0, 0.0);
            var a = new Vector(0.5, 1.0);
            var b = new Vector(0.0, 1.0);
            var c = new Vector(0.0, 2.0);
            var d = new Vector(1.0, 1.0);
            var e = new Vector(1.0, 3.0);
            var f = new Vector(1.5, 0.0);
            var firstEdge = new Edge(a, b);
            var secondEdge = new Edge(a, c);
            var thirdEdge = new Edge(c, d);
            var fourthEdge = new Edge(d, e);
            var fifthEdge = new Edge(d, f);
            var funnel = new FunnelStructure(origin);
            
            funnel.StepOver(firstEdge);
            funnel.StepOver(secondEdge);
            funnel.StepOver(thirdEdge);
            funnel.StepOver(fourthEdge);
            funnel.StepOver(fifthEdge);

            funnel.Apex.List.Count.Should().Be(2);
            funnel.Apex.Value.Should().Be(d);
            funnel.Apex.List.First.Value.Should().Be(f);
            funnel.Apex.List.Last.Value.Should().Be(d);
            funnel.Path.Count.Should().Be(3);
            funnel.Path.First.Value.Should().Be(origin);
            funnel.Path.First.Next.Value.Should().Be(a);
            funnel.Path.First.Next.Next.Value.Should().Be(d);
        }
        
    }
}