using System.Net.Mime;
using System.Text;
using LQM.Web.Api.Services.DataExtractors;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LQM.Web.Api.Unit.Tests.Services.DataExtractors
{
    public partial class DataExtractorServiceTests
    {
        private readonly DataExtractorService _service;

        public DataExtractorServiceTests() =>
            _service = new DataExtractorService();

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturnFileContent_WhenInputIsValid()
        {
            string bankName = "Barclays";
            
            var fileMockObj = CreateValidCSVFile();

            var result = await _service.ExtractCSVAsync(bankName, fileMockObj);

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal("Success", result.Message);
            Assert.Null(result.Errors);
            Assert.NotNull(result.Data);
            Assert.Equal(typeof(byte[]), result.Data.Data.GetType());
            Assert.Equal(".csv", result.Data.Extension);
            Assert.Equal("text/csv", result.Data.ContentType);
        }

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturn400_WhenBankNameIsEmpty()
        {
            string bankName = string.Empty;

            var file = CreateValidCSVFile();

            var result = await _service.ExtractCSVAsync(bankName, file);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }
    }
}
