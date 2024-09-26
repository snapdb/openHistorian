// Example model class for a connection

//using System.ComponentModel;
//using System.Reflection;
//using System.Text.Json.Serialization;
//using Gemstone.Communication;
//using Gemstone.Configuration;
//using Gemstone.Diagnostics;
//using Gemstone.PhasorProtocols;
//using Gemstone.Reflection.MemberInfoExtensions;
//using Gemstone.StringExtensions;
//using static ServiceInterface.Defaults;

//namespace ServiceInterface;

///// <summary>
///// Represents all the settings related to a connection.
///// </summary>
///// <remarks>
///// Class is intended to represent connection settings for simple UI binding.
///// </remarks>
//public class Connection
//{
//    /// <summary>
//    /// Defines the default interface value.
//    /// </summary>
//    public const string DefaultInterface = "0.0.0.0";

//    /// <summary>
//    /// Gets or sets the connection ID.
//    /// </summary>
//    public Guid ID { get; set; }

//    /// <summary>
//    /// Gets or sets the name of the connection.
//    /// </summary>
//    public string Name { get; set; } = default!;

//    /// <summary>
//    /// Gets or sets the flag that determines if connection is enabled.
//    /// </summary>
//    public bool Enabled { get; set; }

//    /// <summary>
//    /// Gets or sets the access ID of the connection.
//    /// </summary>
//    public ushort AccessID { get; set; }

//    /// <summary>
//    /// Gets or sets protocol type of the connection.
//    /// </summary>
//    public PhasorProtocol PhasorProtocol { get; set; }

//    /// <summary>
//    /// Gets or sets the connection parameters for the connection.
//    /// </summary>
//    public ConnectionParameter[] ConnectionParameters { get; set; } = [];

//    /// <summary>
//    /// Gets connection parameters grouped by category.
//    /// </summary>
//    [JsonIgnore]
//    public Dictionary<string, ConnectionParameter[]> GroupedConnectionParameters => ConnectionParameters
//        .GroupBy(parameter => parameter.Category)
//        .ToDictionary(group => group.Key, group => group.ToArray());
 
//    /// <summary>
//    /// Gets or sets the transport protocol of the source connection.
//    /// </summary>
//    /// <remarks>
//    /// Currently only <see cref="TransportProtocol.Tcp"/> and <see cref="TransportProtocol.Udp"/>
//    /// are supported for source connections.
//    /// </remarks>
//    public TransportProtocol SourceTransportProtocol { get; set; }

//    /// <summary>
//    /// Gets or sets the alternate Command transport protocol of the source connection.
//    /// </summary>
//    /// <remarks>
//    /// Currently only <see cref="TransportProtocol.Tcp"/>
//    /// is supported for source connections.
//    /// </remarks>
//    public TransportProtocol AlternateCommandTransportProtocol { get; set; }

//    /// <summary>
//    /// Gets or sets the interface of the source connection. Defaults to "0.0.0.0".
//    /// </summary>
//    public string SourceInterface { get; set; } = DefaultInterface;

//    /// <summary>
//    /// Gets or sets the TCP end point(s) of the source connection, format: "host:port".
//    /// </summary>
//    /// <remarks>
//    /// <para>
//    /// Multiple end points can be specified, separated by commas, for example:
//    /// <c>192.168.1.1:5744,192.168.1.2:5745</c>
//    /// </para>
//    /// <para>
//    /// Endpoints can override default access ID using slash format "host:port/altID", for example:
//    /// <c>192.168.1.1:5744,192.168.1.2:5745/189</c>
//    /// </para>
//    /// </remarks>
//    public string? SourceTcpEndPoint { get; set; }

//    /// <summary>
//    /// Gets or sets flag that determines if source TCP connection is in listening mode, i.e.,
//    /// is being setup as a reverse connection.
//    /// </summary>
//    public bool SourceTcpIsListener { get; set; }

