using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using Hedger;
using System.Text.Json;
using System.Text.Json.Serialization;
using PricingLibrary.RebalancingOracleDescriptions;

internal class Handler
{
    static public void Main(string[] args)
    {
        List<DataFeed> dataFeedList;

        if (args.Length != 3) throw new ArgumentException("Specify json,csv and output ");

        dataFeedList = DataGenerator.dataFeedGenerator(args[1]);
        var basketSample = DataGenerator.basketOptionGenerator(args[0]);

        if (basketSample == null || dataFeedList.Count == 0) throw new ArgumentException("json/csv is empty");

        List<OutputData> outputData = Strategy.getOuputData(dataFeedList, basketSample);

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(), new RebalancingOracleDescriptionConverter() }
        };

        string outputFile = JsonSerializer.Serialize(outputData, options);

        File.WriteAllText(args[2], outputFile);
        
    }
}