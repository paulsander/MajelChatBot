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
        public Commands() : base("commands", "Help Module: List all commands.")
        {
        }

        public override void ExecuteAction(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
            string output = "";
            int paddingWidth;
            int buffer = 6;

            output = "The following is a list of all commands available to you:\n";

            paddingWidth = GetPaddingWidth(discordBot.CommandManager.Dictionary.Keys);

            output += "```";
            foreach (string command in discordBot.CommandManager.Dictionary.Keys)
            {
                output += command.PadRight(paddingWidth + buffer) + discordBot.CommandManager.Dictionary[command].ShortDescription + "\n";
            }

            output += "```\n";

            e.User.SendMessage(output);
        }

        public override void ExecuteHelp(DiscordBot discordBot, MessageEventArgs e, string messageCommand, string messageText)
        {
        }

        private int GetPaddingWidth(Dictionary<string, IBotCommand>.KeyCollection keys)
        {
            int padlength = 0;

            foreach( string item in keys )
            {
                if (item.Length > padlength ) padlength  = item.Length;
            }

            return padlength;
        }
    }
}
