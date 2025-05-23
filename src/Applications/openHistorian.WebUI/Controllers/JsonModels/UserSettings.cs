using Gemstone.Data.Model;
using Gemstone.Expressions.Model;
using System.ComponentModel.DataAnnotations;

namespace openHistorian.WebUI.Controllers.JsonModels;

public class UserSettings
{
    [PrimaryKey(true)]
    public int ID { get; set; }

    public string UserID { get; set; }

    public string AuthProviderID { get; set; }

    public string Settings { get; set; }

}