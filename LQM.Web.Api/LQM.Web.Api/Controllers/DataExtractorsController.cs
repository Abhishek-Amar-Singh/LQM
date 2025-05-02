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

        [HttpPost]
        [Route("ExtractCSVAsync")]
        public async ValueTask<ActionResult> ExtractCSVAsync(string bankName, IFormFile file)
        {
            var response = await this._dataExtractorService.ExtractCSVAsync(bankName, file);

            return response.StatusCode switch
            {
                StatusCodes.Status200OK => File(
                    response.Data!.Data, 
                    response.Data.ContentType, 
                    $"{bankName}_Output_At_{DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:sszzz")}{response.Data.Extension}"),
                _ => StatusCode(response.StatusCode, response)
            };
        }
    }
}
