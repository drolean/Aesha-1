using System.Collections;

namespace Aesha.Infrastructure
{
    public interface IHandleCommand<in TCommand>
    {
        void Handle(TCommand c);
    }
}
