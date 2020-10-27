using System;

using TMPro;

namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ConnectToServerView : ContextUIView
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