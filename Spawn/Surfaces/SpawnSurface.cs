using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn.Surfaces
{
    public class SpawnSurface : MonoBehaviour
    {
        [SerializeField] protected Vector2 _surfaceSize;

        public Vector3 Position => transform.position;
        public Vector2 Size => _surfaceSize;
        public Vector3 Size3D => new Vector3(_surfaceSize.x, 0, _surfaceSize.y);

        private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, Size3D);
    }
}