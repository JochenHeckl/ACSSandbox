using System.IO;
using System.Linq;

using de.JochenHeckl.Unity.ACSSandbox.Example.Protocol;
using de.JochenHeckl.Unity.IoCLight;

using Newtonsoft.Json;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Server
{
	public class BootstrapServer : BootstrapBase, IStateResolver
	{
		private static readonly string configurationFileName = "Configuration.Server.json";

		[Space( 4 )]
		[Header( "Configuration" )]
		public TextAsset defaultConfiguration;
		public ServerResources serverResources;
		public Transform worldRoot;

		private ServerConfiguration configuration;

		private IServerRuntimeData runtimeData;
		private IServerSystem[] serverSystems;

		private float lastIntegrationTimeSec;

		private readonly TimeSampler<IServerSystem> systemUpdateTimes = new TimeSampler<IServerSystem>();

		public override void Compose()
		{
			Container.RegisterInstance( this ).As<IStateResolver>();
			Container.RegisterInstance( serverResources ).SingleInstance();

			Container.RegisterInstance( ParseConfiguration() );

			runtimeData = new ServerRuntimeData()
			{
				ServerIntegrationTimeSec = 0f,
				WorldRoot = worldRoot,
			};

			Container.RegisterInstance( runtimeData ).SingleInstance();

			Container.RegisterInstance( SetupMessageSerializer() ).SingleInstance();

			Container.Register<NetworkServerUnityTransport>().SingleInstance();
			Container.Register<ServerNetworkMessageDispatcher>().SingleInstance();

			Container.Register<NetworkServerSystem>().SingleInstance();
			Container.Register<ServerStateSystem>().SingleInstance();
			Container.Register<SimulationSystem>().SingleInstance();

			Container.Register<ServerOperations>().As<IServerOperations>();

			Container.Register<StartupServer>();
			Container.Register<SimulateWorld>();
			
		}

		public void Start()
		{
			configuration = Container.Resolve<ServerConfiguration>();
			serverSystems = Container.ResolveAll<IServerSystem>();

			foreach ( var system in serverSystems )
			{
				system.Initialize();
				systemUpdateTimes.InitSample( system );
			}

			lastIntegrationTimeSec = Time.realtimeSinceStartup;
		}

		public override void OnDestroy()
		{
			File.WriteAllLines( "serverSytemUpdateTimes.TimeSamples.md", systemUpdateTimes.MarkDownSamples(
				"Server system update times",
				( system ) => system.GetType().Name ) );

			foreach ( var system in serverSystems.Reverse() )
			{
				system.Shutdown();
			}

			serverSystems = null;

			base.OnDestroy();
		}

		public void Update()
		{
			// We do not use Time.deltaTime here
			// nor do we use FixedUpdate() in combination with Time.fixedDeltaTime
			// to avoid time bleeding, which is when The process runs close the machines limits
			// unity will simply throttle down time to be able to keep up.
			// We do not want this to happen behind our backs.

			var nowSec = Time.realtimeSinceStartup;
			var deltaRealTimeSec = nowSec - lastIntegrationTimeSec;
			lastIntegrationTimeSec = nowSec;

			foreach ( var system in serverSystems )
			{
				systemUpdateTimes.StartSample();

				system.Update( deltaRealTimeSec );

				systemUpdateTimes.StopSample( system );
			}
		}

		public IState Resolve<ContextType>()
		{
			return Resolve( typeof( ContextType ) );
		}

		public IState Resolve( System.Type contextType )
		{
			return (IState) Container.Resolve( contextType );
		}

		private ServerConfiguration ParseConfiguration()
		{
			var configuration = defaultConfiguration.text;

			if ( !Application.isEditor )
			{
				var workingDirectory = Directory.GetCurrentDirectory();
				var configFileName = Path.Combine( workingDirectory, configurationFileName );

				if ( !File.Exists( configFileName ) )
				{
					File.WriteAllText( configFileName, defaultConfiguration.text );
				}
				else
				{
					configuration = File.ReadAllText( configFileName );
				}
			}

			return JsonConvert.DeserializeObject<ServerConfiguration>( configuration );
		}

		private IMessageSerializer SetupMessageSerializer()
		{
			var serializer = new MessageSerializerBson();

			foreach ( var message in MessageIds.ClientToServerMessageIds )
			{
				serializer.RegisterType( message.messageId, message.messageType );
			}

			foreach ( var message in MessageIds.ServerToClientMessageIds )
			{
				serializer.RegisterType( message.messageId, message.messageType );
			}

			return serializer;
		}
	}
}