namespace Serilog.Sinks.InfluxDBv2
{
    /// <summary>
    /// Default value for the InfluxDB sink.
    /// </summary>
    public class InfluxDBv2Defaults
    {
        /// <summary>
        /// Default database name in InfluxDB
        /// </summary>
        public const string DefaultBucketName = "LogDb";

        /// <summary>
        /// Default organization name in InfluxDB.
        /// </summary>
        public const string DefaultOrganizationName = "None";
    }
}
