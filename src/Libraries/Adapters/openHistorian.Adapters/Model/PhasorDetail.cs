// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class PhasorDetail : Phasor
{
    public string DeviceAcronym { get; set; } = "";

    public string DeviceName { get; set; } = "";

    public string? PrimaryVoltageLabel { get; set; }

    public string? SecondaryVoltageLabel { get; set; }
}