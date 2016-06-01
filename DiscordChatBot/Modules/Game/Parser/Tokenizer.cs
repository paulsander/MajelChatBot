using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot.Modules.Game.Parser
{
    public struct TokenizerResult
    {
        public int pos;
        public char illegalChar;
        public bool success;
    }

    class Tokenizer
    {
        public Tokenizer()
        {
        }

        public void GenerateTokens(out Stack<DieRollingToken> tokenStack, string input, out TokenizerResult result)
        {
            tokenStack = new Stack<DieRollingToken>();

            string currentToken = "";
            result = new TokenizerResult();
            int tokenStart = -1;
            

            TokenType currTokenType = new TokenType();

            const string operators = "dlh*+-r()";
            const string numbers = "0123456789";
            const string whitespace = " \t\n";

            int pos = 0;

            foreach (char currentChar in input)
            {
                if (operators.IndexOf(currentChar) >= 0)
                {
                    //If true, we have a regular operator. Create the token and push it onto the stack.

                    //Were we already processing a number?
                    if (currentToken != "")
                    {
                        //... then we need to push the number to the stack and keep going.
                        tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken, tokenStart));
                        currentToken = ""; //.. and wipe the old number.
                        tokenStart = -1; // And reset the position pointer.
                    }

                    switch (currentChar)
                    {
                        case 'd': currTokenType = TokenType.DieRoll; break;
                        case 'l': currTokenType = TokenType.DropLowerst; break;
                        case 'h': currTokenType = TokenType.DropHighest; break;
                        case '*': currTokenType = TokenType.Multiply; break;
                        case '+': currTokenType = TokenType.Add; break;
                        case '-': currTokenType = TokenType.Subtract; break;
                        case 'r': currTokenType = TokenType.Repeat; break;
                        case '(': currTokenType = TokenType.LeftParen; break;
                        case ')': currTokenType = TokenType.RightParen;  break;
                    }

                    tokenStack.Push(new DieRollingToken(currTokenType, currentChar.ToString(), pos));
                }
                else if (numbers.IndexOf(currentChar) >= 0)
                {
                    //Here we have a number.
                    currentToken += currentChar.ToString();
                    //Check if tokenStart is -1, and if it's not, set it to the current position.
                    if (tokenStart == -1) tokenStart = pos;
                }
                else if (whitespace.IndexOf(currentChar) >= 0)
                {
                    //Ignore the whitespace, but dump any current number we're working on.
                    if (currentToken != "")
                    {
                        tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken, tokenStart));
                        currentToken = "";

                        tokenStart = -1;
                    }

                }
                else
                {
                    //If we got all the way down here we don't have a supported character
                    result.success = false;
                    result.pos = pos;
                    result.illegalChar = currentChar;
                    return;
                }
                pos++;
            }
            //If we're still processing a number, now's the time to dump it.
            if (currentToken != "")
                tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken, tokenStart));

            result.success = true;
            result.pos = 0;
            result.illegalChar = ' ';
            return;
        }
    }
}
