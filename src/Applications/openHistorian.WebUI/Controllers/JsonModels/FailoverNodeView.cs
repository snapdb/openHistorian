using Gemstone.Data.Model;
using Gemstone.Expressions.Model;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.WebUI.Controllers.JsonModels;

public class FailoverNodeView
{
    [PrimaryKey(true)]
    public string SystemName { get; set; }
    public int Priority { get; set; }
    public DateTime LastLog { get; set; }
}