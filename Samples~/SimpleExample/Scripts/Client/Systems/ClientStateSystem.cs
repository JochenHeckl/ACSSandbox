namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	internal class ClientStateSystem : StackStateMachine, IClientSystem
	{
		public ClientStateSystem( IStateResolver stateResolver ) 
			: base( stateResolver )
		{
		}

		public void Initialize()
		{
			PushState( StateResolver.Resolve<StartupClient>() );
		}

		public void Shutdown()
		{
			SwitchToState( null );
		}
	}
}
