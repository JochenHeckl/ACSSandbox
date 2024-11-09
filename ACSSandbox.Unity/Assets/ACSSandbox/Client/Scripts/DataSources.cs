namespace ACSSandbox.Client
{
    public class DataSources
    {
        public NetworkConnectionStatus NetworkConnectionStatus { get; set; } = new();

        public LoginPanelDataSource LoginPanelDataSource { get; set; } = new();
    }
}
