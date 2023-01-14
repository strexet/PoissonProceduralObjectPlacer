using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    public interface ISpawnedObject
    {
        public float Radius {get;}
        public GameObject ObjectPrefab {get;}
        public bool IsEmpty {get;}
    }
}