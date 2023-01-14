using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    public class EmptySpawnedObject : MonoBehaviour, ISpawnedObject
    {
        [SerializeField] private float _radius;

        public float Radius => _radius;
        public GameObject ObjectPrefab => null;
        public bool IsEmpty => true;
    }
}