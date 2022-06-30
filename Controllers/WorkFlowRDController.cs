using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExternalAPIs.BI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace ExternalAPIs.Controllers
{
    [Route("api/workflowRD")]
    [ApiController]

    
    public class WorkFlowRDController : ControllerBase
    {
        private readonly IConfiguration _config;
        public WorkFlowRDController(IConfiguration configuration)
        {
          _config = configuration;

        }
        [HttpPost("CustomerGroupDetails")]
        public IActionResult CustomerGroupDetails([FromBody] JObject data)
        {
            string retVal = WorkFlowRDAPI.CustomerGroupDetails(_config, data).ToString();
            return Ok(retVal);
        }
    }

}
