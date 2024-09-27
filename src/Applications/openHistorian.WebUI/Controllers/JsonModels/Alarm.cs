namespace openHistorian.WebUI.Controllers.JsonModels;

public class Alarm
{
    public int ID { get; set; }
    public string name { get; set; }
    public SeverityLevel severity { get; set; }
    public DateTime timestamp { get; set; }
}