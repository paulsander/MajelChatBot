using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    //A number of utilities for helping with various bot operations.
    static class Utility
    {
        public static void SplitCommandWord(string rawInput, out string firstWord,
            out string remainder)
        {
            int pos = rawInput.IndexOf(" ");

            if (pos == -1) // That means the command is a single word.
            {
                firstWord = rawInput;
                remainder = "";
            }
            else
            {
                firstWord = rawInput.Substring(0, pos);
                remainder = rawInput.Substring(pos + 1);
            }
            Console.WriteLine("Parsed -- Command '{0}' Text '{1}'", firstWord, remainder);
        }
    }
}
