using System.Linq;
using Aesha.Core;
using Aesha.Infrastructure;

namespace Aesha.Robots.Actions
{
    public class TargetManager
    {
        public void UpdateTarget()
        {
            var enemiesTargetingMe = ObjectManager.Units.Where(u =>
                    u.Target == ObjectManager.Me
                    || (ObjectManager.Me.Pet != null && u.Target == ObjectManager.Me.Pet)
                    || (ObjectManager.Me.Pet != null && ObjectManager.Me.Pet.Target == u))
                .OrderBy(u => u.Distance).ToList();

            if (enemiesTargetingMe.Count > 0)
            {
                CommandManager.GetDefault().SetTarget(enemiesTargetingMe.First());
                return;
            }
            
            var enemies = ObjectManager.Units.Where(u =>
                    u.Attributes.Tapped == false
                    && u.Health.Current > 0
                    && u.SummonedBy == null
                    && u.CreatureType != CreatureType.Critter
                    && u.Distance < 900)
                .OrderBy(u => u.Distance).ToList();

            if (enemies.Count > 0)
                CommandManager.GetDefault().SetTarget(enemies.First());

            if (ObjectManager.Me.Target == ObjectManager.Me.Pet)
                CommandManager.GetDefault().ClearTarget();

            if (ObjectManager.Me.Target != null && ObjectManager.Me.Target.Health.Current == 0)
                CommandManager.GetDefault().ClearTarget();


        }

 
    }
}
