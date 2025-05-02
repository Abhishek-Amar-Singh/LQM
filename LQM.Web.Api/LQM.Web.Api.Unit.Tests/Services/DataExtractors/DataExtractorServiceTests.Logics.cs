using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LQM.Web.Api.Unit.Tests.Services.DataExtractors
{
    public partial class DataExtractorServiceTests
    {
        private IFormFile CreateValidCSVFile()
        {
            var csvBuilder = "ISIN,CFICode,Venue,AlgoParams\n" +
                "US123456,CFI123,NASDAQ,InstIdentCode:DE000C4SA5W8|;InstFullName:DAX|;InstClassification:FFICSX|;NotionalCurr:EUR|;PriceMultiplier:25.0|;UnderlInstCode:DE0008469008|;UnderlIndexName:DAX PERFORMANCE-INDEX|;OptionType:OTHR|;StrikePrice:0.0|;OptionExerciseStyle:|;ExpiryDate:2020-09-18|;DeliveryType:PHYS|";
            
            var bytes = Encoding.UTF8.GetBytes(csvBuilder);
            var ms = new MemoryStream(bytes);
            var fileMock = new Mock<IFormFile>();

            fileMock.Setup(f => f.OpenReadStream()).Returns(() => new MemoryStream(bytes));
            fileMock.Setup(f => f.FileName).Returns("test.csv");
            fileMock.Setup(f => f.Length).Returns(bytes.Length);
            fileMock.Setup(f => f.ContentType).Returns("text/csv");

            return fileMock.Object;
        }
    }
}
