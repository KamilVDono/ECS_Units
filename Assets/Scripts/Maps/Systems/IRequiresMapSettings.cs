using Unity.Entities;

namespace Maps.Systems
{
    public interface IRequiresMapSettings
    {
        Entity MapSettingsEntity { set; }
    }
}
