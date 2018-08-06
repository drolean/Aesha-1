using System;

namespace Aesha.Robots.Actions
{
    public interface IConditionalAction
    {
        bool Evaluate();
        void Do();
    }
}