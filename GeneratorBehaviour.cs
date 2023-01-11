using System.Collections.Generic;
using PoissonProceduralObjectPlacer.Spawn.Collections;
using PoissonProceduralObjectPlacer.Spawn.Surfaces;
using UnityEngine;

namespace PoissonProceduralObjectPlacer
{
    [RequireComponent(typeof(ISpawnCollection))]
    public class GeneratorBehaviour : MonoBehaviour
    {
        public SpawnSurface Surface;
        public int ObjectsCount;

        [Header("Settings")]
        public int RejectionSamplesThreshold = 30;
        public int GetPointTriesThreshold = 7;
        public Vector2 SizeRange = Vector2.one;

        [Header("Debug")]
        public List<int> SpawnCollectionIndexes;
        public List<GameObject> GeneratedObjects;
    }
}