namespace de.JochenHeckl.Unity.ACSSandbox
{
	public interface IStateMachine
    {
        public IStateResolver StateResolver { get; }
        IState ActiveState { get; }
        void SwitchToState( IState context );
        void PushState( IState context );
        IState PopState();
    }
}