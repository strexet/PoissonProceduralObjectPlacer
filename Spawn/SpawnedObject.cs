using UnityEngine;

namespace PoissonProceduralObjectPlacer.Spawn
{
    [SelectionBase]
    public class SpawnedObject : MonoBehaviour, ISpawnedObject
    {
        [SerializeField] private float _radius;
        [SerializeField] private GameObject _prefab;

        public GameObject ObjectPrefab => _prefab;

        public float Radius => _radius;

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position, _radius);
    }
}