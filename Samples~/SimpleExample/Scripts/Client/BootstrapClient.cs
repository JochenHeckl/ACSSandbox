using System;
using System.IO;

using UnityEngine;

using de.JochenHeckl.Unity.IoCLight;
using Newtonsoft.Json;
using System.Linq;
using de.JochenHeckl.Unity.ACSSandbox.Common;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using de.JochenHeckl.Unity.ACSSandbox.Example;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class BootstrapClient : BootstrapBase
	{
		private static readonly string configurationFileName = "Configuration.Client.json";

		[Space( 4 )]
		[Header("Configuration")]
		public TextAsset defaultConfiguration;
		public ClientResources clientResources;
		public RectTransform userInterfaceRoot;

		private IClientRuntimeData runtimeData;
		private IClientSystem[] clientSystems;

		public override void Compose()
		{
			Container.RegisterInstance( Container );
				
			Container.RegisterInstance( ParseConfiguration() );
			Container.RegisterInstance( clientResources ).SingleInstance();

			runtimeData = new ClientRuntimeData()
			{
				TimeSec = Time.realtimeSinceStartup,
				UserInterfaceRoot = userInterfaceRoot
			};

			Container.RegisterInstance( runtimeData ).SingleInstance();
			
			Container.RegisterInstance( SetupMessageSerializer() ).SingleInstance();

			Container.Register<NetworkClientUnityTransport>().SingleInstance();
			Container.Register<NetworkMessageDispatcher>().SingleInstance();
			
			Container.Register<ContextSystem>().SingleInstance();


			Container.Register<Startup>();
			Container.Register<Login>();
			Container.Register<AcquireGlobalServerData>();
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

		public void Start()
		{
			var configuration = Container.Resolve<ClientConfiguration>();
			Application.targetFrameRate = configuration.FPSLimit;

			var contextSystem = Container.Resolve<IContextContainer>();
			
			clientSystems = Container.ResolveAll<IClientSystem>();

			foreach ( var system in clientSystems )
			{
				system.Initialize();
			}
		}

		public override void OnDestroy()
		{
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
	}
}