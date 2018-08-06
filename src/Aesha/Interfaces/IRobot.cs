using Aesha.Domain;

namespace Aesha.Interfaces
{
    public interface IRobot
    {
        void PassiveBehaviour();
        void AttackBehaviour();
        void Tick();

        void SetTarget(WowUnit target);


    }

    public enum RobotState
    {
        Passive,
        Combat,
        Ghost,
        Disconnected
    }
}