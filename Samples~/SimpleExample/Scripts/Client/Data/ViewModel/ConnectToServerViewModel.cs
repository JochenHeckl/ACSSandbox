using System;
using System.Collections.Generic;

using TMPro;

using de.JochenHeckl.Unity.DataBinding;


namespace de.JochenHeckl.Unity.ACSSandbox.Client
{
	public class ConnectToServerViewModel : ViewModelBase
	{
		public string WellKnownHostsLabel { get; set; }
		public int SelectedOptionIndex { get; set; }
		public List<TMP_Dropdown.OptionData> OptionData { get; set; }
		public TMP_Dropdown.DropdownEvent WellKnownHostValueChanged { get; set; }

		public string ServerAddressLabel { get; set; }
		public string ServerAddressText { get; internal set; }

		public string ServerPortLabel { get; set; }
		public string ServerPortText { get; internal set; }

		public string LoginButtonLabel { get; set; }
		public bool EnableLogin { get; internal set; }
		public Action<string, int> LoginAction { get; set; }
	}
}
