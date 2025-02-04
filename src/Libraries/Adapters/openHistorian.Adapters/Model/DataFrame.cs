// ReSharper disable CheckNamespace
#pragma warning disable 1591

namespace openHistorian.Model;

public class DataCell
{
    public ushort IDCode { get; set; }

    public double Frequency { get; set; }

    public double DfDt { get; set; }

    public ushort StatusFlags { get; set; }

    public (double angle, double magnitude)[] Phasors { get; set; } = [];

    public double[] Analogs { get; set; } = [];

    public ushort[] Digitals { get; set; } = [];
}

public class DataFrame
{
    public ushort IDCode { get; set; }

    public DateTime Timestamp { get; set; }

    public uint QualityFlags { get; set; }

    public List<DataCell> Cells { get; set; } = [];
}
