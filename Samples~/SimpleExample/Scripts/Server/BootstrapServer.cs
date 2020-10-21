using System.IO;
using System.Linq;

using de.JochenHeckl.Unity.IoCLight;

using Newtonsoft.Json;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	public class BootstrapServer : BootstrapBase
	{
		private static readonly string configurationFileName = "Configuration.Server.json";

		public TextAsset defaultConfiguration;

		private ServerConfiguration configuration;
		private IServerSystem[] serverSystems;

		private float lastIntegrationTimeSec;

		public override void Compose()
		{
			Container.RegisterInstance( ParseConfiguration() );

			Container.Register<ServerRuntimeData>().As<IServerRuntimeData>().SingleInstance();

			Container.Register<NetworkServerUnityTransport>().As<INetworkServer>().SingleInstance();
			Container.Register<SimulationSystem>().As<IServerSystem>().SingleInstance();
		}

		public void Start()
		{
			configuration = Container.Resolve<ServerConfiguration>();
			serverSystems = Container.ResolveAll<IServerSystem>();

			foreach( var system in serverSystems )
			{
				system.Initialize();
			}

			lastIntegrationTimeSec = Time.realtimeSinceStartup;
		}

		public override void OnDestroy()
		{
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

			var targetTimeSec = Time.realtimeSinceStartup;

			while ( lastIntegrationTimeSec < targetTimeSec )
			{
				foreach ( var system in serverSystems )
				{
					system.Update( configuration.IntegrationTimeStepSec );
				}

				lastIntegrationTimeSec += configuration.IntegrationTimeStepSec;
			}
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
	}
}