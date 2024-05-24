using CsvHelper;
using Google.Protobuf.WellKnownTypes;
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

namespace GrpcBacktestServer.utils
{
    public class inputProtoConverter
    {
        static public BasketTestParameters convertProtoToBasket(BacktestRequest request)
        {

            BasketTestParameters basketSample = new BasketTestParameters();
            Basket basket = new Basket();
            BasketPricingParameters basketPricing = new BasketPricingParameters();

            basket.Maturity = request.TstParams.BasketParams.Maturity.ToDateTime();
            basket.Strike = request.TstParams.BasketParams.Strike;

            int shareCount = request.TstParams.BasketParams.ShareIds.Count;
            double[] weights = new double[shareCount];
            double[] vols = new double[shareCount];
            string[] ids = new string[shareCount];
            double[][] corrs = new double[shareCount][];
            for (int i = 0; i < shareCount; i++)
            {
                ids[i] = request.TstParams.BasketParams.ShareIds[i];
                weights[i] = request.TstParams.BasketParams.Weights[i];
                vols[i] = request.TstParams.PriceParams.Vols[i];
                double[] corrsLine = new double[shareCount];
                for (int j = 0; j < shareCount; j++)
                {
                    corrsLine[j] = request.TstParams.PriceParams.Corrs[i].Value[j];
                }
                corrs[i] = corrsLine;
            }
            basketPricing.Volatilities = vols;
            basketPricing.Correlations = corrs;

            basket.Weights = weights;
            basket.UnderlyingShareIds = ids;

            IRebalancingOracleDescription rebalancingOracleDescription = getRebalancingOracle(request);

            basketSample.BasketOption = basket;
            basketSample.PricingParams = basketPricing;
            basketSample.RebalancingOracleDescription = rebalancingOracleDescription;

            return basketSample;
        }

        static public List<DataFeed> convertProtoToDataFeed(BacktestRequest request)
        {
            int dataCount = request.Data.DataValues.Count;

            List<DataFeed> dataFeedList = request.Data.DataValues.GroupBy(d => d.Date,
                  t => new { Symb = t.Id.Trim(), Val = t.Value },
                  (key, g) => new DataFeed(key.ToDateTime(), g.ToDictionary(e => e.Symb, e => e.Val))).ToList();

            return dataFeedList;
        }

        static public IRebalancingOracleDescription getRebalancingOracle(BacktestRequest request)
        {
            switch (request.TstParams.RebParams.RebTypeCase)
            {
                case RebalancingParams.RebTypeOneofCase.Regular:
                    RegularOracleDescription regularRebalancing = new RegularOracleDescription();
                    regularRebalancing.Period = request.TstParams.RebParams.Regular.Period;
                    return regularRebalancing;
                case RebalancingParams.RebTypeOneofCase.Weekly:
                    WeeklyOracleDescription weeklyRebalancing = new WeeklyOracleDescription();
                    weeklyRebalancing.RebalancingDay = (DayOfWeek)(request.TstParams.RebParams.Weekly.Day);
                    return weeklyRebalancing;
                default:
                    throw new ArgumentException("Unknown type of rebalancing");
            }
        }
    }
}
