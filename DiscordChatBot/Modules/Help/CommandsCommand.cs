using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordChatBot.Modules.Help.Command
{
    class Commands : BotCommand
    {
        public Commands() : base("commands")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            e.User.SendMessage("The following is a list of all commands available to you:\n" +
                                String.Join(", ", discordBot.CommandManager.Dictionary.Keys));
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
        }
    }
}
