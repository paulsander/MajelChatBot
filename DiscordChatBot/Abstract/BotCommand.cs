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
        private string _shortDescription;

        public BotCommand(string name, string shortDescription)
        {
            //Remove whitespace characters and assign.
            _name = name.Replace(" ", "").ToLower();

            _shortDescription = shortDescription;
        }

        #region IBotComand
        public string Name
        {
            get { return _name; }
        }

        public string ShortDescription
        {
            get { return _shortDescription; }
        }

        abstract public void ExecuteAction(DiscordBot discordBot, Discord.MessageEventArgs e, 
            string messageCommand, string messageText);

        abstract public void ExecuteHelp(DiscordBot discordBot, Discord.MessageEventArgs e,
            string messageCommand, string messageText);
        #endregion
    }
}
