#pragma warning disable 1591

using System.ComponentModel.DataAnnotations;
using Gemstone.ComponentModel.DataAnnotations;
using Gemstone.Data.Model;
using Gemstone.Expressions.Model;

namespace openHistorian.Model;

public class VendorDeviceDetail : VendorDevice
{

    [Required]
    [StringLength(200)]
    [AcronymValidation]
    public string VendorAcronym { get; set; }

    [Required]
    [StringLength(200)]
    public string VendorName { get; set; }
}