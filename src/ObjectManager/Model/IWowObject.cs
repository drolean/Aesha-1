using ObjectManager.Infrastructure;

namespace ObjectManager.Model
{
    public interface IWowObject
    {
        ulong Guid { get; }
        uint BaseAddress { get; }
        ObjectType Type { get;}
        string Name { get; }
        Location Location { get; }
        float Rotation { get; }

    }
}