using System.Collections.Generic;

namespace de.JochenHeckl.Unity.ACSSandbox
{
	public class StackStateMachine : IStateMachine
	{
		private readonly Stack<IState> stateStack = new Stack<IState>();

		public IState ActiveState => stateStack.Count > 0 ? stateStack.Peek() : null;
		public IStateResolver StateResolver { get; }

		public StackStateMachine( IStateResolver stateResolver )
		{
			StateResolver = stateResolver;
		}

		public void SwitchToState( IState newState )
		{
			if ( stateStack.Count > 0 )
			{
				var currentState = stateStack.Pop();

				currentState.DeactivateState( this );
				currentState.LeaveState( this );
			}

			newState?.EnterState( this );
			newState?.ActivateState( this );

			if ( newState != null )
			{
				stateStack.Push( newState );
			}
		}

		public void PushState( IState state )
		{
			ActiveState?.DeactivateState( this );

			stateStack.Push( state );
			state.EnterState( this );

			ActiveState?.ActivateState( this );
		}

		public IState PopState()
		{
			ActiveState?.DeactivateState( this );

			var state = stateStack.Pop();
			state.LeaveState( this );

			ActiveState?.ActivateState( this );

			return state;
		}

		public void Update( float deltaTimeSec )
		{
			ActiveState?.UpdateState( this, deltaTimeSec );
		}
	}
}
