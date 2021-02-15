using System;

namespace de.JochenHeckl.Unity.ACSSandbox
{
    public interface IStateResolver
    {
        IState Resolve<StateType>();
    }
}