#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class VendorDevice
{
    [PrimaryKey(true)]
    public int ID { get; set; } = -1;

    public int VendorID { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    [Label("Web Page")]
    [UrlValidation]
    public string URL { get; set; } = "";

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy { get; set; } = "";

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    [Required]
    [StringLength(200)]
    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy { get; set; } = "";
}