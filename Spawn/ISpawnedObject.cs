using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    public interface ISpawnedObject
    {
        public GameObject ObjectPrefab {get;}
        public float Radius {get;}
    }
}