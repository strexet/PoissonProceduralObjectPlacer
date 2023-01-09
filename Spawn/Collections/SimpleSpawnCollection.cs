using System.Collections.Generic;
using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn.Collections
{
    public class SimpleSpawnCollection : MonoBehaviour, ISpawnCollection
    {
        [SerializeField] private List<SpawnedObject> _objectsToSpawn;

        public IReadOnlyList<ISpawnedObject> ObjectsToSpawn => _objectsToSpawn;
    }
}