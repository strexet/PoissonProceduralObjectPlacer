using UnityEngine;

namespace PoissonProceduralObjectPlacer.PointsGenerator.Poisson
{
    public readonly struct Candidate
    {
        private readonly Point _point;
        private readonly CandidateStaticData _staticData;

        public Point Point => _point;

        public int GridPositionX => (int)(_point.position.x * _staticData.cellSizeInverted);
        public int GridPositionY => (int)(_point.position.y * _staticData.cellSizeInverted);

        public Candidate(Point point, CandidateStaticData staticData)
        {
            _point = point;
            _staticData = staticData;
        }

        public bool IsValid() => IsInsideSampleRegion() && HasNoClosePointsInBlock();

        private bool IsInsideSampleRegion() =>
            _point.position.x - _point.radius >= 0 &&
            _point.position.x + _point.radius < _staticData.sampleRegionSize.x &&
            _point.position.y - _point.radius >= 0 &&
            _point.position.y + _point.radius < _staticData.sampleRegionSize.y;

        private bool HasNoClosePointsInBlock()
        {
            var halfBlockStep = Mathf.CeilToInt(
                Consts.SquareRootOfTwoInverted * _staticData.minRadiusInverted * (_point.radius + _staticData.maxRadius)
            );

            var cellX = GridPositionX;
            var searchStartX = Mathf.Max(0, cellX - halfBlockStep);
            var searchEndX = Mathf.Min(cellX + halfBlockStep, _staticData.grid.GetLength(0) - 1);

            var cellY = GridPositionY;
            var searchStartY = Mathf.Max(0, cellY - halfBlockStep);
            var searchEndY = Mathf.Min(cellY + halfBlockStep, _staticData.grid.GetLength(1) - 1);

            for (var y = searchStartY; y <= searchEndY; y++)
            {
                for (var x = searchStartX; x <= searchEndX; x++)
                {
                    var pointIndex = _staticData.grid[x, y] - 1;
                    var pointExists = pointIndex != -1;

                    if (pointExists)
                    {
                        var pointInGrid = _staticData.points[pointIndex];

                        var distanceSqr = (_point.position - pointInGrid.position).sqrMagnitude;
                        var distanceThreshold = _point.radius + pointInGrid.radius;

                        if (distanceSqr < distanceThreshold * distanceThreshold)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}