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

            const string operators = "dlh*+-r()";
            const string numbers = "0123456789";
            const string whitespace = " \t\n";

            int post = 0;

            foreach (char currentChar in input)
            {
                if (operators.IndexOf(currentChar) >= 0)
                {
                    //If true, we have a regular operator. Create the token and push it onto the stack.

                    //Were we already processing a number?
                    if (currentToken != "")
                    {
                        //... then we need to push the number to the stack and keep going.
                        Console.WriteLine("Pushing a number to stack: {0}", currentToken);
                        tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken));
                        currentToken = ""; //.. and wipe the old number.
                        Console.WriteLine("Stack depth: {0}", tokenStack.Count);
                    }

                    Console.WriteLine("Pushing a character to stack: {0}", currentChar);
                    switch (currentChar)
                    {
                        case 'd': tokenStack.Push(new DieRollingToken(TokenType.DieRoll, currentChar.ToString())); break;
                        case 'l': tokenStack.Push(new DieRollingToken(TokenType.DropLowerst, currentChar.ToString())); break;
                        case 'h': tokenStack.Push(new DieRollingToken(TokenType.DropHighest, currentChar.ToString())); break;
                        case '*': tokenStack.Push(new DieRollingToken(TokenType.Multiply, currentChar.ToString())); break;
                        case '+': tokenStack.Push(new DieRollingToken(TokenType.Add, currentChar.ToString())); break;
                        case '-': tokenStack.Push(new DieRollingToken(TokenType.Subtract, currentChar.ToString())); break;
                        case 'r': tokenStack.Push(new DieRollingToken(TokenType.Repeat, currentChar.ToString())); break;
                        case '(': tokenStack.Push(new DieRollingToken(TokenType.LeftParen, currentChar.ToString())); break;
                        case ')': tokenStack.Push(new DieRollingToken(TokenType.RightParen, currentChar.ToString())); break;
                    }
                    Console.WriteLine("Stack depth: {0}", tokenStack.Count);
                }
                else if (numbers.IndexOf(currentChar) >= 0)
                {
                    //Here we have a number.
                    currentToken += currentChar.ToString();
                }
                else if (whitespace.IndexOf(currentChar) >= 0)
                {
                    //Ignore the whitespace, but dump any current number we're working on.
                    if (currentToken != "")
                    {
                        tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken));
                        currentToken = "";
                    }

                }
                else
                {
                    //If we got all the way down here we don't have a supported character
                    result.success = false;
                    result.pos = post;
                    result.illegalChar = currentChar;
                    return;
                }
                post++;
            }
            //If we're still processing a number, now's the time to dump it.
            if (currentToken != "")
                tokenStack.Push(new DieRollingToken(TokenType.Number, currentToken));

            result.success = true;
            result.pos = 0;
            result.illegalChar = ' ';
            return;
        }
    }
}
