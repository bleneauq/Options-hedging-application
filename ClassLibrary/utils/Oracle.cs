﻿using PricingLibrary.DataClasses;
using PricingLibrary.RebalancingOracleDescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hedger
{
     interface Oracle 
    {

        public Boolean isRebalancing(DateTime rebalanceDate);


    }
}
