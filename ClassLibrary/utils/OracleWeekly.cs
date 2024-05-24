using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Hedger
{
    internal class OracleWeekly : Oracle

    {
        int actualDayIteration;
        readonly int period = 5;


        public OracleWeekly(BasketTestParameters basketSample, DateTime initialisationDate)
        {
            WeeklyOracleDescription oracleDescription = (WeeklyOracleDescription)basketSample.RebalancingOracleDescription;
            DayOfWeek dayOfRebalancing = oracleDescription.RebalancingDay;
            DayOfWeek dayOfInitilization = initialisationDate.DayOfWeek;
            int differenceDate;
            //TODO On doit pouvoir faire un modulo ici !
            if (dayOfInitilization <= dayOfRebalancing)
            {
                differenceDate = (int)dayOfRebalancing - (int)dayOfInitilization;
            }
            else
            {
                differenceDate = 5 + (int)(dayOfRebalancing) - (int)dayOfInitilization;
            }
            actualDayIteration = 5 - differenceDate;
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
