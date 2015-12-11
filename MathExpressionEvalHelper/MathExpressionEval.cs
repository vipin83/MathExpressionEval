using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathExpressionEvalHelper
{
    public class MathExpressionEval
    {
        public Stack<Token> InfixTokens { get; set; }
        public Stack<Token> PostfixTokens { get; set; }
        public string strExpression { get; set; }

        //this will convert infix expression into postfix notation (RPN - reverse polish notation)
        public MathExpressionEval(string rawExpression)
        {
            // store the raw formula
            strExpression = rawExpression;

            InfixTokens = new Stack<Token>();
            PostfixTokens = new Stack<Token>();

            bool unaryMinus = false;
            bool unaryMinusBeforeOpeningBracket = false;

            #region generate the InFix Stack

            Stack<Token> tokens = new Stack<Token>();
            string store = "";

            // parse the expression string into a stack of tokens - infix
            for (int pos = 0; pos < rawExpression.Length; pos++)
            {
                string ThisChar = rawExpression[pos].ToString();

                #region number
                //check for digit and '.' for decimal number
                if (Regex.IsMatch(ThisChar, "[0-9\\.]"))
                {
                    // a numeric char, so store it until the end of number is reached
                    store += ThisChar;

                }
                #endregion

                #region operator +, - *
                else if (ThisChar == "+" || ThisChar == "*" || ThisChar == "-")
                {
                    // a value is stored, so add it to the stack before processing the operator
                    if (store != "")
                    {
                        //if store is not empty - that means it's a binary operator. 
                        tokens.Push(new Number(Convert.ToDecimal(store)));

                        if (unaryMinus && !unaryMinusBeforeOpeningBracket)
                        {
                            tokens.Push(new Parenthesis(ParenthesisType.Close));
                            unaryMinus = false;
                        }

                        store = "";

                    }
                

                    #region unary minus check
                    else if (ThisChar == "-") // check is this unary minus?
                    {
                        //if this is unary minus then the followig will be true -
                        // 1. it is in the first place in expression e.g. -2 + 3
                        // 2. it followes opening parantheses  e.g. 2 + (-3 + 5)
                        // 3. it follows an operator e.g. 2 * -3
                        if (pos == 0 || rawExpression[pos - 1].ToString() == "(" || Operator.GetOperatorType(rawExpression[pos - 1].ToString()) != null)
                        {
                            //insert (0- i) in place of -i
                            tokens.Push(new Parenthesis(ParenthesisType.Open));
                            tokens.Push(new Number(Convert.ToDecimal(0)));
                            unaryMinus = true;

                            if (pos == 0 && rawExpression[pos + 1].ToString() == "(") //this is for '-' (unary minus) occuring at the very begining and immediately followed by (, eg. -(2 + 3) * (7 * 8)
                            {
                                unaryMinusBeforeOpeningBracket = true;
                            }
                        }
                    }
                    #endregion
                    
                    //else if (ThisChar == "+") //this is for unary plus 
                    
                    tokens.Push(new Operator((OperatorType)Operator.GetOperatorType(ThisChar)));



                }
                #endregion

                #region paranthesis
                else if (ThisChar == ")" || ThisChar == "(")
                {
                    // a value is stored, so add it to the stack before processing the parenthesis
                    if (store != "")
                    {
                        tokens.Push(new Number(Convert.ToDecimal(store)));
                        store = "";
                    }
                    tokens.Push(new Parenthesis((ParenthesisType)Parenthesis.GetParenthesisType(ThisChar)));

                    if (unaryMinus && unaryMinusBeforeOpeningBracket && ThisChar.ToString() == ")")
                    {
                        tokens.Push(new Parenthesis(ParenthesisType.Close));
                        unaryMinus = false;
                        unaryMinusBeforeOpeningBracket = false;
                    }
                    if (unaryMinus && !unaryMinusBeforeOpeningBracket)
                    {
                        tokens.Push(new Parenthesis(ParenthesisType.Close));
                        unaryMinus = false;
                    }
                }
                #endregion

                else
                {
                    throw new Exception("Unexpected operator in expression");
                }
            }

            // if there is still something in the numeric store, add it to the stack
            if (store != "")
            {
                tokens.Push(new Number(Convert.ToDecimal(store)));
            }


            // reverse the stack
            Stack<Token> reversedStack = new Stack<Token>();
            while (tokens.Count > 0) 
                reversedStack.Push(tokens.Pop());

            // this is infix expression.
            InfixTokens = reversedStack;
            #endregion

            #region generate the PostFix Stack
            // get a reversed copy of the tokens
            Stack<Token> infixTokens = new Stack<Token>(InfixTokens);
            Stack<Token> InFixStack = new Stack<Token>();
            while (infixTokens.Count > 0) 
                InFixStack.Push(infixTokens.Pop());


            // new stacks for maintaining final output and operator
            Stack<Token> output = new Stack<Token>();
            Stack<Token> operators = new Stack<Token>();

            while (InFixStack.Count > 0)
            {
                Token currentToken = InFixStack.Pop();

                // if it's an operator
                if (currentToken.GetType() == typeof(Operator))
                {
                    // move  operators to output based on precedence, if  current opertor is less than or equal to topmost operator in stack in terms of precedence then move operaotr from stack to output
                    while (operators.Count > 0 && operators.Peek().GetType() == typeof(Operator))
                    {
                        Operator currentOperator = (Operator)currentToken;
                        Operator nextOperator = (Operator)operators.Peek();
                        if (currentOperator.Precedence <= nextOperator.Precedence)
                        {
                            output.Push(operators.Pop());
                        }
                        else
                        {
                            break;
                        }
                    }
                    // add to operators
                    operators.Push(currentToken);
                }
                // if it's a bracket
                else if (currentToken.GetType() == typeof(Parenthesis))
                {
                    switch (((Parenthesis)currentToken).ParenthesisType)
                    {
                        // if it's an opening bracket, add it to operators stack
                        case ParenthesisType.Open:
                            operators.Push(currentToken);
                            break;

                        // if it's a closing bracket
                        case ParenthesisType.Close:
                            // move all operators between opening bracket to output stack
                            while (operators.Count > 0)
                            {
                                Token nextOperator = operators.Peek();
                                if (nextOperator.GetType() == typeof(Parenthesis) && ((Parenthesis)nextOperator).ParenthesisType == ParenthesisType.Open)
                                    break;
                                output.Push(operators.Pop());
                            }
                            // remove the opening bracket from  stack
                            operators.Pop();
                            break;
                    }
                }

                // if it's numeric, add to output
                else if (currentToken.GetType() == typeof(Number))
                {
                    output.Push(currentToken);
                }

            }

            // for all remaining operators, move to output
            while (operators.Count > 0)
            {
                output.Push(operators.Pop());
            }

            // reverse the stack
            reversedStack = new Stack<Token>();
            while (output.Count > 0) 
                reversedStack.Push(output.Pop());

            // store in the Tokens property
            PostfixTokens = reversedStack;
            #endregion
        }

        public decimal Evaluate()
        {
            Stack<Number> EvaluationStack = new Stack<Number>();
            // get a reversed copy of the tokens
            Stack<Token> postFixStack = new Stack<Token>(PostfixTokens);
            Stack<Token> PostFixStack = new Stack<Token>();

            while (postFixStack.Count > 0) 
                PostFixStack.Push(postFixStack.Pop());

            while (PostFixStack.Count > 0)
            {
                Token currentToken = PostFixStack.Pop();

                if (currentToken.GetType() == typeof(Number))
                {
                    EvaluationStack.Push((Number)currentToken);
                }
                else if (currentToken.GetType() == typeof(Operator))
                {
                    Operator currentOperator = (Operator)currentToken;
                    if (currentOperator.OperatorType == OperatorType.Plus || currentOperator.OperatorType == OperatorType.Minus || currentOperator.OperatorType == OperatorType.Multiply)
                    {
                        decimal FirstValue = EvaluationStack.Pop().Value;
                        decimal SecondValue = EvaluationStack.Pop().Value;
                        decimal Result;

                        if (currentOperator.OperatorType == OperatorType.Plus)
                        {
                            Result = SecondValue + FirstValue;
                        }
                        else if (currentOperator.OperatorType == OperatorType.Minus)
                        {
                            Result = SecondValue - FirstValue;
                        }                       
                        else if (currentOperator.OperatorType == OperatorType.Multiply)
                        {
                            Result = SecondValue * FirstValue;
                        }
                        else
                        {
                            throw new Exception("Unhandled operator encountered while evaluating");
                        }
                        EvaluationStack.Push(new Number(Result));                        
                    }
                }
                else
                {
                    throw new Exception("Unexpected Token type in Evaluate method");
                }
            }

            if (EvaluationStack.Count != 1)
            {
                throw new Exception("Unexpected number of Tokens in Evaluate method");
            }
            return EvaluationStack.Peek().Value;
        }
    }

}
