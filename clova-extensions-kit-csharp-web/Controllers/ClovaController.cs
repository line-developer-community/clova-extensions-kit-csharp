using LineDC.CEK.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace LineDC.CEK.Controllers
{
    [Route("api/[controller]")]
    public class ClovaController : Controller
    {
        private ICEKClient client;

        public ClovaController(ICEKClient client)
        {
            this.client = client;
        }

        // POST api/clova
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var response = await client.HandleRequestAsync(Request.Headers["SignatureCEK"], Request.Body, true);
            return new OkObjectResult(response);
        }
    }
}
