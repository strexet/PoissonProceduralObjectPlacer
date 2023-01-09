using UnityEngine;

namespace PoissonProceduralObjectPlacer
{
    public readonly struct Point
    {
        public readonly Vector2 position;
        public readonly float radius;

        public Point(Vector2 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }
    }
}