using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class Startup : IContext
	{
		private IContextResolver contextResolver;

		public Startup( IContextResolver contextResolverIn )
		{
			contextResolver = contextResolverIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
			// more or less a placeholder for now.
			// we probably have to check for availability of services, update the client etc
		}

		public void LeaveContext( IContextContainer contextContainer )
		{
			
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			contextContainer.PushConext( contextResolver.Resolve<ConnectToServer>() );
		}
	}
}
