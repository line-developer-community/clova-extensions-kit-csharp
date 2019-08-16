using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LineDC.CEK.Controllers
{
    [Route("api/[controller]")]
    public class ClovaController : Controller
    {
        private IClova Clova { get; }

        public ClovaController(IClova client)
        {
            Clova = client;
        }

        // POST api/clova
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var response = await Clova.RespondAsync(Request.Headers["SignatureCEK"], Request.Body);
            return new OkObjectResult(response);
        }
    }
}
