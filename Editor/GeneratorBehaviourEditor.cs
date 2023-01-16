using PoissonProceduralObjectPlacer.Extensions;
using PoissonProceduralObjectPlacer.PointsGenerator;
using PoissonProceduralObjectPlacer.PointsGenerator.Poisson;
using PoissonProceduralObjectPlacer.Spawn;
using PoissonProceduralObjectPlacer.Spawn.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

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
            Undo.RecordObject(_target, nameof(GeneratePoints));
            
            ClearSpawnPoints();

            var spawnCollection = GetSpawnCollection();
            var generator = CreateGenerator(spawnCollection);
            var objectsToSpawn = spawnCollection.ObjectsToSpawn;
            
            bool finished;

            do
            {
                
                var spawnedObject = objectsToSpawn.GetRandom(out var index);
                var radius = spawnedObject.Radius;

                if (generator.GetNextPointWithRadius(radius, out _, out finished))
                {
                    _target.SpawnCollectionIndexes.Add(index);
                }
            }
            while (!finished);
            
            _pointsGenerator = generator;
            EditorUtility.SetDirty(_target);
        }

        public void GenerateObjects()
        {
            Undo.RecordObject(_target.GeneratedObjects, nameof(GenerateObjects));
            
            ClearGeneratedObjects();

            var pointsGenerator = GetPointsGenerator();
            var generatedPoints = pointsGenerator.GeneratedPoints;
            var generatedPointsCount = generatedPoints.Count;
            var spawnPointsCount = _target.SpawnCollectionIndexes.Count;

            var spawnCollection = GetSpawnCollection();
            var objectsToSpawn = spawnCollection.ObjectsToSpawn;

            var length = Mathf.Min(generatedPointsCount, spawnPointsCount);

            for (var i = 0; i < length; i++)
            {
                var spawnIndex = _target.SpawnCollectionIndexes[i];
                var spawnedObject = objectsToSpawn[spawnIndex];

                

                if (spawnedObject.IsEmpty)
                {
                    continue;
                }

                if (spawnedObject.ObjectPrefab == null)
                {
                    Debug.LogError($"[ERROR]<color=red>{nameof(GeneratorBehaviourEditor)}.{nameof(GenerateObjects)}></color> " + 
                                   "No prefab reference!", _target);
                    continue;
                }

                var obj = PrefabUtility.InstantiatePrefab(spawnedObject.ObjectPrefab) as GameObject;
                var point = generatedPoints[i];
                
                var objTransform = obj.transform;
                objTransform.SetParent(_target.Surface.transform);

                objTransform.position = GetPointPosition(point.position);
                objTransform.rotation = GetRandomRotationXZ();
                objTransform.localScale = Random.Range(_target.SizeRange.x, _target.SizeRange.y) * Vector3.one;

                _target.GeneratedObjects.List.Add(obj);
            }

            EditorUtility.SetDirty(_target.GeneratedObjects);
        }

        public void ClearGeneratedObjects()
        {
            if (_target.GeneratedObjects == null || _target.GeneratedObjects.List.Count == 0)
            {
                return;
            }
            
            Undo.RecordObject(_target.GeneratedObjects, nameof(ClearGeneratedObjects));

            if (Application.isPlaying)
            {
                foreach (var generatedObject in _target.GeneratedObjects.List)
                {
                    Destroy(generatedObject);
                }
            }
            else
            {
                foreach (var generatedObject in _target.GeneratedObjects.List)
                {
                    DestroyImmediate(generatedObject);
                }
            }

            _target.GeneratedObjects.List.Clear();
            EditorUtility.SetDirty(_target.GeneratedObjects);
        }

        public void ClearSpawnPoints()
        {
            if (_target.SpawnCollectionIndexes == null || _target.SpawnCollectionIndexes.Count == 0)
            {
                return;
            }
            
            Undo.RecordObject(_target, nameof(ClearSpawnPoints));
            
            _target.SpawnCollectionIndexes.Clear();
            EditorUtility.SetDirty(_target);
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
            var objectsToSpawn = spawnCollection.ObjectsToSpawn;
            var minRadius = float.MaxValue;
            float maxRadius = 0;

            foreach (var objectToSpawn in objectsToSpawn)
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
                _target.GeneratedObjects.List.Count > 0)
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