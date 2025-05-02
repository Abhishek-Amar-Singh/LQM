using LQM.Web.Api.Models.DataExtractors.Exceptions;

namespace LQM.Web.Api.Services.DataExtractors
{
    public partial class DataExtractorService
    {
        private void ValidateIfBankNameNotProvided(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new MyCustomException("Invalid bank name");
            }
        }

        private void ValidateIfFileProvided(IFormFile file)
        {
            if (file is null || file.Length == 0)
            {
                throw new MyCustomException("File is not provided");
            }
        }

        private (string, string) ValidateFileContentTypeAndExtension(IFormFile file, string? extension = null)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            var contentType = file.ContentType;

            if (fileExtension is null ||
                !fileExtension.StartsWith('.') ||
                contentType is null)
            {
                throw new MyCustomException("Invalid file extension or content type");
            }


            if (extension is not null && fileExtension != extension)
            {
                throw new MyCustomException("Invalid file extension");
            }

            return (fileExtension, contentType);
        }

        private async ValueTask ValidateIfDataIsCorruptInCSVFileAsync(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                string headerLine = (await reader.ReadLineAsync()) ?? string.Empty;
                if (string.IsNullOrWhiteSpace(headerLine))
                    throw new MyCustomException("The CSV file has no header");

                var delimiter = ',';
                var headerColumns = headerLine.Split(delimiter);
                int expectedColumns = headerColumns.Length;

                int lineNumber = 1;

                while (!reader.EndOfStream)
                {
                    string line = (await reader.ReadLineAsync()) ?? string.Empty;
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    var columns = line.Split(delimiter);

                    if (columns.Length != expectedColumns)
                    {
                        throw new MyCustomException($"Corrupted data detected at line {lineNumber} in the CSV file: expected {expectedColumns} columns, but found {columns.Length}");
                    }
                }
            }
        }
    }
}
