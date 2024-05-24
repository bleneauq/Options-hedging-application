using PricingLibrary.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedger
{
    class OutputDataManager
    {
        public static OutputData generateOutputData(DateTime date, double value, double price, double[] deltas, double[] deltasStdDev, double priceStdDev)
        {
            OutputData outputData = new OutputData();
            outputData.Date = date;
            outputData.Price = price;
            outputData.Value = value;
            outputData.Deltas = deltas;
            outputData.DeltasStdDev = deltasStdDev;
            outputData.PriceStdDev = priceStdDev;
            return outputData;
        }

    }
}
