using System.Text;
using LQM.Web.Api.Services.DataExtractors;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LQM.Web.Api.Unit.Tests.Services.DataExtractors
{
    public partial class DataExtractorServiceTests
    {
        private readonly DataExtractorService _dataExtractorService;

        public DataExtractorServiceTests() =>
            _dataExtractorService = new DataExtractorService();

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturnFileContent_WhenInputIsValid()
        {
            string bankName = "Barclays";
            
            var fileMockObj = CreateValidCSVFile();

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, fileMockObj);

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
            string bankName = string.Empty;//--Invalid bank name

            var file = CreateValidCSVFile();

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, file);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturn400_WhenFileIsNull()
        {
            string bankName = "StanChart";
            IFormFile? file = null;//--File not provided

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, file!);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturn400_WhenFileExtensionIsInvalid()
        {
            string bankName = "Union Bank";
            var csvBuilder = "ISIN,CFICode,Venue,AlgoParams\n" +
                "US123456,CFI123,NASDAQ," +
                "InstIdentCode:DE000C4SA5W8|;InstFullName:DAX|;InstClassification:FFICSX|;NotionalCurr:EUR|;PriceMultiplier:25.0|;UnderlInstCode:DE0008469008|;UnderlIndexName:DAX PERFORMANCE-INDEX|;OptionType:OTHR|;StrikePrice:0.0|;OptionExerciseStyle:|;ExpiryDate:2020-09-18|;DeliveryType:PHYS|";
            
            var bytes = Encoding.UTF8.GetBytes(csvBuilder);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));
            fileMock.Setup(f => f.FileName).Returns("test.txt");//--Invalid file extension
            fileMock.Setup(f => f.Length).Returns(bytes.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/plain");

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, fileMock.Object);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturn400_WhenCSVIsCorrupt()
        {
            string bankName = "SBI";
            var csvBuilder = "ISIN,CFICode,Venue,AlgoParams\n" +
                "US123456,CFI123,NASDAQ," +
                "InstIdentCode:DE000C4SA5W8|;InstFullName:DAX|;InstClassification:FFICSX|;NotionalCurr:EUR|;PriceMultiplier:25.0|;UnderlInstCode:DE0008469008|;UnderlIndexName:DAX PERFORMANCE-INDEX|;OptionType:OTHR|;StrikePrice:0.0|;OptionExerciseStyle:|;ExpiryDate:2020-09-18|;DeliveryType:PHYS|," +
                "seesharp";//--Corrupt Data In CSV File

            var bytes = Encoding.UTF8.GetBytes(csvBuilder);
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(bytes.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, fileMock.Object);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }

        [Fact]
        public async Task ExtractCSVAsync_ShouldReturn500_WhenUnhandledExceptionOccurs()
        {
            string bankName = "BankOfBaroda";

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.ContentType).Returns("text/csv");
            fileMock.Setup(f => f.Length).Returns(100);
            fileMock.Setup(f => f.OpenReadStream()).Throws(new IOException("Disk error"));//--Unhandled exception occurs while opening the request stream for reading the uploaded file

            var result = await _dataExtractorService.ExtractCSVAsync(bankName, fileMock.Object);

            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal("Failed", result.Message);
            Assert.Null(result.Data);
            Assert.True(result.Errors?.Any());
        }
    }
}
