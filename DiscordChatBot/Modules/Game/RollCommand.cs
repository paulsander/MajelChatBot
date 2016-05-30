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
        public Roll() : base("roll")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            e.Channel.SendMessage("4 //chosen by fair dice roll, guaranteed to be random.");
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            e.User.SendMessage("This a placeholder for the moment.");
        }
    }
}
