using Google.Protobuf.WellKnownTypes;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using System.Linq.Expressions;

namespace GrpcBacktestServer.utils
{
    public class outputProtoConverter
    {

       /* message BacktestOutput
            {
                repeated BacktestInfo backtest_info = 1;
            }

        message BacktestInfo
        {
            google.protobuf.Timestamp date = 1;

        double portfolio_value = 2;
            repeated double delta = 3;
            repeated double delta_stddev = 4;
            double price = 5;
            double price_stddev = 6;
        }
        */

    static public BacktestOutput convertOutputDataToProto(List<OutputData> outputData)
        {

            BacktestOutput backtestOutput = new BacktestOutput();
            foreach (OutputData data in outputData)
            {
                int deltasCount = data.Deltas.Length;
                double[] deltas = new double[deltasCount];
                double[] deltas_stddev = new double[deltasCount];
                for (var i = 0; i < data.Deltas.Length; i++)
                {
                    deltas[i] = data.Deltas[i];
                    deltas_stddev[i] = data.DeltasStdDev[i];
                }
                BacktestInfo backtestInfo = new BacktestInfo
                {
                    Date = Timestamp.FromDateTime(DateTime.SpecifyKind(data.Date, DateTimeKind.Utc)),
                    PortfolioValue = data.Value,
                    Price = data.Price,
                    PriceStddev = data.PriceStdDev
                    
                };
                backtestInfo.Delta.AddRange(deltas);
                backtestInfo.DeltaStddev.AddRange(deltas_stddev);
                backtestOutput.BacktestInfo.Add(backtestInfo);
            }

            return backtestOutput;
        }
    }
}