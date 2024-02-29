using Unity.Logging;
using Unity.Logging.Internal.Debug;
using Unity.Logging.Sinks;
using UnityEngine;
using Logger = Unity.Logging.Logger;

namespace ACSSandbox.Common
{
    public class LogSetup : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
            Log.Logger = new Logger(
                new LoggerConfig()
                    .MinimumLevel.Debug()
                    .WriteTo.File(
                        "ACSSandbox.log",
                        minLevel: LogLevel.Verbose,
                        formatter: LogFormatterJson.Formatter
                    )
                    .WriteTo.StdOut(outputTemplate: "{Level} || {Timestamp} || {Message}")
            );

            SelfLog.SetMode(SelfLog.Mode.EnabledInUnityEngineDebugLogError);
        }
    }
}
