using System.Threading.Tasks;
using Aesha.Core;

namespace Aesha.Robots.Actions
{
    public class PetAttack : IConditionalAction
    {
        public bool Evaluate()
        {
            return ObjectManager.Me.Pet != null && ObjectManager.Me.Pet.Target == null;
        }

        public void Do()
        {
            KeyboardCommandDispatcher.GetKeyboard().SendCtrlKey(MappedKeys.PetBar1.Key);
            while (ObjectManager.Me.Target?.Target != ObjectManager.Me.Pet)
            {
                Task.Delay(200).Wait();

                if (ObjectManager.Me.Target == null) return;
                if (ObjectManager.Me.Pet == null) return;
                if (ObjectManager.Me.Pet.Target == null) return;
                if (ObjectManager.Me.Pet.Target != ObjectManager.Me.Target)
                {
                    CommandManager.GetDefault().SetTarget(ObjectManager.Me.Pet.Target);
                    return;
                }
            }
        }
    }
}