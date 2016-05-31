using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace DiscordChatBot.Modules.Core.Command
{
    class Help : BotCommand
    {
        public Help() : base ("help", "Get help with using the bot.")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e,
                                            string messageCommand, string messageText)
        {
            //So we need to throw this into the HelpCommandModule system so it can handle all of the help stuff internally.
            discordBot.HelpCommandManager.ParseMessage(discordBot, e, messageText, false);
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e,
                                    string messageCommand, string messageText)
        {
            e.User.SendMessage(String.Format("Congrats! You know how to use the {0}{1} command.",discordBot.CommandManager.Prefix, messageCommand));
        }
    }
}
