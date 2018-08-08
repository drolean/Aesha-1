using System.Linq;
using Aesha.Core;
using Aesha.Domain;
using Aesha.Infrastructure;

namespace Aesha.Robots.Actions
{
    public class AcquireTarget : IConditionalAction
    {
        public bool Evaluate()
        {
            return ObjectManager.Me.Target == null;
            //TODO: better target available?
        }

        public void Do()
        {
            var enemiesTargetingMe = ObjectManager.Units.Where(u =>
                    u.Target == ObjectManager.Me
                    || (ObjectManager.Me.Pet != null && u.Target == ObjectManager.Me.Pet))
                .OrderBy(u => u.Distance).ToList();

            if (enemiesTargetingMe.Count > 0)
            {
                SetTarget(enemiesTargetingMe.First());
                return;
            }
            
            var enemies = ObjectManager.Units.Where(u =>
                    u.Attributes.Tapped == false
                    && u.Health.Current > 0
                    && u.SummonedBy == null
                    && u.CreatureType != CreatureType.Critter
                    && u.Distance < 1000)
                .OrderBy(u => u.Distance).ToList();

            if (enemies.Count > 0)
                SetTarget(enemies.First());

        }

        private void SetTarget(WowUnit unit)
        {
            CommandManager.GetDefault().SetTarget(unit);
        }

    }
}
