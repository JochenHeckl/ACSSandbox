using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;
using de.JochenHeckl.Unity.ACSSandbox.Example;
using de.JochenHeckl.Unity.IoCLight;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal class Startup : IContext
	{
		private IContainer iocContainer;

		public Startup( IContainer iocContainerIn )
		{
			iocContainer = iocContainerIn;
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
			contextContainer.PushConext( iocContainer.Resolve<Login>() );
		}
	}
}
