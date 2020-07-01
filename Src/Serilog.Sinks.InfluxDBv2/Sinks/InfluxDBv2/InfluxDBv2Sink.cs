using InfluxDB.Client;
using InfluxDB.Client.Writes;
using Serilog.Events;
using Serilog.Sinks.PeriodicBatching;
using System;
using System.Collections.Generic;
using System.Linq;
using static InfluxDB.Client.InfluxDBClientOptions;

namespace Serilog.Sinks.InfluxDBv2
{
    /// <summary>
    /// Extends <see cref="LoggerConfiguration"/> with methods to add InfluxDB sinks.
    /// </summary>
    public class InfluxDBv2Sink : PeriodicBatchingSink
    {
        /// <summary>
        /// A reasonable default for the number of events posted in
        /// each batch.
        /// </summary>
        public const int DefaultBatchPostingLimit = 100;

        /// <summary>
        /// A reasonable default time to wait between checking for event batches.
        /// </summary>
        public static readonly TimeSpan DefaultPeriod = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Connection info used to connect to InfluxDB instance.
        /// </summary>
        private readonly InfluxDBv2ConnectionInfo _connectionInfo;

        /// <summary>
        /// Format an object for outpout.
        /// </summary>
        private readonly IFormatProvider _formatProvider;

        /// <summary>
        /// Client object used to connect to InfluxDB instance.
        /// </summary>
        private readonly InfluxDBClient _influxDbClient;

        /// <summary>
        /// Application source name.
        /// </summary>
        private readonly string _source;

        /// <inheritdoc />
        /// <summary>
        /// Construct a sink inserting into InfluxDB with the specified details.
        /// </summary>
        /// <param name="connectionInfo">Connection information used to construct InfluxDB client.</param>
        /// <param name="source">Measurement name in the InfluxDB database.</param>
        /// <param name="batchSizeLimit">The maximum number of events to post in a single batch.</param>
        /// <param name="period">The time to wait between checking for event batches.</param>
        /// <param name="formatProvider"></param>
        public InfluxDBv2Sink(
            InfluxDBv2ConnectionInfo connectionInfo,
            string source,
            int batchSizeLimit,
            TimeSpan period,
            IFormatProvider formatProvider
            ) : base(batchSizeLimit, period)
        {
            _connectionInfo = connectionInfo ?? throw new ArgumentNullException(nameof(connectionInfo));
            _source = source;
            _influxDbClient = CreateInfluxDbClient();
            _formatProvider = formatProvider;
        }

        /// <inheritdoc />
        /// <summary>
        /// Emit a batch of log events, running asynchronously.
        /// </summary>
        /// <param name="events">The events to emit.</param>
        /// <remarks>Override either <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatch(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" /> or <see cref="M:Serilog.Sinks.PeriodicBatching.PeriodicBatchingSink.EmitBatchAsync(System.Collections.Generic.IEnumerable{Serilog.Events.LogEvent})" />,
        /// not both.</remarks>
        protected override void EmitBatch(IEnumerable<LogEvent> events)
        {
            if (events == null) throw new ArgumentNullException(nameof(events));

            var logEvents = events as List<LogEvent> ?? events.ToList();
            var points = new List<PointData>(logEvents.Count);

            foreach (var logEvent in logEvents)
            {
                // Create point data object
                var point = PointData.Measurement(_source);

                // Add tags
                point = point.Tag("level", logEvent.Level.ToString());
                point = point.Tag("hostname", Environment.MachineName);
                if (logEvent.Exception != null) point = point.Tag("exceptionType", logEvent.Exception.GetType().Name);
                if (logEvent.MessageTemplate != null) point = point.Tag("messageTemplate", logEvent.MessageTemplate.Text);

                // Add fields
                foreach (var property in logEvent.Properties)
                {
                    point = point.Field(property.Key, property.Value.ToString());
                }
                point = point.Field("message", logEvent.RenderMessage(_formatProvider));

                // Set the log event timestamp
                point = point.Timestamp(DateTime.UtcNow, InfluxDB.Client.Api.Domain.WritePrecision.Ns);

                // Add a point in the list of points
                points.Add(point);
            }

            // Send log events to InfluxDB instance.
            using var writeApi = _influxDbClient.GetWriteApi();
            writeApi.WritePoints(_connectionInfo.Bucket, _connectionInfo.Organization, points);
        }

        /// <summary>
        /// Initialize and return an InfluxDB client object.
        /// </summary>
        /// <returns></returns>
        private InfluxDBClient CreateInfluxDbClient()
        {
            return _connectionInfo.AuthenticationType switch
            {
                AuthenticationScheme.Token => InfluxDBClientFactory.Create(_connectionInfo.Address, _connectionInfo.Token.ToCharArray()),
                _ => InfluxDBClientFactory.Create(_connectionInfo.Address, _connectionInfo.Username, _connectionInfo.Password.ToCharArray()),
            };
        }
    }
}
