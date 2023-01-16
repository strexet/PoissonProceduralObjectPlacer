using UnityEngine;
using UsefulTools.Runtime.DataStructures.InterfaceImplementations;

namespace PoissonProceduralObjectPlacer.Spawn
{
    public class EmptySpawnedObject : MonoBehaviourImplementation<ISpawnedObject>, ISpawnedObject
    {
        [SerializeField] private float _radius;

        public float Radius => _radius;
        public GameObject ObjectPrefab => null;
        public bool IsEmpty => true;
    }
}