using Aesha.Core;

namespace Aesha.Domain
{
    public class Spell
    {

        public Spell(int id, string name, int rank, MappedKeyAction keyAction, int castTime = 0)
        {
            Id = id;
            Name = name;
            Rank = rank;
            KeyAction = keyAction;
            CastTime = castTime;
        }

        public int Id { get; }

        public string Name { get; }

        public int Rank { get; }
        public MappedKeyAction KeyAction { get; }
        public int CastTime { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj.GetType() == typeof(Spell) && Id.Equals(((Spell) obj).Id);
        }

        protected bool Equals(Spell other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return $"[{Id}] {Name} (Rank {Rank})";
        }
    }
}
