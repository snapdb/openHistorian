#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class VendorDeviceDetail
{
    [PrimaryKey(true)]
    public int ID
    {
        get;
        set;
    }

    public int VendorID
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    public string Name
    {
        get;
        set;
    }

    public string Description
    {
        get;
        set;
    }

    [Label("Web Page")]
    [UrlValidation]
    public string URL
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    [AcronymValidation]
    public string VendorAcronym
    {
        get;
        set;
    }

    [Required]
    [StringLength(200)]
    public string VendorName
    {
        get;
        set;
    }
    /*
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
    public string UpdatedBy { get; set; }*/
}