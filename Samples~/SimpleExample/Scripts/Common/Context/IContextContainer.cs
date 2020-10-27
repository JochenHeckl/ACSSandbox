using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
    public interface IContextContainer
    {
        IContext ActiveContext { get; }

        void SwitchToContext( IContext context );
        void PushContext( IContext context );
        IContext PopContext();
    }
}