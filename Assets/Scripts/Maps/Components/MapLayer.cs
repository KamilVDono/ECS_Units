using System;

using Unity.Entities;

namespace Maps.Components
{
    [Serializable]
    public struct MapLayer : ISharedComponentData, IEquatable<MapLayer>
    {
        public MapLayerType Layer;

        public bool Equals( MapLayer other ) => Layer == other.Layer;
        public override int GetHashCode() => (int)Layer;
    }


    public enum MapLayerType
    {
        Tile = 1,
        Resource = 2,
    }
}