//    /// <summary>
//    /// Gets or sets the TCP listening port when <see cref="SourceTcpIsListener"/> is <c>true</c>.
//    /// </summary>
//    public ushort SourceTcpServerPort { get; set; }

//    /// <summary>
//    /// Gets or sets the UDP listening port.
//    /// </summary>
//    public ushort SourceUdpServerPort { get; set; }

//    /// <summary>
//    /// Gets or sets flag that determines if source connection uses an alternate command channel.
//    /// </summary>
//    public bool SourceUsesAltCommandChannel { get; set; }

//    /// <summary>
//    /// Gets or sets the end point(s) of the alternate command channel for the source connection,
//    /// format: "host:port".
//    /// </summary>
//    /// <remarks>
//    /// <para>
//    /// Currently the alternate command channel is restricted to <see cref="TransportProtocol.Tcp"/>
//    /// and is assumed to use the same <see cref="SourceInterface"/>, for configuration simplicity.
//    /// </para>
//    /// <para>
//    /// Additionally, multiple end points can be specified, separated by commas, for example:
//    /// <c>192.168.1.1:5744,192.168.1.2:5745</c>
//    /// </para>
//    /// <para>
//    /// Alternate command channel does not currently support override of default access ID.
//    /// </para>
//    /// </remarks>
//    public string? SourceAltCommandChannelTcpEndPoint { get; set; }

//    /// <summary>
//    /// Gets or sets the transport protocol of the proxy connection.
//    /// </summary>
//    /// <remarks>
//    /// Currently only <see cref="TransportProtocol.Tcp"/> and <see cref="TransportProtocol.Udp"/>
//    /// are supported for proxy connections.
//    /// </remarks>
//    public TransportProtocol ProxyTransportProtocol { get; set; }

//    /// <summary>
//    /// Gets or sets the interface of the proxy connection. Defaults to "0.0.0.0".
//    /// </summary>
//    public string ProxyInterface { get; set; } = DefaultInterface;

//    /// <summary>
//    /// Gets or sets the TCP end point(s) of the proxy connection, format: "host:port",
//    /// used in reverse connection scenarios.
//    /// </summary>
//    /// <remarks>
//    /// <para>
//    /// Multiple end points can be specified, separated by commas, for example:
//    /// <c>192.168.1.1:5744,192.168.1.2:5745</c>
//    /// </para>
//    /// <para>
//    /// TCP proxy connection does not support override of default access ID, this just
//    /// forwards received data.
//    /// </para>
//    /// </remarks>
//    public string? ProxyTcpEndPoint { get; set; }

//    /// <summary>
//    /// Gets or sets flag that determines if proxy TCP connection is in listening mode, i.e.,
//    /// is being setup as a proxy server data redistribution point connection, most common, or,
//    /// instead is being used to feed data to a listening reverse source connection.
//    /// </summary>
//    public bool ProxyTcpIsListener { get; set; }

//    /// <summary>
//    /// Gets or sets the TCP listening port when <see cref="ProxyTcpIsListener"/> is <c>true</c>.
//    /// </summary>
//    public ushort ProxyTcpServerPort { get; set; }

//    /// <summary>
//    /// Gets or sets collection of proxy UDP end points, each with format: "host:port".
//    /// </summary>
//    public string[] ProxyUdpEndPoints { get; set; } = [];

//    /// <summary>
//    /// Gets or sets the connection string of the connection.
//    /// </summary>
//    /// <remarks>
//    /// Property serializes and deserializes all connection settings to and from a string.
//    /// </remarks>
//    [JsonIgnore]
//    public string ConnectionString
//    {
//        get
//        {
//            // Generate source connection information
//            Dictionary<string, string> sourceSettings = new(StringComparer.OrdinalIgnoreCase)
//            {
//                ["transportProtocol"] = $"{SourceTransportProtocol}",
//                ["interface"] = SourceInterface,
//                ["phasorProtocol"] = $"{PhasorProtocol}",
//                ["accessID"] = $"{AccessID}"
//            };

