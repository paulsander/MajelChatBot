using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    abstract class BotCommand : IBotCommand
    {
        private string _name;

        public BotCommand(string name)
        {
            //Remove whitespace characters and assign.
            _name = name.Replace(" ", "");
        }

        #region IBotComand
        public string Name
        {
            get { return _name; }
        }

        abstract public void ExecuteAction(DiscordBot discordBot, Discord.MessageEventArgs e, 
            string messageCommand, string messageText);

        abstract public void ExecuteHelp(DiscordBot discordBot, Discord.MessageEventArgs e,
            string messageCommand, string messageText);
        #endregion
    }
}
