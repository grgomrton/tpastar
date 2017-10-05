namespace TriangulatedPolygonAStar
{
    /// <summary>
    /// Contains the result of an evaluation of a triangle during traversing the triangle graph.
    /// </summary>
    public class TriangleEvaluationResult
    {
        
        /// <summary>
        /// Initializes a new instance of the <see cref="TriangleEvaluationResult"/> class
        /// which contains the result of a triangle evaluation during exploring the triangle graph.
        /// </summary>
        /// <param name="path"></param>
        public TriangleEvaluationResult(TPAPath path)
        {
            EstimatedMinimalCost = path.MinimalTotalCost;
            ShortestPathToEdgeLength = path.ShortestPathToEdgeLength;
            LongestPathToEdgeLength = path.LongestPathToEdgeLength;
        }  

        /// <summary>
        /// The length of the possibly shortest path from the start to the closest goal point along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double EstimatedMinimalCost { get; private set; }
        
        /// <summary>
        /// The length of the possibly shortest path from the start to the current edge along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double ShortestPathToEdgeLength { get; private set; }
        
        /// <summary>
        /// The length of the longest possible path from the start to the current edge along the set of triangles
        /// stepped over during building the path.
        /// </summary>
        public double LongestPathToEdgeLength { get; private set; }
    }
}