namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	internal interface IServerSystem
	{
		void Initialize();
		void Shutdown();

		void Update( float deltaTime );
	}
}