using Aesha.Domain;

namespace Aesha.Interfaces
{
    public interface IRobot
    {
        void PassiveBehaviour();
        void AttackBehaviour();
        void Tick(RobotState state);

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