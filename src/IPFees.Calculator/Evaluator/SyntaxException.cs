﻿using System;

namespace IPFees.Evaluator
{
    // Exception for syntax errors
    public class SyntaxException : Exception
    {
        public SyntaxException(string message) 
            : base(message)
        {
        }
    }
}
