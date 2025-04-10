using ServiceInterface;

namespace openHistorian.WebUI.Controllers.JsonModels;

public class SystemStatus
{
    public string status { get; set; }
    public ServiceStatus type { get; set; }
    public string description { get; set; }
}