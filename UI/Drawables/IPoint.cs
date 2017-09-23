namespace TriangulatedPolygonAStar.UI
{
    public interface IPoint : IDrawable
    {
        IVector Position { get; }
        double Radius { get; }
        void SetPosition(IVector positionInAbsoluteCoordinateSystem);
    }
}