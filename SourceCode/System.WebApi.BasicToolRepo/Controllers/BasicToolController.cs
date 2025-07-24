using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http.HttpResults;
namespace System.Webapi.BasicToolRepo
{
    [ApiVersion("1.0")]
    [Route("Costs/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BasicToolController : ControllerBase
    {
        [HttpGet("test/hello")]
        public async Task<IActionResult> TestHello([FromHeader(Name = "im-countrycode")] string countryCode,
            [FromHeader(Name = "im-companycode")] string companyCode)
        {
            await Task.FromResult(1);
            return Ok("success");
        }
    }
}
