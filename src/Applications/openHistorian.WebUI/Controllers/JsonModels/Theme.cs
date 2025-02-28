using Gemstone.Data.Model;
using Gemstone.Expressions.Model;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.WebUI.Controllers.JsonModels;

public class Theme
{
    [PrimaryKey(true)]
    [DefaultValueExpression("-1")]
    public int ID { get; set; }

    public string FileName { get; set; }

    public string Name { get; set; }

    [DefaultValueExpression("0")]
    public int LoadOrder { get; set; }

    [DefaultValueExpression("DateTime.UtcNow")]
    public DateTime CreatedOn { get; set; }

    [DefaultValueExpression("UserInfo.CurrentUserID")]
    public string CreatedBy { get; set; }

    [DefaultValueExpression("this.CreatedOn", EvaluationOrder = 1)]
    [UpdateValueExpression("DateTime.UtcNow")]
    public DateTime UpdatedOn { get; set; }

    [DefaultValueExpression("this.CreatedBy", EvaluationOrder = 1)]
    [UpdateValueExpression("UserInfo.CurrentUserID")]
    public string UpdatedBy { get; set; }

}