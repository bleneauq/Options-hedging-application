using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedger
{
    internal class OracleDaily : Oracle
    {

        readonly int period;
        int actualDayIteration;


        public OracleDaily(BasketTestParameters basketSample)
        {
            RegularOracleDescription oracleDescription = (RegularOracleDescription)basketSample.RebalancingOracleDescription;
            period = oracleDescription.Period;
            actualDayIteration = 0;
        }

        public bool isRebalancing(DateTime rebalanceDate)
        {
            actualDayIteration++;
            if (actualDayIteration == period)
            {
                actualDayIteration = 0;
                return true;
            }
            return false;
        }
    }
}
