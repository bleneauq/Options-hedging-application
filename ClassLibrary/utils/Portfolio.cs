using PricingLibrary;
using PricingLibrary.Computations;
using PricingLibrary.DataClasses;
using PricingLibrary.MarketDataFeed;
using PricingLibrary.RebalancingOracleDescriptions;
using PricingLibrary.TimeHandler;
using System.Diagnostics;

namespace Hedger
{
    class Portfolio
    {

        public Dictionary<string, double> shareQuantity
        { get; set; }
        public double cash
        { get; set; }
        public double optionPrice
        { get; set; }

        public double[] deltas { get; set; }
        public double[] detalsStdDev { get; set; }
        public double optionPriceStdDev { get; set; }



        public DateTime lastRebalancingDay { get; set; }

        public Portfolio(Dictionary<string, double> s, double c, DateTime lR)
        {
            shareQuantity = s;
            cash = c;
            lastRebalancingDay = lR;
        }

        public static Portfolio initalizePortfolio(BasketTestParameters basketOption, Dictionary<string, double> sharesPrice, DateTime simulationDate)
        {

            Pricer p = new Pricer(basketOption);
            Double timeToMaturity = MathDateConverter.ConvertToMathDistance(simulationDate, basketOption.BasketOption.Maturity);

            double[] shareValueMarket = new double[basketOption.BasketOption.UnderlyingShareIds.Length];
            string[] underlyingShareID = basketOption.BasketOption.UnderlyingShareIds;
            int index = 0;
            foreach (string share in underlyingShareID)
            {
                shareValueMarket[index] = sharesPrice[share];
                index++;
            }

            PricingResults pR = p.Price(timeToMaturity, shareValueMarket);

            Dictionary<string, double> sharesQuantity = new Dictionary<string, double>();
            foreach (var key in sharesPrice.Keys.ToList())
            {
                sharesQuantity[key] = 0;
            }
            return new Portfolio(sharesQuantity, pR.Price, simulationDate);

        }

        public Double getPortfolioValue(DateTime simulationDate, Dictionary<string, double> sharesPrice)
        {
            
            Double portfolioValue = 0;
            foreach (var key in shareQuantity.Keys.ToList())
            {
                portfolioValue += shareQuantity[key] * sharesPrice[key];

            }
            return portfolioValue + cash;
        }

        public void rebalance(Dictionary<string, double> sharesPrice, DateTime rebalanceDate, BasketTestParameters basketOption)
        {

            cash = cash * RiskFreeRateProvider.GetRiskFreeRateAccruedValue(lastRebalancingDay, rebalanceDate);

            Pricer p = new Pricer(basketOption);
            double timeToMaturity = MathDateConverter.ConvertToMathDistance(rebalanceDate, basketOption.BasketOption.Maturity);


            
            double[] shareValueMarket = new double[basketOption.BasketOption.UnderlyingShareIds.Length];
            string[] underlyingShareID = basketOption.BasketOption.UnderlyingShareIds;
            int index = 0;
            foreach (string share in underlyingShareID)
            {
                shareValueMarket[index] = sharesPrice[share];
                index++;
            }


            PricingResults pR = p.Price(timeToMaturity, shareValueMarket);
            double[] delta = pR.Deltas;

           
            Dictionary<string, double> dicSharesDelta = sharesPrice.Keys.Zip(delta, (k, v) => new { k, v })
              .ToDictionary(x => x.k, x => x.v);

            double cashPrice = 0;

            
            foreach (var share in shareQuantity.Keys.ToList())
            {

                double numberOfShareToBuy = -shareQuantity[share] + dicSharesDelta[share]; // to buy 
                cashPrice += -sharesPrice[share] * numberOfShareToBuy;
                shareQuantity[share] = dicSharesDelta[share];
            }
            cash = cash + cashPrice;
            optionPrice = pR.Price;
            lastRebalancingDay = rebalanceDate;
            deltas = delta;
            detalsStdDev = pR.DeltaStdDev;
            optionPriceStdDev = pR.PriceStdDev;
        }
    }



}
