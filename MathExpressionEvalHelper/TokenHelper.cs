using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathExpressionEvalHelper
{
    public class Token { }

    class Operator : Token
    {
        public OperatorType OperatorType { get; set; }
        public Operator(OperatorType operatorType)
        {
            OperatorType = operatorType;
        }

        public int Precedence
        {
            get
            {
                switch (this.OperatorType)
                {
                    case OperatorType.Multiply:                    
                        return 2;
                    case OperatorType.Plus:
                    case OperatorType.Minus:
                        return 1;
                    default:
                        throw new Exception("Invalid Operator Type");
                }
            }
        }

        public override string ToString()
        {
            switch (OperatorType)
            {
                case OperatorType.Plus: 
                    return "+";
                case OperatorType.Minus: 
                    return "-";
                case OperatorType.Multiply: 
                    return "*";  
                default: 
                    return null;
            }
        }

        public static OperatorType? GetOperatorType(string operatorValue)
        {
            switch (operatorValue)
            {
                case "+": 
                    return OperatorType.Plus;
                case "-": 
                    return OperatorType.Minus;
                case "*": 
                    return OperatorType.Multiply;                
                default: 
                    return null;
            }
        }
    } //end of operator types function

    class Parenthesis : Token
    {
        public ParenthesisType ParenthesisType { get; set; }
        public Parenthesis(ParenthesisType parenthesisType)
        {
            ParenthesisType = parenthesisType;
        }

        public override string ToString()
        {
            if (ParenthesisType == ParenthesisType.Open)
                return "(";
            else
                return ")";
        }
        public static ParenthesisType? GetParenthesisType(string parenthesisValue)
        {
            switch (parenthesisValue)
            {
                case "(": 
                    return ParenthesisType.Open;
                case ")": 
                    return ParenthesisType.Close;
                default: 
                    return null;
            }
        }
    } //end of parantheses types 

    class Number : Token
    {
        public decimal Value { get; set; }
        public Number(decimal value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    } //end of decimal number type

}
