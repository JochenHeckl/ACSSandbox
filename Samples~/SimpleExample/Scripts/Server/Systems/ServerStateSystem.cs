namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal class ServerStateSystem : StackStateMachine, IServerSystem
	{
		public ServerStateSystem( IStateResolver stateResolver )
			: base( stateResolver )
		{
		}

		public void Initialize()
		{
			SwitchToState( StateResolver.Resolve<StartupServer>() );
		}

		public void Shutdown()
		{
			SwitchToState( null );
		}
	}
}
