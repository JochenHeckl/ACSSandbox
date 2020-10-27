using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerContextSystem : IServerSystem, IContextContainer
	{
		private readonly IContextResolver contextResolver;
		private readonly Stack<IContext> contextStack = new Stack<IContext>();

		public IContext ActiveContext { get { return contextStack.Peek(); } }

		public ServerContextSystem( IContextResolver contextResolverIn )
		{
			contextResolver = contextResolverIn;
		}

		public void Initialize()
		{
			SwitchToContext( contextResolver.Resolve<StartupServer>() );
		}

		public void Shutdown()
		{
			SwitchToContext( null );
		}

		public void Update( float deltaTimeSec )
		{
			ActiveContext?.Update( this, deltaTimeSec );
		}

		public void SwitchToContext( IContext newContext )
		{
			while ( contextStack.Count > 0 )
			{
				var currentContext = contextStack.Pop();
				currentContext.LeaveContext( this );
			}

			newContext?.EnterContext( this );
			contextStack.Push( newContext );
		}

		public void PushContext( IContext context )
		{
			contextStack.Push( context );
			context.EnterContext( this );
		}

		public IContext PopContext()
		{
			var context = contextStack.Pop();
			context.LeaveContext( this );

			return context;
		}
	}
}
