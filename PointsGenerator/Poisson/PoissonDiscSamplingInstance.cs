using System.Collections.Generic;
using UnityEngine;

namespace PoissonProceduralObjectPlacer.PointsGenerator.Poisson
{
    public class PoissonDiscSamplingGenerator : IPointsGenerator
    {
        private readonly int _count;
        private readonly float _minRadius;
        private readonly float _maxRadius;

        private readonly int _numSamplesBeforeRejection;
        private readonly int _getPointTriesThreshold;

        private readonly List<Point> _points;
        private readonly List<Point> _spawnPoints;

        private readonly int[,] _grid;

        private readonly CandidateStaticData _candidateStaticData;

        private int _numberOfTries;

        public IReadOnlyList<Point> GeneratedPoints => _points;
        public int GeneratedCount {get; private set;}

        private bool CanGetNextPoint => GeneratedCount < _count && _spawnPoints.Count > 0;

        public PoissonDiscSamplingGenerator(int count,
            Vector2 sampleRegionSize, Vector2 startSamplingOffset,
            float minRadius, float maxRadius,
            int numSamplesBeforeRejection, int getPointTriesThreshold)
        {
            _count = count;

            _maxRadius = maxRadius;
            _minRadius = minRadius;

            _numSamplesBeforeRejection = numSamplesBeforeRejection;
            _getPointTriesThreshold = getPointTriesThreshold;

            _points = new List<Point>();
            _spawnPoints = new List<Point>();

            var startPosition = 0.5f * sampleRegionSize + startSamplingOffset; // mi≠ddle position
            var startPoint = new Point(startPosition, minRadius);
            _spawnPoints.Add(startPoint);

            var minRadiusInverted = 1 / minRadius;
            var cellSizeInverted = Consts.SquareRootOfTwoInverted * minRadiusInverted;

            var gridSize = sampleRegionSize * cellSizeInverted;
            _grid = new int[Mathf.CeilToInt(gridSize.x), Mathf.CeilToInt(gridSize.y)];

            _candidateStaticData = new CandidateStaticData
            {
                sampleRegionSize = sampleRegionSize,
                maxRadius = maxRadius,
                minRadiusInverted = minRadiusInverted,
                cellSizeInverted = cellSizeInverted,
                points = _points,
                grid = _grid
            };

            GeneratedCount = 0;
            _numberOfTries = 0;
        }

        public bool GetNextPointWithRadius(float radius, out Point point, out bool finished)
        {
            if (TrySpawnNextPoint(out point, radius))
            {
                finished = !CanGetNextPoint;
                return true;
            }

            _numberOfTries++;

            finished = !CanGetNextPoint;
            point = default;

            return false;
        }

        private bool TrySpawnNextPoint(out Point point, float radius)
        {
            if (!CanGetNextPoint)
            {
                point = default;
                return false;
            }

            var spawnIndex = Random.Range(0, _spawnPoints.Count);
            var spawnCentre = _spawnPoints[spawnIndex].position;

            for (var i = 0; i < _numSamplesBeforeRejection; i++)
            {
                if (!GenerateValidCandidate(out var candidate, spawnCentre, radius))
                {
                    continue;
                }

                point = candidate.Point;

                _points.Add(point);
                _spawnPoints.Add(point);

                _grid[candidate.GridPositionX, candidate.GridPositionY] = _points.Count;
                GeneratedCount++;

                return true;
            }

            if (_numberOfTries > _getPointTriesThreshold)
            {
                _spawnPoints.RemoveAt(spawnIndex);
                _numberOfTries = 0;
            }

            point = default;

            return false;
        }

        private bool GenerateValidCandidate(out Candidate candidate, Vector2 spawnCentre, float radius)
        {
            candidate = CreateCandidate(spawnCentre, radius);
            return candidate.IsValid();
        }

        private Candidate CreateCandidate(Vector2 spawnCentre, float radius)
        {
            var angle = 2 * Mathf.PI * Random.value;
            var direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            var candidatePosition = spawnCentre + Random.Range(radius + _minRadius, 2 * (radius + _maxRadius)) * direction;
            var point = new Point(candidatePosition, radius);
            var candidate = new Candidate(point, _candidateStaticData);

            return candidate;
        }
    }
}