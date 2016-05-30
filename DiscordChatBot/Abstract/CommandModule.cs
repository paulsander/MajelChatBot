using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordChatBot
{
    abstract class CommandModule : ICommandModule
    {
        private string _name;
        private List<IBotCommand> _commandList;

        public CommandModule(string name)
        {
            _name = name;
            _commandList = new List<IBotCommand>();
        }

        #region ICommandModule
        public string Name
        {
            get { return _name; }
        }
        public List<IBotCommand> CommandList
        {
            get { return _commandList; }
        }

        public void AddCommand(IBotCommand botCommand)
        {
            _commandList.Add(botCommand);
        }
        #endregion
    }
}
