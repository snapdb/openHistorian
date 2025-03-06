// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel;
using Gemstone.Expressions.Model;
using Gemstone.Numeric.EE;

namespace openHistorian.Model;

public class FrequencyDefinition
{
    public string Label { get; set; }
}

public class PhasorDefinition
{
    public int ID { get; set; }

    public string Label { get; set; }

    public string PhasorType { get; set; }

    public string Phase { get; set; }

    public int? DestinationPhasorID { get; set; }

    public int? NominalVoltage { get; set; }

    public int SourceIndex { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime UpdatedOn { get; set; }
}

public class AnalogDefinition
{
    public string Label { get; set; }

    public string AnalogType { get; set; }
}

public class DigitalDefinition
{
    public string Label { get; set; }
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

    public FrequencyDefinition FrequencyDefinition { get; set; }

    public List<PhasorDefinition> PhasorDefinitions { get; } = [];

    public List<AnalogDefinition> AnalogDefinitions { get; } = [];

    public List<DigitalDefinition> DigitalDefinitions { get; } = [];
}

public class ConfigurationFrame
{
    public List<ConfigurationCell> Cells { get; } = [];

    public ushort IDCode { get; set; }

    public string StationName { get; set; }

    public string IDLabel { get; set; }

    //[DefaultValueExpression("Global.DefaultCalculationFramesPerSecond")]
    [DefaultValue(30)]
    public ushort FrameRate { get; set; }

    public string ConnectionString { get; set; }

    public int ProtocolID { get; set; }

    public bool IsConcentrator { get; set; }
}