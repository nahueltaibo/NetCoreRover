using System.Numerics;

namespace Robot.Controllers.Tests.Infrastructure
{
    public class MapCoordinates
    {
        public Vector3 Position { get; set; }
        public Vector3 Speed { get; set; }

        public override string ToString()
        {
            return $"<{Position.X:F2}, {Position.Y:F2}, {Position.Z:F2}> : <{Speed.X:F2}, {Speed.Y:F2}, {Speed.Z:F2}>";
        }
    }
}