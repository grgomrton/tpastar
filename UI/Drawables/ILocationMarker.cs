namespace TriangulatedPolygonAStar.UI
{
    public interface ILocationMarker : IDrawable
    {
        /// <summary>
        /// The position where this marker is currently put.
        /// </summary>
        IVector CurrentLocation { get; }
        
        /// <summary>
        /// Sets the position of this marker.
        /// </summary>
        /// <param name="position">The position to move this marker to</param>
        void SetLocation(IVector position);

        /// <summary>
        /// Determines, whether the specified position is under the marker sign.
        /// </summary>
        /// <param name="position">The position in the same coordinate-system as the location</param>
        /// <returns>true if the specified position falls under the marker, otherwise false</returns>
        bool IsPositionUnderMarker(IVector position);
    }
}