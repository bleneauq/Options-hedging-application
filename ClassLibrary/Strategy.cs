using CsvHelper;
using Hedger;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.RebalancingOracleDescriptions;
using PricingLibrary.TimeHandler;
using PricingLibrary.Utilities;
using System.Drawing.Printing;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Hedger
{
    public class Strategy
    {
        static public List<OutputData> getOuputData(List<DataFeed> dataFeedList, BasketTestParameters basketSample)
        {
            DateTime firstDate = dataFeedList[0].Date;
            Dictionary<string, double> firstPrices = dataFeedList[0].PriceList;
            var portfolio = Portfolio.initalizePortfolio(basketSample, firstPrices, firstDate);

            RebalancingOracleType typeRebalance = basketSample.RebalancingOracleDescription.Type;
            Oracle oracleBuild = OracleFactory.buildOracle(typeRebalance, basketSample, firstDate);
            portfolio.rebalance(firstPrices, firstDate, basketSample);

            List<OutputData> outputDatas = new List<OutputData>();
            outputDatas.Add(OutputDataManager.generateOutputData(firstDate, portfolio.getPortfolioValue(firstDate, firstPrices), portfolio.optionPrice, portfolio.deltas, portfolio.detalsStdDev, portfolio.optionPriceStdDev));

            foreach (DataFeed dataFeed in dataFeedList.Skip(1))
            {
                DateTime dateOfPrice = dataFeed.Date;
                Dictionary<string, double> sharePrices = dataFeed.PriceList;

                if (oracleBuild.isRebalancing(dateOfPrice))
                {
                    portfolio.rebalance(sharePrices, dateOfPrice, basketSample);
                    outputDatas.Add(OutputDataManager.generateOutputData(dateOfPrice, portfolio.getPortfolioValue(dateOfPrice, sharePrices), portfolio.optionPrice, portfolio.deltas, portfolio.detalsStdDev, portfolio.optionPriceStdDev));
                }
            }
            return outputDatas;
        }
    }
}


