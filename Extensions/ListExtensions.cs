using System.Collections.Generic;

namespace PoissonProceduralObjectPlacer.Extensions
{
    public static class ListExtensions
    {
        public static T GetRandom<T>(this IReadOnlyList<T> list) => list[UnityEngine.Random.Range(0, list.Count)];

        public static T GetRandom<T>(this IReadOnlyList<T> list, out int index)
        {
            index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
    }
}