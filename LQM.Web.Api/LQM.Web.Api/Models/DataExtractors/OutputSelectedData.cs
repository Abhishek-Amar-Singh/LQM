namespace LQM.Web.Api.Models.DataExtractors
{
    public class OutputSelectedData
    {
        public string ISIN { get; set; } = string.Empty;
        public string CFICode { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public string ContractSize { get; set; } = string.Empty;
    }
}
