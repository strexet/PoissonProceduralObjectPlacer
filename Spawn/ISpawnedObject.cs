using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    public interface ISpawnedObject
    {
        public GameObject Object {get;}
        public float Radius {get;}
    }
}