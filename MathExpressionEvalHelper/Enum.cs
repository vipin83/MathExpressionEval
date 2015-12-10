using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionEvalHelper
{
    //different types of possible characters that could be present in input string
    enum CharTypes
    {
        Operator,
        Number,
        Parenthesis
    }

    //Operator it supports at the minute
    enum OperatorType
    {
        Plus,
        Minus,
        Multiply        
    }

    //open '(' and clodse ')' bracket types
    enum ParenthesisType
    {
        Open,
        Close
    }

}
