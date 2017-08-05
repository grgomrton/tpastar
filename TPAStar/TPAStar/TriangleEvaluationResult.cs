namespace PathFinder.TPAStar
{
    public class TriangleEvaluationResult
    {
        public TriangleEvaluationResult(double h, double fMin, double gMin, double gMax)
        {
            H = h;
            FMin = fMin;
            GMin = gMin;
            GMax = gMax;
        }
        // TODO: add meaningful descriptions
        
        public double H { get; }
        
        public double FMin { get; }
        
        public double GMin { get; }
        
        public double GMax { get; }
    }
}