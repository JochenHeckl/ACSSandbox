using System;
using JH.DataBinding;

namespace ACSSandbox.Client
{
    public class LoginPanelDataSource : DataSourceBase<LoginPanelDataSource>
    {
        public string Hostname { get; set; }
        public string Username { get; set; }

        public Action<string, string> HandleConnect { get; set; }
    }
}
