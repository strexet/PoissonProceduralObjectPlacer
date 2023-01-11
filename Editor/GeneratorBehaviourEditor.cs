using System.Collections.Generic;
using PoissonProceduralObjectPlacer.Extensions;
using PoissonProceduralObjectPlacer.PointsGenerator;
using PoissonProceduralObjectPlacer.PointsGenerator.Poisson;
using PoissonProceduralObjectPlacer.Spawn.Collections;
using UnityEditor;
using UnityEngine;

namespace PoissonProceduralObjectPlacer.Editor
{
    [CustomEditor(typeof(GeneratorBehaviour))]
    public class GeneratorBehaviourEditor : UnityEditor.Editor
    {
        private GeneratorBehaviour _target;

        private ISpawnCollection _spawnCollection;
        private IPointsGenerator _pointsGenerator;

        private void OnEnable()
        {
            _target = target as GeneratorBehaviour;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate!"))
            {
                GeneratePoints();
                GenerateObjects();
            }

            if (GUILayout.Button("Clear!"))
            {
                ClearGeneratedObjects();
                ClearSpawnPoints();
            }

            GUILayout.Space(20f);

            if (GUILayout.Button(nameof(GeneratePoints)))
            {
                GeneratePoints();
            }

            if (GUILayout.Button(nameof(GenerateObjects)))
            {
                GenerateObjects();
            }

            if (GUILayout.Button(nameof(ClearGeneratedObjects)))
            {
                ClearGeneratedObjects();
            }

            if (GUILayout.Button(nameof(ClearSpawnPoints)))
            {
                ClearSpawnPoints();
            }
        }

        public void GeneratePoints()
        {
            ClearSpawnPoints();

            var spawnCollection = GetSpawnCollection();
            var generator = CreateGenerator(spawnCollection);

            bool finished;

            do
            {
                var spawnedObject = spawnCollection.ObjectsToSpawn.GetRandom(out var index);
                var radius = spawnedObject.Radius;

                if (generator.GetNextPointWithRadius(radius, out _, out finished))
                {
                    _target.SpawnCollectionIndexes.Add(index);
                }
            }
            while (!finished);

            _pointsGenerator = generator;
        }

        public void GenerateObjects()
        {
            ClearGeneratedObjects();

            var pointsGenerator = GetPointsGenerator();
            var generatedPoints = pointsGenerator.GeneratedPoints;
            var generatedPointsCount = generatedPoints.Count;
            var spawnPointsCount = _target.SpawnCollectionIndexes.Count;

            var spawnCollection = GetSpawnCollection();

            var length = Mathf.Min(generatedPointsCount, spawnPointsCount);

            for (var i = 0; i < length; i++)
            {
                var spawnIndex = _target.SpawnCollectionIndexes[i];
                var spawnedObject = spawnCollection.ObjectsToSpawn[spawnIndex];

                var point = generatedPoints[i];

                var obj = PrefabUtility.InstantiatePrefab(spawnedObject.ObjectPrefab) as GameObject;
                var objTransform = obj.transform;

                objTransform.SetParent(_target.Surface.transform);
                objTransform.position = GetPointPosition(point.position);
                objTransform.rotation = GetRandomRotationXZ();
                objTransform.localScale = Random.Range(_target.SizeRange.x, _target.SizeRange.y) * Vector3.one;

                _target.GeneratedObjects.Add(obj);
            }

            EditorUtility.SetDirty(_target.Surface);
        }

        public void ClearGeneratedObjects()
        {
            if (_target.GeneratedObjects == null || _target.GeneratedObjects.Count == 0)
            {
                return;
            }

            if (Application.isPlaying)
            {
                foreach (var generatedObject in _target.GeneratedObjects)
                {
                    Destroy(generatedObject);
                }
            }
            else
            {
                foreach (var generatedObject in _target.GeneratedObjects)
                {
                    DestroyImmediate(generatedObject);
                }
            }

            EditorUtility.SetDirty(this);

            _target.GeneratedObjects.Clear();
        }

        public void ClearSpawnPoints()
        {
            _target.SpawnCollectionIndexes ??= new List<int>();
            _target.SpawnCollectionIndexes.Clear();
        }

        private IPointsGenerator GetPointsGenerator()
        {
            if (_pointsGenerator == null)
            {
                var spawnCollection = GetSpawnCollection();
                _pointsGenerator = CreateGenerator(spawnCollection);
            }

            return _pointsGenerator;
        }

        private IPointsGenerator CreateGenerator(ISpawnCollection spawnCollection)
        {
            var (minRadius, maxRadius) = GetRadiusMinMax(spawnCollection);

            return new PoissonDiscSamplingGenerator(_target.ObjectsCount, _target.Surface.Size, _target.Surface.StartSamplingOffset,
                minRadius, maxRadius, _target.RejectionSamplesThreshold, _target.GetPointTriesThreshold);
        }

        private ISpawnCollection GetSpawnCollection() => _spawnCollection ??= _target.GetComponent<ISpawnCollection>();

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

        private Vector3 GetPointPosition(Vector2 point)
        {
            var pointSurfacePosition = _target.Surface.Position + new Vector3(point.x, 0, point.y) - 0.5f * _target.Surface.Size3D;
            var result = pointSurfacePosition;

            const float upDistance = 300f;
            const float rayDistance = upDistance * 2;
            var pointUpPosition = pointSurfacePosition + upDistance * Vector3.up;

            if (Physics.Raycast(pointUpPosition, Vector3.down, out var hitInfo, rayDistance, _target.Surface.LayerMask))
            {
                result = hitInfo.point;
            }

            return result;
        }

        private static Quaternion GetRandomRotationXZ() => Quaternion.AngleAxis(Random.Range(-180f, 180f), Vector3.up);

        private void OnDrawGizmos()
        {
            if (Application.isPlaying ||
                _pointsGenerator == null ||
                _target.GeneratedObjects.Count > 0)
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