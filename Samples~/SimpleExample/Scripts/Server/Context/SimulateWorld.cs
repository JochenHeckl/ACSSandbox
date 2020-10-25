using System;

using de.JochenHeckl.Unity.ACSSandbox.Common;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class SimulateWorld : IContext
	{
		private IContextResolver contextResolver;

		public SimulateWorld( IContextResolver contextResolverIn )
		{
			contextResolver = contextResolverIn;
		}

		public void EnterContext( IContextContainer contextContainer )
		{
		}

		public void LeaveContext( IContextContainer contextContainer )
		{	
		}

		public void Update( IContextContainer contextContainer, float deltaTimeSec )
		{
			// test for conditions to shut down the server
		}
	}
}
