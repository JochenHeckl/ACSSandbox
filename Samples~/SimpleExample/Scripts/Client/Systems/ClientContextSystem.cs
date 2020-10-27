using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class ClientContextSystem : IClientSystem, IContextContainer
	{
		private readonly IContextResolver contextResolver;
		private readonly Stack<IContext> contextStack = new Stack<IContext>();

		public IContext ActiveContext { get { return contextStack.Peek(); } }

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
