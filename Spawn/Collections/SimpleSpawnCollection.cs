using System.Collections.Generic;
using UnityEngine;
using UsefulTools.Runtime.DataStructures.InterfaceImplementations;

namespace PoissonProceduralObjectPlacer.Spawn.Collections
{
    public class SimpleSpawnCollection : MonoBehaviour, ISpawnCollection
    {
        [SerializeField] private MonoBehaviourImplementationList<ISpawnedObject> _objectsToSpawn;

        public IReadOnlyList<ISpawnedObject> ObjectsToSpawn => _objectsToSpawn.ToImplementationList();
    }
}