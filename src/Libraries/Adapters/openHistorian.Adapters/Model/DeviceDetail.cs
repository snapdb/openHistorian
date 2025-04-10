namespace openHistorian.Model;
public class DeviceDetail : Device
{
    public string CompanyName { get; set; } = "";

    public string CompanyAcronym { get; set; } = "";

    public string CompanyMapAcronym { get; set; } = "";

    public string HistorianAcronym { get; set; } = "";  

    public string VendorAcronym { get; set; } = "";

    public string VendorDeviceName { get; set; } = "";

    public string ProtocolName { get; set; } = "";

    public string ProtocolType { get; set; } = "";

    public string Category { get; set; } = "";

    public string InterconnectionName { get; set; } = "";

    public string ParentAcronym { get; set; } = "";
}
