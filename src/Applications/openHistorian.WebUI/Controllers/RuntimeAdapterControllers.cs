using Gemstone.Timeseries.Adapters;
using Gemstone.Timeseries.Model;
using Gemstone.Web.APIController;
using Microsoft.AspNetCore.Mvc;
using openHistorian.Adapters.Model;
using openHistorian.Model;

namespace openHistorian.WebUI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RuntimeCustomActionAdapterController : ReadOnlyModelController<IaonActionAdapter>;

[Route("api/[controller]")]
[ApiController]
public class RuntimeCustomInputAdapterController : ReadOnlyModelController<IaonInputAdapter>;

[Route("api/[controller]")]
[ApiController]
public class RuntimeCustomOutputAdapterController : ReadOnlyModelController<IaonOutputAdapter>;

[Route("api/[controller]")]
[ApiController]
public class RuntimeCustomFilterAdapterController : ReadOnlyModelController<IaonFilterAdapter>;