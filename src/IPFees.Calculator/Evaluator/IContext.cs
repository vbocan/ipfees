﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPFees.Evaluator
{
    public interface IContext
    {
        decimal ResolveVariable(string name);
        decimal CallFunction(string name, decimal[] arguments);
    }
}
