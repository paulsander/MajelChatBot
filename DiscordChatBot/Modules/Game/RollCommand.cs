using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordChatBot.Modules.Game.Command
{
    class Roll : BotCommand
    {
        public Roll() : base("roll", "Rolls some dice.")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            //e.Channel.SendMessage("4 //chosen by fair dice roll, guaranteed to be random.");
            Parser.Tokenizer myParser;
            myParser = new Parser.Tokenizer();

            Stack<Parser.DieRollingToken> tokenStack;
            
            Parser.TokenizerResult myResult;

            myParser.GenerateTokens(out tokenStack, messageText, out myResult);
            if (!myResult.success)
            {
                e.Channel.SendMessage(String.Format("```Error in RollerParser while tokenizing string. Illegal character '{0}' at position {1}.\nTry '!help command roll' for more information.```", myResult.illegalChar, myResult.pos));
            }
            else
            {
                //We should now have tokenized the string.
                //For debugging purposes I will dump everything out the console.
                string consoleOutput;
                consoleOutput = "";
                bool firstLoop;
                firstLoop = false;
                foreach (Parser.DieRollingToken token in tokenStack.Reverse<Parser.DieRollingToken>())
                {
                    if (firstLoop) firstLoop = false; else consoleOutput += " ";
                    consoleOutput += ("[" + token.Text + "]");
                }
                e.Channel.SendMessage("Input tokenized. Here is the current stack: ```\n" + consoleOutput + "```");
            }
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            e.User.SendMessage(
                "You can roll a series of dice using this command. This command supports the following operations:\n" +
                "(Note: Currently this command only parses input and rejects invalid input. )\n" +
                "```" +
                " x d y          d - Performs a die roll with x number of y-sided dice.\n" +
                " x d y l z      l - Drops the z lowest dice from a given die roll.\n" +
                " x d y h z      h - Drops the z-highest dice from a given die roll.\n" +
                " x d y r z      r - Repeats a given die roll z times. (Only valid at the end of an expression.)\n" +
                " x d y + z      + - Performs the given die roll and adds z to the result.\n" +
                "                    The dice roller supports +, -, and *.\n" +
                " 2 d (4 + 5)   () - Operation order can be controlled with parentheses.\n" +
                "```" +
                "These can be combined in arbitrary ways for more complex dice rolls, for example:\n```" +
                "'4d6 l 1 r 6'             DnD character generation.\n" +
                "'1d8 + 5 + 3d8 + 1d4'     Attack roll with bonus damage.\n```" );
        }
    }
}
