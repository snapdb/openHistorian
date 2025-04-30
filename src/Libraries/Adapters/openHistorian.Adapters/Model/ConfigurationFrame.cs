// ReSharper disable CheckNamespace
#pragma warning disable 1591

using Gemstone.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;


namespace openHistorian.Model;

public class FrequencyDefinition
{
    public string Label { get; set; }

    public string PointTag { get; set; }

    public string AlternateTag { get; set; }

    public int? SignalTypeID { get; set; }
}

public class PhasorDefinition
{
    public int ID { get; set; }

    public string Label { get; set; }

    public string PhasorType { get; set; }

    public string Phase { get; set; }

    public int? PrimaryVoltageID { get; set; }

    public int? SecondaryVoltageID { get; set; }

    public int? NominalVoltage { get; set; }

    public int SourceIndex { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }

    public bool TaggedForDelete { get; set; }

    public bool Enabled { get; set; } = true;

    [JsonIgnore]
    public bool IsVoltage => string.Compare(PhasorType, "Voltage", StringComparison.OrdinalIgnoreCase) == 0;

    [JsonIgnore]
    public bool IsCurrent => !IsVoltage;

    public PhasorDefinition? GetAssociatedVoltage(ConfigurationCell cell)
    {
        if (IsVoltage)
            return null;

        int voltageID = PrimaryVoltageID ?? SecondaryVoltageID.GetValueOrDefault();
        return voltageID == 0 ? null : cell.PhasorDefinitions.FirstOrDefault(phasor => phasor.ID == voltageID);
    }
}

public class AnalogDefinition
{
    public string PointTag { get; set; }

    public string AlternateTag { get; set; }

    public string Description { get; set; } = "";

    public double Adder { get; set; }

    public double Multiplier { get; set; }

    public string Label { get; set; }

    public string AnalogType { get; set; }

    public int? SignalTypeID { get; set; }
}

public class DigitalDefinition
{
    public string PointTag { get; set; }

    public string AlternateTag { get; set; }

    public string Description { get; set; } = "";

    public double Adder { get; set; }

    public double Multiplier { get; set; }

    public string Label { get; set; }

    public int? SignalTypeID { get; set; }
}

public class ConfigurationCell
{
    public int ID { get; set; }

    public int? ParentID { get; set; }

    public Guid? UniqueID { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Latitude { get; set; }

    public ushort IDCode { get; set; }

    public string StationName { get; set; }

    public string IDLabel { get; set; }

    public double NominalFrequency { get; set; }

    public double FramesPerSecond { get; set; }

    public FrequencyDefinition FrequencyDefinition { get; set; }

    public List<PhasorDefinition> PhasorDefinitions { get; set; } = [];

    public List<AnalogDefinition> AnalogDefinitions { get; set; } = [];

    public List<DigitalDefinition> DigitalDefinitions { get; set; } = [];

    // Device record field proxies
    [JsonIgnore]
    public string Acronym
    {
        get => ConfigurationFrame.GetCleanAcronym(string.IsNullOrWhiteSpace(IDLabel) ? StationName : IDLabel);
        set => IDLabel = value;
    }

    // If user is changing the Acronym, UI should set the OriginalAcronym field
    // to the original value so updates can be applied to the correct record.
    public string OriginalAcronym { get; set; }

    public string TimeZone { get; set; }

    public int? HistorianID { get; set; }

    public int? InterconnectionID { get; set; }

    public int? VendorDeviceID { get; set; }

    public int? CompanyID { get; set; }

    public string ContactList { get; set; }
}

public class ConfigurationFrame
{
    public List<ConfigurationCell> Cells { get; set; } = [];

    public ushort IDCode { get; set; }

    public string StationName { get; set; }

    public string IDLabel { get; set; }


    public string ConnectionString { get; set; }

    public bool IsConcentrator { get; set; }

    [JsonIgnore]
    public string Acronym
    {
        get => ConfigurationFrame.GetCleanAcronym(string.IsNullOrWhiteSpace(IDLabel) ? StationName : IDLabel);
        set => IDLabel = value;
    }

    public string TimeZone { get; set; }

    public decimal? Longitude { get; set; }

    public decimal? Latitude { get; set; }

    public int? CompanyID { get; set; }

    public int? HistorianID { get; set; }

    public int? InterconnectionID { get; set; }

    public int? VendorDeviceID { get; set; }

    public string ContactList { get; set; }

    public int LoadOrder { get; set; }

    public bool Enabled { get; set; }

    public long TimeAdjustmentTicks { get; set; }

    /// <summary>
    /// Gets a clean acronym.
    /// </summary>
    /// <param name="acronym">Acronym.</param>
    /// <returns>Clean acronym.</returns>
    public static string GetCleanAcronym(string acronym)
    {
        return Regex.Replace((acronym ?? "").ToUpperInvariant().Replace(" ", "_"), @"[^A-Z0-9\-!_\.@#\$]", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }

    /// <summary>
    /// Gets a clean point tag.
    /// </summary>
    /// <param name="pointTag">Point tag.</param>
    /// <returns>Clean point tag.</returns>
    public static string GetCleanPointTag(string pointTag)
    {
        // Remove any invalid characters from point tag
        return Regex.Replace(pointTag.ToUpperInvariant(), @"[^A-Z0-9\-\+!\:_\.@#\$]", "", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}