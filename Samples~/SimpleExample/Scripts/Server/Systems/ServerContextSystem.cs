using System;
using System.Collections.Generic;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class ServerContextSystem : IServerSystem, IStateMachine
	{
		
		private readonly Stack<IState> contextStack = new Stack<IState>();

		public IState ActiveState { get { return contextStack.Peek(); } }
		public IStateResolver StateResolver { get; private set; }

		public ServerContextSystem( IStateResolver contextResolverIn )
		{
			StateResolver = contextResolverIn;
		}

		public void Initialize()
		{
			SwitchToState( StateResolver.Resolve<StartupServer>() );
		}

		public void Shutdown()
		{
			SwitchToState( null );
		}

		public void Update( float deltaTimeSec )
		{
			ActiveState?.UpdateState( this, deltaTimeSec );
		}

		public void SwitchToState( IState newContext )
		{
			while ( contextStack.Count > 0 )
			{
				var currentContext = contextStack.Pop();
				currentContext.LeaveState( this );
			}

			newContext?.EnterState( this );
			contextStack.Push( newContext );
		}

		public void PushState( IState context )
		{
			contextStack.Push( context );
			context.EnterState( this );
		}

		public IState PopState()
		{
			var context = contextStack.Pop();
			context.LeaveState( this );

			return context;
		}
	}
}
