
using System.ComponentModel;
using static InfluxDB.Client.InfluxDBClientOptions;

namespace Serilog.Sinks.InfluxDBv2
{
    /// <summary>
    /// Connection information for use by the InfluxDB sink.
    /// </summary>
    public class InfluxDBv2ConnectionInfo
    {
        /// <summary>
        /// Constructs the <see cref="InfluxDBv2ConnectionInfo"/> with the default port and database name.
        /// </summary>
        public InfluxDBv2ConnectionInfo()
        {
            Bucket = InfluxDBv2Defaults.DefaultBucketName;
        }

        /// <summary>
        /// Address of InfluxDB instance.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the bucket name in InfluxDB.
        /// Default value is logDb.
        /// </summary>
        [DefaultValue(InfluxDBv2Defaults.DefaultBucketName)]
        public string Bucket { get; set; }

        /// <summary>
        /// Gets or sets the organization name in InfluxDB.
        /// Default value is logDb.
        /// </summary>
        [DefaultValue(InfluxDBv2Defaults.DefaultOrganizationName)]
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the authentication scheme used to connect to InfluxDB.
        /// </summary>
        public AuthenticationScheme AuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the username used for authentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password used for authentication.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the token used for authentication.
        /// </summary>
        public string Token { get; set; }
    }
}