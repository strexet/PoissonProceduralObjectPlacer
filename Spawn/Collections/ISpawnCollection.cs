using System.Collections.Generic;

namespace PoissonProceduralObjectPlacer.Spawn.Collections
{
    public interface ISpawnCollection
    {
        IReadOnlyList<ISpawnedObject> ObjectsToSpawn {get;}
    }
}