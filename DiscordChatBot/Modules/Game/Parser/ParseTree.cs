using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game.Parser
{
    public enum ParseError { None, ParseOperatorMissing, ParseOperandMissing,
        ParseMismatchedParenthesis, ParseEmptyParentheses, ParseNotEnoughClosingParentheses,
        ParseUnknownError,
        ExecuteRepeatPlacementError, ExecuteRepeatNegativeIterations,
        ExecuteDropIllegalLeftOperand, ExecuteDieNonPositiveSides, ExecuteDieNonNegativeNumRolls,
        ExecuteUndefinedError };

    public struct ParseResult
    {
        public bool success;
        public ParseError parseError;
        //TODO: Fix the inconsistent accessibility here
        internal DieRollingToken token;

        internal void SetValues(bool success = false, ParseError parseError = ParseError.None, DieRollingToken token = null )
        {
            this.success = success;
            this.parseError = parseError;
            this.token = token;
        }
    }

    public class ParseValue
    {
        private Queue<DiceGroup> _diceQueue;
        private DiceGroup _last;
        private int _value;

        public ParseValue()
        {
            _diceQueue = new Queue<DiceGroup>();
            _value = 0;
        }

        //If I'm going to add this method, I probably should be wrapping
        //  the functions I want to expose from the Queue (including Concat)
        //  so I can ensure this stays in a correct state.
        //TODO: Encapsulate the Queue methods instead of exposing them directly.
        internal void AddDieGroup(DiceGroup newGroup)
        {
            _diceQueue.Enqueue(newGroup);
            _last = newGroup;
        }

        #region Properties
        internal Queue<DiceGroup> DiceGroups
        {
            get { return _diceQueue; }
            set { _diceQueue = value; }
        }

        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        internal DiceGroup Last
        {
            get { return _last; }
            set { _last = value; }
        }
        #endregion
    }

    class ParseTree
    {
        //Private class for use with the parse tree
        private class ParseTreeNode
        {
            private ParseTreeNode _parent, _left, _right;
            private DieRollingToken _token;

            public ParseTreeNode(ParseTreeNode parent = null, ParseTreeNode left = null,
                ParseTreeNode right = null, DieRollingToken token = null)
            {
                _parent = parent;
                _left = left;
                _right = right;
                _token = token;
            }

            public ParseTreeNode Parent
            {
                get { return _parent; }
                set { _parent = value; }
            }

            public ParseTreeNode Left
            {
                get { return _left; }
                set { _left = value; }
            }

            public ParseTreeNode Right
            {
                get { return _right; }
                set { _right = value; }
            }

            public DieRollingToken Token
            {
                get { return _token; }
                set { _token = value; }
            }

        }

        private ParseTreeNode _head;
        private bool _treeReady;

        private delegate int MathDelegate(int leftOperand, int rightOperand);
        private delegate int DropDelegate(DiceGroup diceGroup, int dropNumber);

        private Dictionary<TokenType, int> _operators;
        private Dictionary<TokenType, MathDelegate> _mathDelegates;
        private Dictionary<TokenType, DropDelegate> _dropDelegates;
        private Dictionary<ParseError, string> _errorMessages;

        public ParseTree()
        {
            _treeReady = false;

            //Load the opreators dictionary, which contains all of the operators and their presedence
            _operators = new Dictionary<TokenType, int>();
            _operators.Add(TokenType.DieRoll, 1);
            _operators.Add(TokenType.DropLowest, 2);
            _operators.Add(TokenType.DropHighest, 2);
            _operators.Add(TokenType.Multiply, 3);
            _operators.Add(TokenType.Add, 4);
            _operators.Add(TokenType.Subtract, 4);
            _operators.Add(TokenType.Repeat, 5);

            //Load up our math delegates
            _mathDelegates = new Dictionary<TokenType, MathDelegate>();
            _mathDelegates.Add(TokenType.Add, AddDelegate);
            _mathDelegates.Add(TokenType.Subtract, SubtractDelegate);
            _mathDelegates.Add(TokenType.Multiply, MultiplyDelegate);

            //Load up the drop dice delegates
            _dropDelegates = new Dictionary<TokenType, DropDelegate>();
            _dropDelegates.Add(TokenType.DropLowest, DropLowDelegate);
            _dropDelegates.Add(TokenType.DropHighest, DropHighDelegate);

            //Load up the error messages.
            _errorMessages = new Dictionary<ParseError, string>();
            _errorMessages.Add(ParseError.None, "No error message.");
            _errorMessages.Add(ParseError.ParseOperatorMissing, "Error Operator Missing.");
            _errorMessages.Add(ParseError.ParseOperandMissing, "Error Operand Missing.");
            _errorMessages.Add(ParseError.ParseMismatchedParenthesis, "Error Mismatched Parenthesis.");
            _errorMessages.Add(ParseError.ParseEmptyParentheses, "Error Empty Parentheses.");
            _errorMessages.Add(ParseError.ParseNotEnoughClosingParentheses, "Error all parentheses pairs not closed.");
            _errorMessages.Add(ParseError.ParseUnknownError, "Unkown error during parsing.");
            _errorMessages.Add(ParseError.ExecuteRepeatPlacementError, "No other operations allowed on results of a repeat operator.");
            _errorMessages.Add(ParseError.ExecuteRepeatNegativeIterations, "The number of iterations in a repeat operation must be positive.");
            _errorMessages.Add(ParseError.ExecuteDropIllegalLeftOperand, "Drop operations only accept dice rolls or drop operations for the left operand.");
            _errorMessages.Add(ParseError.ExecuteDieNonPositiveSides, "Dice rolls only accept positive numbers for their right operand.");
            _errorMessages.Add(ParseError.ExecuteDieNonNegativeNumRolls, "Dice rolls do not accept negative values for their left operand.");
            _errorMessages.Add(ParseError.ExecuteUndefinedError, "Unknown error during execution.");

        }

        public string ParseTokens(Stack<DieRollingToken> tokenStack, out ParseResult parseResult)
        {
            parseResult = new ParseResult();
            parseResult.success = true; //Assume success until we set an error message.


            Head = GrammarBuilder(tokenStack, ref parseResult);

            //We'll handle output here for now.
            //First, let's check for success or failure
            if ( !parseResult.success ) // We hit some error here.
                return String.Format(GetErrorMessage(parseResult.parseError) + " token '{0}' position '{1}'", (parseResult.token != null)?parseResult.token.Text:"<null>", (parseResult.token != null)?parseResult.token.Position.ToString():"<null>");
            
            //Tree building successful. Now execute it.

            //Essentially a check for a null string.
            if(Head == null)
            {
                return "You might want to try giving some input. Use '!help command roll' for more information.";
            }

            Queue<ParseValue> results = new Queue<ParseValue>();

            results = ExecuteTree(Head, ref parseResult);

            //Execution error results here...
            if (!parseResult.success)
                return String.Format(GetErrorMessage(parseResult.parseError) + " token '{0}' position '{1}'", parseResult.token.Text, parseResult.token.Position);

            //Finally we can report success.
            //We'll need to build up the results to report back.
            StringBuilder resultsString = new StringBuilder();
            Console.WriteLine("Beginning formatting...");
            resultsString.Append("```\n");

            foreach( ParseValue parseValue in results )
            {
                Console.WriteLine("Entering parseValue/results loop. Dicegroups count {0}", parseValue.DiceGroups.Count);
                //Outer loop over each parse result
                resultsString.Append("[ "); //Opening bracket
                foreach (DiceGroup dicegroup in parseValue.DiceGroups)
                {
                    Console.WriteLine("Entering dicegroup/Dicegroups loop");
                    resultsString.Append("[ "); //open DiceGroup
                    foreach (Die die in dicegroup.Dice)
                    {
                        Console.WriteLine("Entering die/Dicegroup loop");
                        resultsString.Append("[");
                        if (die.Discarded)
                            resultsString.AppendFormat("~~{0}~~", die.Value);
                        else
                            resultsString.AppendFormat("{0}", die.Value);
                        resultsString.Append("]");
                    }
                    resultsString.Append(" ]");
                }
                resultsString.AppendFormat(" ] Total: {0}\n", parseValue.Value);
            }
            resultsString.Append("```");

            if (resultsString.Length > 1900 )
            {
                //If the string is really long we're just going to summarize the results instead.
                StringBuilder summarizeResults = new StringBuilder();
                summarizeResults.Append("Results too long to display. Summarizing results: ");
                summarizeResults.Append(String.Join(", ", from parseValue in results select parseValue.Value));

                if (summarizeResults.Length > 1900)
                    return "Unable to summarize results. It's probably Grath's fault.";
                else
                    return summarizeResults.ToString();
            }
            return resultsString.ToString();
        }

        #region Helper Functions
        private ParseTreeNode GrammarBuilder(Stack<DieRollingToken> tokenStack, ref ParseResult result, int depth = 0)
        {
            DieRollingToken current;
            result.success = true; //Assume true until we encounter an error that sets it to false.
            ParseTreeNode pointer = null; //For keeping track of the tree traversal
            ParseTreeNode head = null;

            while ( tokenStack.Count > 0 )
            {
                current = tokenStack.Pop();

                if (IsNumber(current.Type))
                {
                    #region Parsing a new number node
                    //Now go through all the possible cases and take the appropraite action.
                    if (pointer == null) //Ideally this should be the starting state of a new tree
                    {
                        ParseTreeNode newNode = new ParseTreeNode(token: current);
                        head = pointer = newNode;
                    }
                    else if (IsNumber(pointer.Token.Type))
                    {
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseOperatorMissing;
                    }
                    else if (IsOperator(pointer.Token.Type))
                    {
                        if (pointer.Right == null) //Operator has room for another number
                        {
                            ParseTreeNode newNode = new ParseTreeNode(token: current, parent: pointer);
                            pointer.Right = newNode;
                        }
                        else //The operator is already 'full' and can't take another number.
                        {
                            result.success = false;
                            result.token = current;
                            result.parseError = ParseError.ParseOperatorMissing;
                        }
                    }
                    #endregion
                }
                else if (IsOperator(current.Type))
                {
                    #region Parsing a new operator node
                    if (pointer == null) //This is an empty tree
                    {
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseOperandMissing;
                    }
                    else if (IsNumber(pointer.Token.Type))
                    {
                        //Consistent state check. This shouldn't happen, but just in case:
                        if (pointer.Parent != null)
                        {
                            //This means we're pointing at a number with a parent, which shouldn't happen.
                            // The only number we should be pointing at is the initial one we start the
                            // tree with.
                            InconsistentStateException e = new InconsistentStateException(current);
                            throw e;
                        }
                        else
                        {
                            //Slot the opreator in above the number.
                            ParseTreeNode newNode = new ParseTreeNode(token: current, left: pointer);
                            pointer.Parent = newNode;
                            head = pointer = newNode; // Walk the pointer and head up to the new node.
                        }
                    }
                    else if (pointer.Right == null) //This means the current opreator doesn't have multiple inputs
                    {
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseOperandMissing;
                    }
                    else if (Presedence(current.Type) < Presedence(pointer.Token.Type)) //Lower number is higher priorty
                    {
                        //In this case what we do is detach the right side branch to slot the new operator in
                        ParseTreeNode newNode = new ParseTreeNode(token: current, parent: pointer, left: pointer.Right);
                        pointer.Right = newNode;
                        pointer = newNode;
                    } else //Presedence(current.Type) >= Presedence(pointer.Token.Type)
                    {
                        //If the new operator is equal or lower presedence, then we should walk back up the tree
                        // to slot it at the top.
                        pointer = head;
                        ParseTreeNode newNode = new ParseTreeNode(token: current, left: pointer);
                        pointer.Parent = newNode;
                        head = pointer = newNode;
                    }
                    #endregion
                }
                else if (IsLeftParen(current.Type))
                {
                    #region Parsing left parenthesis
                    if (pointer == null)
                    {
                        pointer = GrammarBuilder(tokenStack, ref result, depth + 1);
                        head = pointer;
                    }
                    else if (IsNumber(pointer.Token.Type))
                    {
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseOperatorMissing;
                    }
                    else if (IsOperator(pointer.Token.Type))
                    {
                        if (pointer.Right != null)
                        {
                            //Someone attmpted to use parentheses without an operator
                            result.success = false;
                            result.token = current;
                            result.parseError = ParseError.ParseOperatorMissing;
                        }
                        else
                        {
                            // This means the expression around the parenthesis is correct
                            pointer.Right = GrammarBuilder(tokenStack, ref result, depth + 1);
                            pointer.Right.Parent = pointer;
                            head = pointer;
                        }
                    }
                    #endregion
                }
                else if (IsRightParent(current.Type))
                {
                    #region Parsing right parenthesis
                    if (pointer == null)
                    {
                        // This means the parenthesis expression was an error.
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseEmptyParentheses;
                        return head;
                    }
                    else if (depth == 0)
                    {
                        // This means we have too many right parentheses.
                        result.success = false;
                        result.token = current;
                        result.parseError = ParseError.ParseMismatchedParenthesis;
                    }
                    else if (IsNumber(pointer.Token.Type))
                    {
                        return head;
                    }
                    else if (IsOperator(pointer.Token.Type))
                    {
                        if (pointer.Right == null)
                        {
                            // This is an error as it means there's an unfinished expression
                            result.success = false;
                            result.token = current;
                            result.parseError = ParseError.ParseOperandMissing;
                            return head;
                        }
                        else
                        {
                            //We should have built a complete subtree. REturn it.
                            return head;
                        }
                    }
                    #endregion
                }
                if ( result.success == false )
                {
                    return head;
                }
            }
            // If we've gotten here it means we've successfully parsed everything in the stack.
            // Now we just need to do the depth check to make sure all the parentheses were matched up properly.
            if (depth != 0 )
            {
                //This means we're still recursing.
                result.success = false;
                result.token = null;
                result.parseError = ParseError.ParseNotEnoughClosingParentheses;
            }
            return head;
        }

        private Queue<ParseValue> ExecuteTree( ParseTreeNode pointer, ref ParseResult parseResult, int depth = 0 )
        {
            Queue<ParseValue> returnResult = new Queue<ParseValue>();

            //First we check for the node type...
            if (pointer.Token.Type == TokenType.Repeat)
            {
                #region Repeat Token
                //Repeat most be the topmost node in a tree, because doing
                // operations on a set of results is semantically unclear.
                if (pointer.Parent != null)
                {
                    parseResult.SetValues(false, ParseError.ExecuteRepeatPlacementError, pointer.Token);
                    return null;
                }
                else
                {
                    //First we'll execute the right node, since we need to know the repeat number.
                    Queue<ParseValue> rightResult = ExecuteTree(pointer.Right, ref parseResult, depth + 1);

                    if (parseResult.success == false)
                        return null;

                    //Make sure we're going for a non-zero number of repeats.
                    if (rightResult.Peek().Value < 0)
                    {
                        parseResult.SetValues(false, ParseError.ExecuteRepeatNegativeIterations, pointer.Token);
                        return null;
                    }

                    //If we got this far, we're ready to to go.
                    for (int i = 0; i < rightResult.Peek().Value; i++)
                    {
                        returnResult.Enqueue(ExecuteTree(pointer.Left, ref parseResult, depth + 1).Dequeue());
                    }

                    foreach (ParseValue parseValue in rightResult)
                    {
                        if (parseValue.DiceGroups.Count != 0) //This means there are dice rolls we need
                            returnResult.Enqueue(parseValue);
                    }

                    if (parseResult.success)
                        return returnResult;
                    else
                        return null;
                }
                #endregion
            }
            else if (IsMathOperator(pointer.Token.Type))
            {
                #region Math Token
                Queue<ParseValue> leftResult = ExecuteTree(pointer.Left, ref parseResult, depth + 1);

                if (parseResult.success == false)
                    return null;

                Queue<ParseValue> rightResult = ExecuteTree(pointer.Right, ref parseResult, depth + 1);

                if (parseResult.success == false)
                    return null;

                //Now we need to combine our results.
                ParseValue newResult = new ParseValue();

                newResult.Value = _mathDelegates[pointer.Token.Type](leftResult.Peek().Value, rightResult.Peek().Value);
                foreach (DiceGroup diceGroup in leftResult.Peek().DiceGroups)
                    newResult.DiceGroups.Enqueue(diceGroup);
                foreach (DiceGroup diceGroup in rightResult.Peek().DiceGroups)
                    newResult.DiceGroups.Enqueue(diceGroup);

                Queue<ParseValue> mathResult = new Queue<ParseValue>();
                mathResult.Enqueue(newResult);

                return mathResult;
                #endregion
            }
            else if (IsDropOperator(pointer.Token.Type))
            {
                #region Drop Token
                //First, we need to check that we're only operating on a direct
                // die roll or the result of another drop operation.
                if (!(IsDieOperator(pointer.Left.Token.Type) || IsDropOperator(pointer.Left.Token.Type)))
                {
                    parseResult.SetValues(false, ParseError.ExecuteDropIllegalLeftOperand, pointer.Token);
                    return null;
                }

                //So now we can do the actual drop operation.
                //First we need the right operand.
                Queue<ParseValue> rightResult = ExecuteTree(pointer.Right, ref parseResult, depth + 1);

                if (!parseResult.success)
                    return null;

                //Now we execute the left side...
                Queue<ParseValue> leftResult = ExecuteTree(pointer.Left, ref parseResult, depth + 1);
                if (!parseResult.success)
                    return null;

                //Now we need to execute the actual drop, and then modify the total by the dropped diee's value
                leftResult.Peek().Value -= _dropDelegates[pointer.Token.Type](leftResult.Peek().Last, rightResult.Peek().Value);

                //So the value from Left is what we'll pass up, but we still want any die rolls
                // from the right to be passed along, too.
                // Note that Last is not modified, so we can do another drop in the next
                // level up.
                foreach (DiceGroup diceGroup in rightResult.Peek().DiceGroups)
                {
                    leftResult.Peek().DiceGroups.Enqueue(diceGroup);
                }

                return leftResult;
                #endregion
            }
            else if ( IsDieOperator(pointer.Token.Type) )
            {
                #region Die Roll Token
                //The fun stuff.
                Queue<ParseValue> rightResult = ExecuteTree(pointer.Right, ref parseResult, depth + 1);

                if (!parseResult.success)
                    return null;

                //Check right operand for validity
                if (rightResult.Peek().Value < 1 ) // zero or negative-sided dice
                {
                    parseResult.SetValues(false, ParseError.ExecuteDieNonPositiveSides, pointer.Token);
                    return null;
                }

                Queue<ParseValue> leftResult = ExecuteTree(pointer.Left, ref parseResult, depth + 1);
                if (!parseResult.success)
                    return null;

                //Check left operand for validity
                if (leftResult.Peek().Value < 0 ) // Can't roll a negative number of dice
                {
                    parseResult.SetValues(false, ParseError.ExecuteDieNonNegativeNumRolls, pointer.Token);
                    return null;
                }

                //This will destroy the old value in both operands but preserve the dice groups
                DiceGroup newGroup = new DiceGroup();
                int sum = 0;

                for (int i = 0; i < leftResult.Peek().Value; i++)
                    sum += newGroup.AddDie(rightResult.Peek().Value);

                ParseValue newParseValue = new ParseValue();
                newParseValue.Value = sum;
                foreach (DiceGroup diceGroup in leftResult.Peek().DiceGroups)
                    newParseValue.DiceGroups.Enqueue(diceGroup);

                foreach (DiceGroup diceGroup in rightResult.Peek().DiceGroups)
                    newParseValue.DiceGroups.Enqueue(diceGroup);

                newParseValue.DiceGroups.Enqueue(newGroup);
                newParseValue.Last = newGroup;

                Queue<ParseValue> dieResult = new Queue<ParseValue>();
                dieResult.Enqueue(newParseValue);

                return dieResult;
                #endregion
            }
            else if ( IsNumber(pointer.Token.Type) )
            {
                #region Number Token
                //End of the line.
                //No nodes to traverse, as the structure should already be guaranteed correct
                int value;
                int.TryParse(pointer.Token.Text, out value);

                ParseValue newParseValue = new ParseValue();
                newParseValue.Value = value;

                Queue<ParseValue> numberResult = new Queue<ParseValue>();
                numberResult.Enqueue(newParseValue);

                return numberResult;
                #endregion
            }
            else
            {
                parseResult.SetValues(false, ParseError.ExecuteUndefinedError, pointer.Token);
                return null;
            }
        }
        #endregion

        #region Type Checking Helpers
        //TODO: I might want to possibly reorganize this so that instead of checking
        // the type separately with multple function calls, I instead have a single
        // type that returns an enum that I then switch on.
        private bool IsOperator(TokenType tokenType)
        {
            return _operators.ContainsKey(tokenType);
        }

        private int Presedence(TokenType tokenType)
        {
            return _operators[tokenType];
        }

        private bool IsNumber(TokenType tokenType)
        {
            return tokenType == TokenType.Number;
        }

        private bool IsLeftParen(TokenType tokenType)
        {
            return tokenType == TokenType.LeftParen;
        }

        private bool IsRightParent(TokenType tokenType)
        {
            return tokenType == TokenType.RightParen;
        }

        private bool IsMathOperator(TokenType tokenType)
        {
            return tokenType == TokenType.Add || tokenType == TokenType.Subtract
                || tokenType == TokenType.Multiply;
        }

        private bool IsDropOperator(TokenType tokenType)
        {
            return tokenType == TokenType.DropLowest || tokenType == TokenType.DropHighest;
        }

        private bool IsDieOperator(TokenType tokenType )
        {
            return tokenType == TokenType.DieRoll;
        }
        #endregion

        #region Output
        public void ConsoleDump()
        {
            ConsoleDumpRecurseHelper(Head);
        }

        private void ConsoleDumpRecurseHelper(ParseTreeNode node, int depth = 0)
        {
            int multiplier = 3;

            if (node == null)
            {
                return;
            }
            else
            {                //Print main node first.
                string output = node.Token.Text;
                output = output.PadLeft(depth * multiplier);
                Console.WriteLine(output);

                ConsoleDumpRecurseHelper(node.Left, depth + 1);
                ConsoleDumpRecurseHelper(node.Right, depth + 1);
            }
        }

        private string GetErrorMessage(ParseError parseError)
        {
            return _errorMessages[parseError];
        }
        #endregion

        #region Delegates
        private int AddDelegate(int leftOperand, int rightOperand)
        {
            return leftOperand + rightOperand;
        }

        private int SubtractDelegate(int leftOperand, int rightOperand)
        {
            return leftOperand - rightOperand;
        }

        private int MultiplyDelegate(int leftOperand, int rightOperand)
        {
            return leftOperand * rightOperand;
        }

        private int DropLowDelegate(DiceGroup dieGroup, int dropNumber )
        {
            return dieGroup.DropLow(dropNumber);
        }

        private int DropHighDelegate(DiceGroup dieGroup, int dropNumber)
        {
            return dieGroup.DropHigh(dropNumber);
        }


        #endregion

        #region Properties
        private ParseTreeNode Head
        {
            get { return _head; }
            set { _head = value; }
        }
        #endregion
    }
}