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
        public int ObjectsCount = 300;

        [Header("Settings")]
        public int RejectionSamplesThreshold = 30;
        public int GetPointTriesThreshold = 7;
        public Vector2 SizeRange = new Vector2(0.95f, 1.05f);

        [Header("Generated")]
        public List<int> SpawnCollectionIndexes;
        public GeneratedObjectsCollection GeneratedObjects;
    }
}