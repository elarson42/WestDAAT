﻿using System;
using WesternStatesWater.WaDE.Common;
using Microsoft.Extensions.Logging;

namespace WesternStatesWater.WaDE.Common
{
    public abstract class ServiceContractBase
    {
        // just to make sure all our manager, engines, and accessors have a TestMe method for smoke tests
        public virtual string TestMe(string input)
        {
            string result = $"{input} : {GetType().Name}";
            Console.WriteLine(result);
            return result;
        }

        protected ILogger Logger { get; set; }

        public AmbientContext Context { get; set; }
    }
}
