using Unity.Logging;
using Unity.Logging.Internal.Debug;
using Unity.Logging.Sinks;
using UnityEngine;

namespace ACSSandbox.Common
{
    public class AppSetup : MonoBehaviour
    {
        [Header("Target FrameRate")]
        public int targetFrameRate = 1;

        [Header("Logging")]
        public string logFile = "ACSSandbox.log";
        public LogLevel FileLogLevel = LogLevel.Verbose;
        public LogLevel ConsoleLogLevel = LogLevel.Info;

        // Start is called before the first frame update
        void Awake()
        {
            Log.Logger = new Unity.Logging.Logger(
                new LoggerConfig()
                    .WriteTo.File(
                        logFile,
                        outputTemplate: "{Level} || {Timestamp} || {Message}",
                        minLevel: FileLogLevel,
                        formatter: LogFormatterText.Formatter,
                        captureStackTrace: true
                    )
                    .WriteTo.StdOut(minLevel: ConsoleLogLevel, captureStackTrace: false)
            );

            Application.targetFrameRate = targetFrameRate;
        }
    }
}
