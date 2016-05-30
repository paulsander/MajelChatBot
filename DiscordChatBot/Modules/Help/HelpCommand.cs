using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordChatBot.Modules.Help.Command
{
    class Help : BotCommand
    {
        public Help() : base("")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            //TODO: Find a way to gracefully pass down the "help" command into the parser.
            e.User.SendMessage(String.Format(
                "Welcome to the Majel Bot help file! For more information on what you can do, try these commands:\n" +
                "{0}{1} commands                  Get a complete list of commands.\n" +
                "{0}{1} command <name>    Get command-specific help.",
                discordBot.CommandManager.Prefix, "help"));
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
        }
    }
}
