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
            Parser.DiceParser myParser;
            myParser = new Parser.DiceParser();

            Stack<Parser.RollerToken> tokenStack;
            bool success;

            Parser.ParseResult myResult;
            myResult = new Parser.ParseResult();

            myParser.GenerateTokens(out tokenStack, out success, messageText, out myResult);
            if (!success)
            {
                e.Channel.SendMessage(String.Format("```Error in RollerParser while tokenizing string. Illegal character '{0}' at position {1}.```", myResult.illegalChar, myResult.pos));
            }
            else
            {
                //We should now have tokenized the string.
                //For debugging purposes I will dump everything out the console.
                string consoleOutput;
                consoleOutput = "";
                bool firstLoop;
                firstLoop = false;
                foreach (Parser.RollerToken token in tokenStack.Reverse<Parser.RollerToken>())
                {
                    if (firstLoop) firstLoop = false; else consoleOutput += " ";
                    consoleOutput += ("[" + token.Text + "]");
                }
                e.Channel.SendMessage("Input tokenized. Here is the current stack: ```" + consoleOutput + "```");
            }
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            e.User.SendMessage("This a placeholder for the moment.");
        }
    }
}
