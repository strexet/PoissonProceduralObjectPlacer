using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn.Surfaces
{
    public class SpawnPlane : SpawnSurface
    {
        private void OnValidate()
        {
            var scale = _surfaceSize / 10;
            transform.localScale = new Vector3(scale.x, scale.x, scale.y);
        }
    }
}