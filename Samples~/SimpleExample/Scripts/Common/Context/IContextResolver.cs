using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Common
{
    public interface IContextResolver
    {
        IContext Resolve<ContextType>();
        IContext Resolve( Type contextType );
    }
}