//            switch (SourceTransportProtocol)
//            {
//                case TransportProtocol.Tcp:
//                    if (SourceTcpIsListener)
//                    {
//                        sourceSettings["port"] = $"{SourceTcpServerPort}";
//                        sourceSettings["isListener"] = "true";
//                    }
//                    else
//                    {
//                        sourceSettings["server"] = SourceTcpEndPoint!;
//                    }

//                    break;
//                case TransportProtocol.Udp:
//                    sourceSettings["localport"] = $"{SourceUdpServerPort}";
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("transportProtocol", "Service currently only supports TCP or UDP source transports");
//            }

//            if (SourceUsesAltCommandChannel)
//            {
//                sourceSettings["commandChannel"] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
//                {
//                    ["server"] = SourceAltCommandChannelTcpEndPoint!,
//                    ["interface"] = SourceInterface,
//                    ["maxSendQueueSize"] = $"{MaxSendQueueSize}",
//                    ["transportProtocol"] = $"{AlternateCommandTransportProtocol}",
//                }
//                .JoinKeyValuePairs();
//            }

//            // Generate proxy connection information
//            Dictionary<string, string> proxySettings = new(StringComparer.OrdinalIgnoreCase)
//            {
//                ["protocol"] = ProxyTransportProtocol.ToString(),
//                ["interface"] = ProxyInterface
//            };

//            switch (ProxyTransportProtocol)
//            {
//                case TransportProtocol.Tcp:
//                    if (ProxyTcpIsListener)
//                    {
//                        proxySettings["port"] = $"{ProxyTcpServerPort}";
//                    }
//                    else
//                    {
//                        proxySettings["server"] = ProxyTcpEndPoint!;
//                        proxySettings["useClientPublishChannel"] = "true";
//                    }

//                    break;
//                case TransportProtocol.Udp:
//                    proxySettings["port"] = "-1";
//                    proxySettings["clients"] = string.Join(", ", ProxyUdpEndPoints.Where(value => !string.IsNullOrWhiteSpace(value)));
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("protocol", "Service currently only supports TCP or UDP proxy transports");
//            }

//            sourceSettings["maxSendQueueSize"] = $"{MaxSendQueueSize}";
//            proxySettings["maxSendQueueSize"] = $"{MaxSendQueueSize}";

//            // Return full connection string
//            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
//            {
//                ["sourceSettings"] = sourceSettings.JoinKeyValuePairs(),
//                ["proxySettings"] = proxySettings.JoinKeyValuePairs(),
//                ["enabled"] = $"{Enabled}",
//                ["name"] = Name
//            }
//            .JoinKeyValuePairs();
//        }
//        set
//        {
//            Dictionary<string, string> settings = value.ParseKeyValuePairs();

//            Name = settings.TryGetValue("name", out string? setting) ? setting : ID.ToString();
//            Enabled = settings.TryGetValue("enabled", out setting) && setting.ParseBoolean();

//            // Parse source connection settings
//            if (settings.TryGetValue("sourceSettings", out setting))
//            {
//                Dictionary<string, string> sourceSettings = setting.ParseKeyValuePairs();

//                SourceTransportProtocol = sourceSettings.TryGetValue("transportProtocol", out setting) && Enum.TryParse(setting, false, out TransportProtocol transportProtocol) ? transportProtocol : TransportProtocol.Tcp;

//                if (SourceTransportProtocol != TransportProtocol.Tcp && SourceTransportProtocol != TransportProtocol.Udp)
//                    throw new ArgumentOutOfRangeException("transportProtocol", "Service currently only supports TCP or UDP source transports");

