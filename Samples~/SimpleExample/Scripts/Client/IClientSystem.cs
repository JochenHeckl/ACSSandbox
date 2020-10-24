using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	internal interface IClientSystem
	{
		void Initialize();
		void Shutdown();
		void Update( float deltaTimeSec );
	}
}
