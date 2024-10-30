// ReSharper disable CheckNamespace
#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class PhasorDetail
{
    [PrimaryKey(true)]
    public int ID { get; set; }

    [ParentKey(typeof(Device))]
    public int DeviceID { get; set; }

    public string DeviceAcronym { get; set; } = "";

    public string DeviceName { get; set; } = "";

    [Required]
    public string Label { get; set; } = "";

    [Required]
    public string? Type { get; set; }

    [Required]
    public string? Phase { get; set; }

    public int SourceIndex { get; set; }

    public int? DestinationPhasorID { get; set; }

    public string? DestinationPhasorLabel { get; set; }

    public int BaseKV { get; set; }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    [StringLength(50)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string? CreatedBy { get; set; }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    //[UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    [StringLength(50)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    //[UpdateValueExpression("UserInfo.CurrentUserID")]
    public string? UpdatedBy { get; set; }
}