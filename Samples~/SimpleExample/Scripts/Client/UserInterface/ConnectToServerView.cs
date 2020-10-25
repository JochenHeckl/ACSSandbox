using System;
using System.Linq;

using de.JochenHeckl.Unity.DataBinding;

using TMPro;

using UnityEngine;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ConnectToServerView : ClientView
	{
		public TMP_InputField serverAddress;
		public TMP_InputField serverPort;

		public Action<string, int> LoginAction { get; set; }

		public void HandleLoginButtonClicked()
		{
			LoginAction?.Invoke( serverAddress.text, int.Parse( serverPort.text ) );
		}
	}
}