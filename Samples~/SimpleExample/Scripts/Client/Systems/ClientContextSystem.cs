using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class ClientContextSystem : IClientSystem, IStateMachine
	{
		private readonly Stack<IState> contextStack = new Stack<IState>();

		public IState ActiveState { get { return contextStack.Count > 0  ? contextStack.Peek() : null; } }
		public IStateResolver StateResolver { get; private set; }
		public ClientContextSystem( IStateResolver contextResolverIn )
		{
			StateResolver = contextResolverIn;
		}

		public void Initialize()
		{
			PushState( StateResolver.Resolve<StartupClient>() );
		}

		public void Shutdown()
		{
			while( contextStack.Count > 0 )
			{
				contextStack.Pop().LeaveState( this );
			}
		}

		public void Update( float deltaTimeSec )
		{
			ActiveState?.UpdateState( this, deltaTimeSec );
		}

		public void SwitchToState( IState newContext )
		{
			if( newContext == null )
			{
				throw new ArgumentNullException( nameof( newContext ) );
			}

			if ( contextStack.Count > 0 )
			{
				var currentContext = contextStack.Pop();

				currentContext.DeactivateState( this );
				currentContext.LeaveState( this );
			}

			newContext?.EnterState( this );
			newContext?.ActivateState( this );

			contextStack.Push( newContext );
		}

		public void PushState( IState context )
		{
			ActiveState?.DeactivateState( this );

			contextStack.Push( context );
			context.EnterState( this );

			ActiveState?.ActivateState( this );
		}

		public IState PopState()
		{
			ActiveState?.DeactivateState( this );

			var context = contextStack.Pop();
			context.LeaveState( this );

			ActiveState?.ActivateState( this );

			return context;
		}
	}
}
