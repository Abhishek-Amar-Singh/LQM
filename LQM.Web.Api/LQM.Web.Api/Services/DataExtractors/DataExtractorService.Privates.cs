using System.Text;
using LQM.Web.Api.Models.DataExtractors;
using LQM.Web.Api.Models.DataExtractors.Exceptions;

namespace LQM.Web.Api.Services.DataExtractors
{
    public partial class DataExtractorService
    {
        private async ValueTask<IDictionary<string, List<string>>> ConvertCSVToDictAsync(IFormFile file)
        {
            int iterRow = 1;
            string[] headers = Array.Empty<string>();
            IDictionary<string, List<string>> headerAsKeyDataAsValsDict =
                new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream)
            {
                var line = (await reader.ReadLineAsync()) ?? string.Empty;
                var values = line.Split(',');

                if (iterRow == 1)
                {
                    if (line == string.Empty)
                    {
                        throw new MyCustomException("No headers found");
                    }

                    headers = this.MakeHeadersUnique(values);
                    foreach (var header in headers)
                    {
                        headerAsKeyDataAsValsDict[header] = new List<string>();
                    }
                }

                iterRow++;

                for (int i = 0; i < headers.Length; i++)
                {
                    string value = i < values.Length ? values[i] : string.Empty;
                    headerAsKeyDataAsValsDict[headers[i]].Add(value);
                }
            }

            return headerAsKeyDataAsValsDict;
        }

        private string[] MakeHeadersUnique(string[] headers)
        {
            var hs = new HashSet<string>();

            for (var i = 0; i < headers.Length; i++)
            {
                if (!hs.Add(headers[i]))
                {
                    headers[i] = $"{headers[i]}ColNo{i + 1}";
                }
            }

            return headers;
        }

        private IDictionary<string, List<string>> ParseComplexFieldToExtractSubField(
            IDictionary<string, List<string>> dict,
            string existingHeader,
            string[] separators,
            string subFieldKeyToFind,
            char subFieldKeyValSeparator,
            string? newHeaderName = null)
        {
            newHeaderName = newHeaderName ?? existingHeader;

            var values = dict[existingHeader];

            dict.Add(newHeaderName, new());

            for (int i = 0; i < values.Count; i++)
            {
                string? priceValue = i == 0 ? newHeaderName : values[i]
                    .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(p =>
                    p.StartsWith($"{subFieldKeyToFind}{subFieldKeyValSeparator}"))?.Split(subFieldKeyValSeparator)[1];

                dict[newHeaderName].Add(priceValue ?? string.Empty);
            }

            return dict;
        }

        private string ConvertDictToCSVStr(IDictionary<string, List<string>> dict, string[] headers)
        {
            StringBuilder csvBuilder = new();

            int maxRows = dict[headers[0]].Count;

            for (int i = 0; i < maxRows; i++)
            {
                var row = new List<string>();
                foreach (var key in headers)
                {
                    var list = dict[key];
                    row.Add(list[i]);
                }
                csvBuilder.AppendLine(string.Join(",", row));
            }

            return csvBuilder.ToString();
        }

        private string RemoveDuplicateRowsFromCSVStr(string csvData, string[] headers)
        {
            var lines = csvData.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var rows = lines.Skip(1)
                .Select(line => line.Split(','))
                .Select(values => headers.Zip(values, (h, v) => new { h, v })
                .ToDictionary(x => x.h, x => x.v)).ToList();
            var rowsToModelArr = rows
                .Select(r => new OutputSelectedData
                {
                    ISIN = r[headers[0]],
                    CFICode = r[headers[1]],
                    Venue = r[headers[2]],
                    ContractSize = r[headers[3]]
                }).ToList();

            var distinctItems = rowsToModelArr
                .GroupBy(x => new { x.ISIN, x.CFICode, x.Venue, x.ContractSize })
                .Select(g => g.First())
                .ToList();

            StringBuilder csvBuilder = new();
            csvBuilder.AppendLine($"{headers[0]},{headers[1]},{headers[2]},{headers[3]}");
            foreach (var row in distinctItems)
            {
                csvBuilder.AppendLine($"{row.ISIN},{row.CFICode},{row.Venue},{row.ContractSize}");
            }

            return csvBuilder.ToString();
        }
    }
}
