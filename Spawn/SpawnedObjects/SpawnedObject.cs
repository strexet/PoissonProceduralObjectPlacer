using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    [SelectionBase]
    public class SpawnedObject : MonoBehaviour, ISpawnedObject
    {
        [SerializeField] private float _radius;
        [SerializeField] private GameObject _prefab;

        public float Radius => _radius;
        public GameObject ObjectPrefab => _prefab;
        public bool IsEmpty => false;

        private void OnDrawGizmos()
        {
            var color = Gizmos.color;
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _radius);
            Gizmos.color = color;
        }
    }
}