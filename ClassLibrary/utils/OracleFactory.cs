using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hedger
{
    internal class OracleFactory
    {
        public static Oracle buildOracle(RebalancingOracleType type, BasketTestParameters basketOption, DateTime initialisationDate)
        {


            switch (type)
            {
                case RebalancingOracleType.Regular:
                    OracleDaily daily = new OracleDaily(basketOption);
                    return daily;

                case RebalancingOracleType.Weekly:
                    OracleWeekly weekly = new OracleWeekly(basketOption, initialisationDate);
                    return weekly;

                default:
                    throw new NotImplementedException("Wrong type of oracle");
            }
        }
    }
}