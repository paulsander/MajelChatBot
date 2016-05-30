using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordChatBot.Modules.Help.Command
{
    class Command : BotCommand
    {
        public Command() : base("command")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            string commandWord = "";
            string remainder = "";

            Utility.SplitCommandWord(messageText, out commandWord, out remainder);

            if (discordBot.CommandManager.Dictionary.ContainsKey(commandWord))
            {
                discordBot.CommandManager.Dictionary[commandWord].ExecuteHelp(discordBot, e, commandWord, remainder);
            }
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
        }
    }
}
