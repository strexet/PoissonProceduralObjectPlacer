using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn.Surfaces
{
    public class SpawnSurface : MonoBehaviour
    {
        [SerializeField] protected Vector2 _surfaceSize;
        [SerializeField] protected Vector2 _startSamplingOffset;
        [SerializeField] private LayerMask _surfaceLayerMask;

        public Vector3 Position => transform.position;
        public Vector2 Size => _surfaceSize;
        public Vector2 StartSamplingOffset => _startSamplingOffset;
        public Vector3 Size3D => new(_surfaceSize.x, 0, _surfaceSize.y);
        public LayerMask LayerMask => _surfaceLayerMask;

        private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, Size3D);
    }
}