//                SourceInterface = sourceSettings.TryGetValue("interface", out setting) ? setting : DefaultInterface;
//                PhasorProtocol = sourceSettings.TryGetValue("phasorProtocol", out setting) && Enum.TryParse(setting, false, out PhasorProtocol phasorProtocol) ? phasorProtocol : PhasorProtocol.IEEEC37_118V2;
//                AccessID = sourceSettings.TryGetValue("accessID", out setting) ? ushort.Parse(setting) : (ushort)1;
//                SourceTcpServerPort = sourceSettings.TryGetValue("port", out setting) && ushort.TryParse(setting, out ushort port) ? port : default;

//                if (sourceSettings.TryGetValue("server", out setting))
//                    SourceTcpEndPoint = setting;

//                SourceTcpIsListener = sourceSettings.TryGetValue("isListener", out setting) && setting.ParseBoolean();

//                if (sourceSettings.TryGetValue("localport", out setting))
//                    SourceUdpServerPort = ushort.Parse(setting);

//                if (sourceSettings.TryGetValue("commandChannel", out setting))
//                {
//                    SourceUsesAltCommandChannel = true;

//                    if (setting.ParseKeyValuePairs().TryGetValue("server", out string? cmdChannelsetting))
//                        SourceAltCommandChannelTcpEndPoint = cmdChannelsetting;

//                    AlternateCommandTransportProtocol = setting.ParseKeyValuePairs().TryGetValue("transportProtocol", out cmdChannelsetting) && Enum.TryParse(cmdChannelsetting, false, out transportProtocol) ? transportProtocol : TransportProtocol.Tcp;
//                }
//            }
//            else
//            {
//                throw new InvalidOperationException("Source settings are missing from connection string.");
//            }

//            // Parse proxy connection settings
//            if (settings.TryGetValue("proxySettings", out setting))
//            {
//                Dictionary<string, string> proxySettings = setting.ParseKeyValuePairs();

//                ProxyTransportProtocol = proxySettings.TryGetValue("protocol", out setting) && Enum.TryParse(setting, false, out TransportProtocol transportProtocol) ? transportProtocol : TransportProtocol.Tcp;
                
//                if (ProxyTransportProtocol != TransportProtocol.Tcp && ProxyTransportProtocol != TransportProtocol.Udp)
//                    throw new ArgumentOutOfRangeException("protocol", "Service currently only supports TCP or UDP proxy transports");

//                ProxyInterface = proxySettings.TryGetValue("interface", out setting) ? setting : DefaultInterface;
//                ProxyTcpServerPort = proxySettings.TryGetValue("port", out setting) && ushort.TryParse(setting, out ushort port) ? port : default;

//                if (proxySettings.TryGetValue("server", out setting))
//                    ProxyTcpEndPoint = setting;

//                if (proxySettings.TryGetValue("useClientPublishChannel", out setting))
//                    ProxyTcpIsListener = !setting.ParseBoolean();
//                else
//                    ProxyTcpIsListener = ProxyTcpServerPort > 0;

//                ProxyUdpEndPoints = proxySettings.TryGetValue("clients", out setting) ? setting.Split((char[])[','], StringSplitOptions.RemoveEmptyEntries).ToArray() : [];
//            }
//            else
//            {
//                throw new InvalidOperationException("Proxy settings are missing from connection string.");
//            }
//        }
//    }

//    private static int? s_maxSendQueueSize;

//    internal static int MaxSendQueueSize
//    {
//        get
//        {
//            if (s_maxSendQueueSize is not null)
//                return s_maxSendQueueSize.Value;

//            Settings settings = Settings.Instance;
//            return s_maxSendQueueSize ??= settings.GetSetting(nameof(MaxSendQueueSize), DefaultMaxQueueSize);
//        }
//    }

//    /// <summary>
//    /// Generates connection parameters for the given connection.
//    /// </summary>
//    /// <param name="connection">Source connection.</param>
//    /// <returns>New protocol specific instance of connection parameters.</returns>
//    public static IConnectionParameters? GenerateConnectionParameters(Connection? connection)
//    {
//        // Using reflection for the current phasor protocol, assume the Gemstone.PhasorProtocol namespace
//        // for the protocol matches the protocol name, then check if a ConnectionParameters class exists
//        // for the protocol, and if so, create the type and load the connection parameters
//        const string PhasorProtocolsNamespace = $"{nameof(Gemstone)}.{nameof(Gemstone.PhasorProtocols)}";

