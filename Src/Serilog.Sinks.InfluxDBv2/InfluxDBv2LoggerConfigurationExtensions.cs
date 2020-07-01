using Serilog.Configuration;
using Serilog.Events;
using System;
using static InfluxDB.Client.InfluxDBClientOptions;

namespace Serilog.Sinks.InfluxDBv2
{
    /// <summary>
    /// Adds the WriteTo.InfluxDBv2() extension method to <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class InfluxDBv2LoggerConfigurationExtensions
    {
        /// <summary>
        /// Writes log events to InfluxDB.
        /// </summary>
        public static LoggerConfiguration InfluxDBv2(
            this LoggerSinkConfiguration loggerConfiguration,
            string source,
            string address,
            string bucket,
            string token,
            string organization = InfluxDBv2Defaults.DefaultOrganizationName,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = InfluxDBv2Sink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (string.IsNullOrEmpty(source)) throw new ArgumentException("source");
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentException("bucket");
            if (string.IsNullOrEmpty(token)) throw new ArgumentException("token");

            return InfluxDBv2(
                loggerConfiguration, source, address,
                bucket, null, null, token,
                organization, restrictedToMinimumLevel,
                batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Writes log events to InfluxDB.
        /// </summary>
        public static LoggerConfiguration InfluxDBv2(
            this LoggerSinkConfiguration loggerConfiguration,
            string source,
            string address,
            string bucket,
            string username,
            string password,
            string organization = InfluxDBv2Defaults.DefaultOrganizationName,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = InfluxDBv2Sink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            if (loggerConfiguration == null) throw new ArgumentNullException(nameof(loggerConfiguration));
            if (string.IsNullOrEmpty(source)) throw new ArgumentException("source");
            if (string.IsNullOrEmpty(address)) throw new ArgumentNullException(nameof(address));
            if (string.IsNullOrEmpty(bucket)) throw new ArgumentException("bucket");
            if (string.IsNullOrEmpty(username)) throw new ArgumentException("username");
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("password");

            return InfluxDBv2(
                loggerConfiguration, source, address,
                bucket, username, password, null,
                organization, restrictedToMinimumLevel,
                batchPostingLimit, period, formatProvider);
        }

        /// <summary>
        /// Writes log events to InfluxDB.
        /// </summary>
        private static LoggerConfiguration InfluxDBv2(
            this LoggerSinkConfiguration loggerConfiguration,
            string source,
            string address,
            string bucket,
            string username,
            string password,
            string token,
            string organization = InfluxDBv2Defaults.DefaultOrganizationName,
            LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            int batchPostingLimit = InfluxDBv2Sink.DefaultBatchPostingLimit,
            TimeSpan? period = null,
            IFormatProvider formatProvider = null)
        {
            // Define connection type
            var connectionType = AuthenticationScheme.Session;
            if(!string.IsNullOrEmpty(token))
            {
                connectionType = AuthenticationScheme.Token;
            }

            // Create connection info configuration
            var connectionInfo = new InfluxDBv2ConnectionInfo
            {
                Address = address,
                Bucket = bucket,
                Organization = organization,
                AuthenticationType = connectionType,
                Username = username,
                Password = password,
                Token = token
            };

            // Set the default period if it is null
            var defaultedPeriod = period ?? InfluxDBv2Sink.DefaultPeriod;

            // Register InfluxDB in Serilog
            return loggerConfiguration.Sink(new InfluxDBv2Sink(connectionInfo, source, batchPostingLimit, defaultedPeriod, formatProvider), restrictedToMinimumLevel);
        }

    }
}
