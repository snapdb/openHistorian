using System.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using openHistorian.Model;

namespace openHistorian.Adapters.Model;
public class IaonFilterAdapter : IIaonAdapter
{
    [PrimaryKey(true)]
    public int ID { get; set; }

    [Required]
    [StringLength(200)]
    public string AdapterName { get; set; }

    [Required]
    public string AssemblyName { get; set; }

    [Required]
    [StringLength(200)]
    public string TypeName { get; set; }

    public string ConnectionString { get; set; }
}
