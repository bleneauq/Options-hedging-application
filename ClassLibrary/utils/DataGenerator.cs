using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using PricingLibrary.MarketDataFeed;
using CsvHelper;
using System.Globalization;

namespace Hedger
{
    public class DataGenerator
    {
        public static BasketTestParameters? basketOptionGenerator(String jsonPATH)
        {
            if (jsonPATH != null)
            {

                // Initialisation Options Tests parameters 
                var json = File.ReadAllText(jsonPATH);

                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(), new RebalancingOracleDescriptionConverter() }
                };

                var basketSample = JsonSerializer.Deserialize<BasketTestParameters>(json, options);

                return basketSample;
            }
            else
            {
                Console.WriteLine("ERROR : JsonPATH is null ");
                return null;
            }
        }

        public static List<DataFeed> dataFeedGenerator(String csvPATH)
        {

            if (csvPATH != null)
            {
                var reader = new StreamReader(csvPATH);
                var readerCsv = new CsvReader(reader, CultureInfo.InvariantCulture);

                var MarketData = readerCsv.GetRecords<ShareValue>().ToList();

                List<DataFeed> dataFeedList = MarketData.GroupBy(d => d.DateOfPrice,
                      t => new { Symb = t.Id.Trim(), Val = t.Value },
                      (key, g) => new DataFeed(key, g.ToDictionary(e => e.Symb, e => e.Val))).ToList();
                return dataFeedList;
            }
            else
            {
                Console.WriteLine("ERROR : CSVPath is null and isSample is false ");
                return new List<DataFeed>();
            }

        }
    }
}
