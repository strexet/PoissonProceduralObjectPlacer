using System.Collections.Generic;
using PoissonProceduralObjectPlacer.Extensions;
using PoissonProceduralObjectPlacer.PointsGenerator;
using PoissonProceduralObjectPlacer.PointsGenerator.Poisson;
using PoissonProceduralObjectPlacer.Spawn.Collections;
using PoissonProceduralObjectPlacer.Spawn.Surfaces;
using UnityEngine;

namespace PoissonProceduralObjectPlacer
{
    public class GeneratorBehaviour : MonoBehaviour
    {
        [SerializeField] private SpawnSurface _surface;
        [SerializeField] private int _objectsCount;

        [Header("Settings")]
        [SerializeField] private int _rejectionSamplesThreshold = 30;
        [SerializeField] private int _getPointTriesThreshold = 7;

        [Header("Debug")]
        [SerializeField] private bool _drawGizmos;

        [Space]
        [SerializeField] private int _generatedObjectsCount;
        [SerializeField] private List<int> _spawnCollectionIndexes;
        [SerializeField] private List<GameObject> _generatedObjects;

        private ISpawnCollection _spawnCollection;
        private IPointsGenerator _pointsGenerator;

        private bool _needRedraw;

        [ContextMenu(nameof(CreatePoints))]
        private void CreatePoints()
        {
            _spawnCollection ??= GetComponent<ISpawnCollection>();
            _spawnCollectionIndexes ??= new List<int>();
            Init();
        }

        private void Awake()
        {
            _spawnCollection = GetComponent<ISpawnCollection>();
            _spawnCollectionIndexes = new List<int>();
            _generatedObjects = new List<GameObject>();
            Init();
        }

        private void Update()
        {
            if (_needRedraw)
            {
                Redraw();
            }
        }

        private void Init()
        {
            var (minRadius, maxRadius) = GetRadiusMinMax(_spawnCollection);

            _pointsGenerator = new PoissonDiscSamplingGenerator(_objectsCount, _surface.Size, minRadius, maxRadius,
                _rejectionSamplesThreshold,
                _getPointTriesThreshold);

            GeneratePoints(_pointsGenerator, _spawnCollection);

            _generatedObjectsCount = _spawnCollectionIndexes.Count;
            _needRedraw = true;
        }

        private void GeneratePoints(IPointsGenerator generator, ISpawnCollection spawnCollection)
        {
            _spawnCollectionIndexes.Clear();
            _generatedObjectsCount = 0;

            bool finished;

            do
            {
                var spawnedObject = spawnCollection.ObjectsToSpawn.GetRandom(out var index);
                var radius = spawnedObject.Radius;

                if (generator.GetNextPointWithRadius(radius, out _, out finished))
                {
                    _spawnCollectionIndexes.Add(index);
                }
            }
            while (!finished);
        }

        [ContextMenu(nameof(Redraw))]
        private void Redraw()
        {
            ClearGeneratedObjects();

            for (var i = 0; i < _pointsGenerator.GeneratedPoints.Count; i++)
            {
                var spawnIndex = _spawnCollectionIndexes[i];
                var spawnedObject = _spawnCollection.ObjectsToSpawn[spawnIndex];

                var point = _pointsGenerator.GeneratedPoints[i];
                var obj = Instantiate(spawnedObject.Object, GetPointPosition(point.position), GetRandomRotationXZ());

                _generatedObjects.Add(obj);
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif

            _needRedraw = false;
        }

        private static (float minRadius, float maxRadius) GetRadiusMinMax(ISpawnCollection spawnCollection)
        {
            var minRadius = float.MaxValue;
            float maxRadius = 0;

            foreach (var objectToSpawn in spawnCollection.ObjectsToSpawn)
            {
                var radius = objectToSpawn.Radius;

                if (radius < minRadius)
                {
                    minRadius = radius;
                }

                if (radius > maxRadius)
                {
                    maxRadius = radius;
                }
            }

            return (minRadius, maxRadius);
        }

        [ContextMenu(nameof(ClearGeneratedObjects))]
        private void ClearGeneratedObjects()
        {
            if (_generatedObjects == null || _generatedObjects.Count == 0)
            {
                return;
            }

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                foreach (var generatedObject in _generatedObjects)
                {
                    Destroy(generatedObject);
                }
            }
            else
            {
                foreach (var generatedObject in _generatedObjects)
                {
                    DestroyImmediate(generatedObject);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
#else
            foreach (var generatedObject in _generatedObjects)
            {
                Destroy(generatedObject);
            }

#endif

            _generatedObjects.Clear();
        }

        private Vector3 GetPointPosition(Vector2 point) =>
            transform.position + new Vector3(point.x, 0, point.y) - 0.5f * _surface.Size3D;

        private static Quaternion GetRandomRotationXZ() => Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.up);

        private void OnDrawGizmos()
        {
            if (!_drawGizmos)
            {
                return;
            }

            if (Application.isPlaying || _pointsGenerator == null)
            {
                return;
            }

            foreach (var point in _pointsGenerator.GeneratedPoints)
            {
                Gizmos.DrawSphere(GetPointPosition(point.position), point.radius);
            }
        }
    }
}