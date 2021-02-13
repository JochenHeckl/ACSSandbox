using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class ClientContextSystem : IClientSystem, IContextContainer
	{
		private readonly IContextResolver contextResolver;
		private readonly Stack<IContext> contextStack = new Stack<IContext>();

		public IContext ActiveContext { get { return contextStack.Count > 0  ? contextStack.Peek() : null; } }

		public ClientContextSystem( IContextResolver contextResolverIn )
		{
			contextResolver = contextResolverIn;
		}

		public void Initialize()
		{
			PushContext( contextResolver.Resolve<StartupClient>() );
		}

		public void Shutdown()
		{
			while( contextStack.Count > 0 )
			{
				contextStack.Pop().LeaveContext( this );
			}
		}

		public void Update( float deltaTimeSec )
		{
			ActiveContext?.Update( this, deltaTimeSec );
		}

		public void SwitchToContext( IContext newContext )
		{
			if( newContext == null )
			{
				throw new ArgumentNullException( nameof( newContext ) );
			}

			if ( contextStack.Count > 0 )
			{
				var currentContext = contextStack.Pop();

				currentContext.DeactivateContext( this );
				currentContext.LeaveContext( this );
			}

			newContext?.EnterContext( this );
			newContext?.ActivateContext( this );

			contextStack.Push( newContext );
		}

		public void PushContext( IContext context )
		{
			ActiveContext?.DeactivateContext( this );

			contextStack.Push( context );
			context.EnterContext( this );

			ActiveContext?.ActivateContext( this );
		}

		public IContext PopContext()
		{
			ActiveContext?.DeactivateContext( this );

			var context = contextStack.Pop();
			context.LeaveContext( this );

			ActiveContext?.ActivateContext( this );

			return context;
		}

		public IContext Resolve<ContextType>()
		{
			return contextResolver.Resolve<ContextType>();
		}

		public IContext Resolve( Type contextType )
		{
			return contextResolver.Resolve( contextType );
		}
	}
}
