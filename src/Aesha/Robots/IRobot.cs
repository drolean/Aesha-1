using Aesha.Domain;
using FluentBehaviourTree;

namespace Aesha.Robots
{
    public interface IRobot
    {
        IBehaviourTreeNode PassiveBehaviour { get; }
        IBehaviourTreeNode AttackBehaviour { get; }
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