//        if (connection is null)
//            return null;

//        // Attempt to get the ConnectionParameters type for the protocol - this assumes protocol name matches namespace and
//        // connection parameters class name is named "ConnectionParameters". This matches current implementation pattern for
//        // all current protocols that have custom connection parameters:
//        Type? connectionParametersType = Type.GetType($"{PhasorProtocolsNamespace}.{connection.PhasorProtocol}.ConnectionParameters, {PhasorProtocolsNamespace}");

//        // For some types, the connection parameters type does not exist (as it's not needed)
//        if (connectionParametersType is null)
//            return null;

//        // Create an instance of connections parameter type and set its fields based on connection parameters
//        if (Activator.CreateInstance(connectionParametersType) is not IConnectionParameters parameters)
//            return null;

//        foreach (ConnectionParameter parameter in connection.ConnectionParameters)
//        {
//            PropertyInfo? property = connectionParametersType.GetProperty(parameter.Name);

//            try
//            {
//                property?.SetValue(parameters, parameter.Value.ConvertToType(property.PropertyType));
//            }
//            catch (Exception ex)
//            {
//                Logger.SwallowException(ex);
//            }
//        }

//        return parameters;
//    }

//    /// <summary>
//    /// Uses reflection to generate connection parameters for the given connection.
//    /// </summary>
//    /// <param name="parameters">Connection parameters to parse.</param>
//    /// <returns>Collection of connection parameters.</returns>
//    public static ConnectionParameter[] ParseConnectionParameters(IConnectionParameters? parameters)
//    {
//        return parameters?.GetType().GetProperties().Select(property => new ConnectionParameter
//        {
//            Name = property.Name,
//            Category = getCategory(property),
//            Description = getDescription(property),
//            DataType = getDataType(property),
//            DefaultValue = getDefaultValue(property)?.ToString() ?? "",
//            AvailableValues = getAvailableValues(property),
//            Value = property.GetValue(parameters)?.ToString() ?? ""
//        })
//        .ToArray() ?? [];

//        static string getCategory(PropertyInfo value)
//        {
//            return value.TryGetAttribute(out CategoryAttribute? attribute) ? attribute.Category : "General";
//        }

//        static string getDescription(PropertyInfo value)
//        {
//            return value.TryGetAttribute(out DescriptionAttribute? attribute) ? attribute.Description : string.Empty;
//        }

//        static object? getDefaultValue(PropertyInfo value)
//        {
//            return value.TryGetAttribute(out DefaultValueAttribute? attribute) ? attribute.Value : null;
//        }

//        static DataType getDataType(PropertyInfo value)
//        {
//            return value.PropertyType switch
//            {
//                { } type when type == typeof(string) => DataType.String,
//                { } type when type == typeof(short) => DataType.Int16,
//                { } type when type == typeof(ushort) => DataType.UInt16,
//                { } type when type == typeof(int) => DataType.Int32,
//                { } type when type == typeof(uint) => DataType.UInt32,
//                { } type when type == typeof(long) => DataType.Int64,
//                { } type when type == typeof(ulong) => DataType.UInt64,
//                { } type when type == typeof(float) => DataType.Single,
//                { } type when type == typeof(double) => DataType.Double,
//                { } type when type == typeof(DateTime) => DataType.DateTime,
//                { } type when type == typeof(bool) => DataType.Boolean,
//                { IsEnum: true } => DataType.Enum,
//                _ => DataType.String
//            };
//        }

//        static string[] getAvailableValues(PropertyInfo value)
//        {
//            return value.PropertyType.IsEnum ? Enum.GetNames(value.PropertyType) : [];
//        }
//    }
//}