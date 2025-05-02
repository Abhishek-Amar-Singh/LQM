using System.Text;
using LQM.Web.Api.Models;
using LQM.Web.Api.Models.DataExtractors;
using LQM.Web.Api.Models.DataExtractors.Exceptions;
using Shared.Space.Lib.Models;

namespace LQM.Web.Api.Services.DataExtractors
{
    public partial class DataExtractorService : IDataExtractorService
    {
        public async ValueTask<Response<FileContentRes>> MethodAsync(string bankName, IFormFile file)
        {
            try
            {
                ValidateIfBankNameNotProvided(bankName);
                ValidateIfFileProvided(file);
                (string fileExtension, string fileContentType) = ValidateFileContentTypeAndExtension(file, ".csv");
                await ValidateIfDataIsCorruptInCSVFileAsync(file);

                var headerAsKeyDataAsValsDict = await this.ConvertCSVToDictAsync(file);

                var outputHeaders = new string[4] {
                    nameof(OutputSelectedData.ISIN),
                    nameof(OutputSelectedData.CFICode),
                    nameof(OutputSelectedData.Venue),
                    nameof(OutputSelectedData.ContractSize),
                };

                headerAsKeyDataAsValsDict = this.ParseComplexFieldToExtractSubField(
                    headerAsKeyDataAsValsDict,
                    "AlgoParams",
                    new[] { "|;" },
                    "PriceMultiplier",
                    ':',
                    outputHeaders[3]);

                var csvData = this.ConvertDictToCSVStr(headerAsKeyDataAsValsDict, outputHeaders);

                var csvDataWithoutDuplicates = this.RemoveDuplicateRowsFromCSVStr(csvData, outputHeaders);

                var fileInBytes = Encoding.UTF8.GetBytes(csvDataWithoutDuplicates);

                return new()
                {
                    Data = new()
                    {
                        Data = fileInBytes,
                        Extension = fileExtension,
                        ContentType = fileContentType
                    }
                };
            }
            catch (MyCustomException myCustomEx)
            {
                return new()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Failed",
                    Errors = new List<string>() { myCustomEx.Message }
                };
            }
            //--Add other custom exceptions
            catch (Exception ex)
            {
                return new()
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    Message = "Failed",
                    Errors = new List<string>() { "Something went wrong", ex.Message }
                };
            }
        }
    }
}
