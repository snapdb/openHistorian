// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class Measurement
{
    [Label("Point ID")]
    [PrimaryKey(true)]
    public int PointID { get; set; }

    [Label("Unique Signal ID")]
    [DefaultValueExpression("Guid.NewGuid()")]
    public Guid SignalID { get; set; }

    public int? HistorianID { get; set; }

    [ParentKey(typeof (Device))]
    public int? DeviceID { get; set; }

    [Label("Tag Name")]
    [Required]
    [StringLength(200)]
    public string PointTag { get; set; } = "";

    [Label("Alternate Tag Name")]
    public string AlternateTag { get; set; } = "";

    [Label("Signal Type")]
    public int SignalTypeID { get; set; }

    [Label("Phasor Source Index")]
    public int? PhasorSourceIndex { get; set; }

    [Label("Signal Reference")]
    [Required]
    [StringLength(200)]
    public string SignalReference { get; set; } = "";

    [DefaultValue(0.0D)]
    public double Adder { get; set; }

    [DefaultValue(1.0D)]
    public double Multiplier { get; set; }

    public string Description { get; set; } = "";

    public bool Internal { get; set; }

    public bool Subscribed { get; set; }

    public bool Enabled { get; set; }

    /// <summary>
    /// Created on field.
    /// </summary>
    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Created by field.
    /// </summary>
    [Required]
    [StringLength(200)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy { get; set; }

    /// <summary>
    /// Updated on field.
    /// </summary>
    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    /// <summary>
    /// Updated by field.
    /// </summary>
    [Required]
    [StringLength(200)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy { get; set; }
}