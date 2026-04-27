namespace TP.ConcurrentProgramming.Data
{
    /// <summary>
    /// Two-dimensional immutable vector.
    /// </summary>
    internal record Vector : IVector
    {
        public double x { get; init; }
        public double y { get; init; }

        public Vector(double xComponent, double yComponent)
        {
            x = xComponent;
            y = yComponent;
        }
    }
}
