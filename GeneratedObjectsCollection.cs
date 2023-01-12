using System.Collections.Generic;
using UnityEngine;

namespace PoissonProceduralObjectPlacer
{
    public class GeneratedObjectsCollection : MonoBehaviour
    {
        public List<GameObject> List;

        public void UpdatePositions(LayerMask surfaceLayerMask)
        {
            if (List == null || List.Count == 0)
            {
                return;
            }
            
            foreach (var generatedObject in List)
            {
                if (generatedObject == null)
                {
                    continue;
                }
                
                generatedObject.transform.position = GetNewPosition(generatedObject.transform.position, surfaceLayerMask);
            }
        }

        private Vector3 GetNewPosition(Vector3 oldPosition, LayerMask surfaceLayerMask)
        {
            const float upDistance = 300f;
            const float rayDistance = upDistance * 2;

            var result = oldPosition;
            var upPosition = oldPosition + upDistance * Vector3.up;

            if (Physics.Raycast(upPosition, Vector3.down, out var hitInfo, rayDistance, surfaceLayerMask))
            {
                result = hitInfo.point;
            }

            return result;
        }
    }
}