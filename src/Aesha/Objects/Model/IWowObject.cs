using Aesha.Infrastructure;

namespace Aesha.Objects.Model
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