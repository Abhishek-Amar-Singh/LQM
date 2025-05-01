using LQM.Web.Api.Services.DataExtractors;
using Microsoft.AspNetCore.Mvc;

namespace LQM.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataExtractorsController : ControllerBase
    {
        private readonly IDataExtractorService _dataExtractorService;

        public DataExtractorsController(IDataExtractorService _dataExtractorService) =>
            this._dataExtractorService = _dataExtractorService;

        [HttpGet]
        [Route("GetData")]
        public ActionResult GetData()
        {
            return StatusCode(StatusCodes.Status200OK, "DataExtractorsController");
        }

        [HttpPost]
        [Route("MethodAsync")]
        public async ValueTask<ActionResult> MethodAsync(string bankName, IFormFile file)
        {
            var response = await this._dataExtractorService.MethodAsync(file);

            return response.StatusCode switch
            {
                StatusCodes.Status200OK => File(
                    response.Data, 
                    "text/csv", 
                    $"{bankName}_Output_Data_{DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")}.csv"),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
