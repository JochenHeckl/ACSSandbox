namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal interface IServerSystem
	{
		void Initialize();
		void Shutdown();

		void Update( float deltaTime );
	}
}