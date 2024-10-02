namespace openHistorian.WebUI.Controllers.JsonModels;

public class Event
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string PointTag { get; set; }
    public string Details { get; set; }
    public string Type { get; set; }
    public Guid ID { get; set; }
}
