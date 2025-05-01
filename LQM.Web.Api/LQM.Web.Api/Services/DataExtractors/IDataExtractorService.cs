
using Shared.Space.Lib.Models;

namespace LQM.Web.Api.Services.DataExtractors
{
    public interface IDataExtractorService
    {
        ValueTask<Response<byte[]>> MethodAsync(IFormFile file);
    }
}
