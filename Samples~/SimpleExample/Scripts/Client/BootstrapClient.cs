using System.IO;
using System.Linq;

using UnityEngine;

using Newtonsoft.Json;

using de.JochenHeckl.Unity.IoCLight;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Protocol;

namespace de.JochenHeckl.Unity.ACSSandbox.Example.Client
{
	public class BootstrapClient : BootstrapBase, IStateResolver
	{
		private static readonly string configurationFileName = "Configuration.Client.json";

		[Space( 4 )]
		[Header( "Configuration" )]
		public TextAsset defaultConfiguration;
		public ClientResources clientResources;
		public Transform worldRoot;
		public RectTransform userInterfaceRoot;

		private IClientRuntimeData runtimeData;
		private IClientSystem[] clientSystems;

		private readonly TimeSampler<IClientSystem> systemUpdateTimes = new TimeSampler<IClientSystem>();

		public override void Compose()
		{
			Container.RegisterInstance( this ).As<IStateResolver>();

			Container.RegisterInstance( ParseConfiguration() );
			Container.RegisterInstance( clientResources ).SingleInstance();

			runtimeData = new ClientRuntimeData()
			{
				TimeSec = Time.realtimeSinceStartup,

				WorldRoot = worldRoot,
				UserInterfaceRoot = userInterfaceRoot
			};

			Container.RegisterInstance( runtimeData ).SingleInstance();

			Container.RegisterInstance( SetupMessageSerializer() ).SingleInstance();

			Container.Register<NetworkClientUnityTransport>().SingleInstance();
			Container.Register<ClientNetworkMessageDispatcher>().SingleInstance();

			Container.Register<NetworkClientSystem>().SingleInstance();
			Container.Register<ClientStateSystem>().SingleInstance();
			Container.Register<UserInputSystem>().SingleInstance();

			Container.Register<ClientOperations>().As<IClientOperations>().SingleInstance();

			Container.Register<StartupClient>();
			Container.Register<ConnectToServer>();
			Container.Register<InteractWithWorld>();
		}

		public void OnEnable()
		{
			var configuration = Container.Resolve<ClientConfiguration>();
			Application.targetFrameRate = configuration.FPSLimit;

			clientSystems = Container.ResolveAll<IClientSystem>();

			foreach ( var system in clientSystems )
			{
				system.Initialize();
				systemUpdateTimes.InitSample( system );
			}
		}

		public void OnDisable()
		{
			File.WriteAllLines( "clientSytemUpdateTimes.TimeSamples.md", systemUpdateTimes.MarkDownSamples(
				"Client system update times",
				( system ) => system.GetType().Name ) );

			foreach ( var system in clientSystems.Reverse() )
			{
				system.Shutdown();
			}
		}

		public void Update()
		{
			foreach ( var system in clientSystems )
			{
				systemUpdateTimes.StartSample();

				system.Update( Time.deltaTime );

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

		private ClientConfiguration ParseConfiguration()
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

			return JsonConvert.DeserializeObject<ClientConfiguration>( configuration );
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