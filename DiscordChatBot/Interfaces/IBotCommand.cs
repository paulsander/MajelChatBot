using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    interface IBotCommand
    {
        string Name { get; }

        string ShortDescription { get; }

        //TODO: Wrap the Discord MessageEventArgs in another class so this is
        // platform-agnostic.
        void ExecuteAction(DiscordBot discordBot, Discord.MessageEventArgs e,
            string messageCommand, string messageText);

        void ExecuteHelp(DiscordBot discordBot, Discord.MessageEventArgs e,
            string messageCommand, string messageText);
    }
}
