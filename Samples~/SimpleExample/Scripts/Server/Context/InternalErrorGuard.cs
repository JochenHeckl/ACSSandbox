using System;

namespace de.JochenHeckl.Unity.ACSSandbox.Server
{
	internal class InternalErrorGuard : IDisposable
	{
		public Action InternalErrorAction { get; set; }
		public bool RequestCompleted { get; set; }

		public void Dispose()
		{
			if( !RequestCompleted )
			{
				InternalErrorAction();
			}
		}
	}
}