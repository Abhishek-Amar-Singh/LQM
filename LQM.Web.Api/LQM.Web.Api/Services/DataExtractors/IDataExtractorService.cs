
using LQM.Web.Api.Models;
using Shared.Space.Lib.Models;

namespace LQM.Web.Api.Services.DataExtractors
{
    public interface IDataExtractorService
    {
        ValueTask<Response<FileContentRes>> MethodAsync(string bankName, IFormFile file);
    }
}
