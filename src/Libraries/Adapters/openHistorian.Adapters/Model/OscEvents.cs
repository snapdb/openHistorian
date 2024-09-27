// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;

namespace openHistorian.Model;

public class OscEvents
{
    [PrimaryKey(true)]
    public int ID { get; set; }

    public int? ParentID { get; set; }

    [Label("Oscillation Source")]
    [Required]
    [StringLength(200)]
    public string Source { get; set; }

    [Label("Start Time")]
    public DateTime? StartTime { get; set; }

    [Label("Stop Time")]
    public DateTime? StopTime { get; set; }

    public double? FrequencyBand1 { get; set; }
        
    public double? FrequencyBand2 { get; set; }
        
    public double? FrequencyBand3 { get; set; }
        
    public double? FrequencyBand4 { get; set; }

    public double? TriggeringMagnitudeBand1 { get; set; }
        
    public double? TriggeringMagnitudeBand2 { get; set; }
        
    public double? TriggeringMagnitudeBand3 { get; set; }
        
    public double? TriggeringMagnitudeBand4 { get; set; }

    public double? MaximumMagnitudeBand1 { get; set; }

    public double? MaximumMagnitudeBand2 { get; set; }

    public double? MaximumMagnitudeBand3 { get; set; }

    public double? MaximumMagnitudeBand4 { get; set; }

    public double? AverageMagnitudeBand1 { get; set; }

    public double? AverageMagnitudeBand2 { get; set; }

    public double? AverageMagnitudeBand3 { get; set; }

    public double? AverageMagnitudeBand4 { get; set; }

    public string Notes { get; set; }
}