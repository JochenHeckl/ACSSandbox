using System;
using System.IO;

using UnityEngine;

using de.JochenHeckl.Unity.IoCLight;
using Newtonsoft.Json;
using System.Linq;
using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class BootstrapClient : BootstrapBase, IContextContainer
	{
		private static readonly string configurationFileName = "Configuration.Client.json";

		[Space( 4 )]
		[Header("Configuration")]
		public TextAsset defaultConfiguration;
		public ClientResources clientResources;
		public RectTransform userInterfaceRoot;

		private IClientRuntimeData clientRuntimeData;
		private ClientConfiguration configuration;
		private IClientSystem[] clientSystems;

		public IContext ActiveContext { get; set; }

		public override void Compose()
		{
			Container.RegisterInstance( ParseConfiguration() );
			Container.RegisterInstance( clientResources ).As<IClientResources>().SingleInstance();

			clientRuntimeData = new ClientRuntimeData()
			{
				UserInterfaceRoot = userInterfaceRoot
			};

			Container.RegisterInstance( clientRuntimeData ).As<IClientRuntimeData>().SingleInstance();

			Container.Register<NetworkClientUnityTransport>().As<INetworkClient>().SingleInstance();
			
			Container.Register<LoginContext>();
		}

		public void Start()
		{
			configuration = Container.Resolve<ClientConfiguration>();

			Application.targetFrameRate = configuration.FPSLimit;

			clientSystems = Container.ResolveAll<IClientSystem>();

			foreach ( var system in clientSystems )
			{
				system.Initialize();
			}

			SwitchToContext( Container.Resolve<LoginContext>() );
		}

		public override void OnDestroy()
		{
			SwitchToContext( null );

			foreach ( var system in clientSystems.Reverse() )
			{
				system.Shutdown();
			}

			base.OnDestroy();
		}

		public void Update()
		{
			foreach ( var system in clientSystems )
			{
				system.Update( Time.deltaTime );
			}

			ActiveContext?.Update( Time.deltaTime );
		}

		private ClientConfiguration ParseConfiguration()
		{
			string configuration = defaultConfiguration.text;

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

		public void SwitchToContext( IContext newContext )
		{
			if( ActiveContext != null )
			{
				ActiveContext.LeaveContext( newContext );
			}

			newContext?.EnterContext( ActiveContext );

			ActiveContext = newContext;
		}

		public void PushConext( IContext context )
		{
			throw new NotImplementedException();
		}

		public void PopContext()
		{
			throw new NotImplementedException();
		}
	}
}